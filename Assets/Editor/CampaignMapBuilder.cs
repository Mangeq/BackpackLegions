using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using UnityEditor.SceneManagement;
using Unity.AI.Navigation;
using Object = UnityEngine.Object;

public static class CampaignMapBuilder
{
    [Serializable] class MaterialInfo { public string name; public float[] color; }
    [Serializable] class Report
    {
        public int chapter;
        public string name;
        public float[] spawn, goal;
        public MaterialInfo[] materials;
    }
    static Vector3 V(float[] p) => new Vector3(p[0], p[1], p[2]);
    const string Folder = "Assets/Prefabs/CampaignMaps";
    [MenuItem("Backpack/Build Five Campaign Maps")]
    public static void Build()
    {
        if (EditorApplication.isPlaying) throw new Exception("Stop Play before building maps.");
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (scene.path != "Assets/Scenes/GameScane.unity") throw new Exception("Open GameScane first.");
        NavMesh.RemoveAllNavMeshData();
        Directory.CreateDirectory(Folder);
        var maps = new List<CampaignMap>();
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) throw new Exception("URP Lit shader missing.");
        foreach (var jsonPath in Directory.GetFiles("Assets/Art/CampaignMaps", "*.json").Where(p => !p.EndsWith("manifest.json")).OrderBy(p => p))
        {
            var report = JsonUtility.FromJson<Report>(File.ReadAllText(jsonPath));
            var modelPath = Path.ChangeExtension(jsonPath, ".fbx").Replace('\\', '/');
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            if (model == null) throw new Exception("Missing imported model: " + modelPath);
            var root = new GameObject("Chapter " + report.chapter + " - " + report.name);
            try
            {
                var geometry = Object.Instantiate(model, root.transform);
                geometry.name = "Blender Terrain";
                // Blender's FBX basis is rotated 180 degrees around Unity's vertical axis.
                geometry.transform.localRotation = Quaternion.Euler(0, 180, 0);
                var materials = new Dictionary<string, Material>();
                foreach (var m in report.materials)
                {
                    string assetPath = Folder + "/" + report.name + "_" + m.name + ".mat";
                    var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                    if (material == null) { material = new Material(shader); AssetDatabase.CreateAsset(material, assetPath); }
                    material.color = new Color(m.color[0], m.color[1], m.color[2], 1);
                    material.SetFloat("_Smoothness", .12f);
                    EditorUtility.SetDirty(material);
                    materials[m.name] = material;
                }
                foreach (var renderer in geometry.GetComponentsInChildren<MeshRenderer>())
                {
                    renderer.sharedMaterials = renderer.sharedMaterials.Select(m => m != null && materials.ContainsKey(m.name) ? materials[m.name] : m).ToArray();
                    renderer.gameObject.isStatic = true;
                    if (renderer.name == "Ground" || renderer.name == "Road") renderer.gameObject.AddComponent<MeshCollider>();
                }
                var map = root.AddComponent<CampaignMap>();
                map.spawnPoint = new GameObject("Enemy Entry").transform;
                map.spawnPoint.SetParent(root.transform, false); map.spawnPoint.localPosition = V(report.spawn);
                map.goal = new GameObject("Castle Goal").transform;
                map.goal.SetParent(root.transform, false); map.goal.localPosition = V(report.goal);
                map.goal.gameObject.tag = "Base";
                map.goal.gameObject.AddComponent<Base>().maxHealth = 100;
                var goalCollider = map.goal.gameObject.AddComponent<BoxCollider>();
                goalCollider.isTrigger = true; goalCollider.size = new Vector3(2.1f, 3, 2.1f); goalCollider.center = Vector3.up;
                var rb = map.goal.gameObject.AddComponent<Rigidbody>(); rb.isKinematic = true; rb.useGravity = false;
                foreach (var pad in geometry.GetComponentsInChildren<MeshRenderer>().Where(r => r.name.StartsWith("Pad_")))
                {
                    // Mesh origins are baked at world zero; the bounds identify the platform center.
                    var center = pad.bounds.center; center.y = .40f;
                    var slot = new GameObject(pad.name + " Build Slot"); slot.transform.SetParent(root.transform, false); slot.transform.position = center;
                    var towerSlot = slot.AddComponent<TowerSlot>(); towerSlot.platformRenderers = new Renderer[] { pad };
                    var collider = slot.AddComponent<BoxCollider>(); collider.size = new Vector3(2.6f, .6f, 2.6f);
                }
                var road = geometry.GetComponentsInChildren<MeshFilter>().Single(m => m.name == "Road");
                var sources = new List<NavMeshBuildSource> { new NavMeshBuildSource { shape = NavMeshBuildSourceShape.Mesh,
                    sourceObject = road.sharedMesh, transform = road.transform.localToWorldMatrix, area = 0 } };
                var settings = NavMesh.GetSettingsByID(-1372625422);
                settings.overrideVoxelSize = true; settings.voxelSize = .06f;
                var data = NavMeshBuilder.BuildNavMeshData(settings, sources, new Bounds(Vector3.zero, new Vector3(100, 20, 90)), Vector3.zero, Quaternion.identity);
                if (data == null) throw new Exception("Navigation bake failed: " + report.name);
                var navPath = Folder + "/" + report.name + "_Navigation.asset";
                var existing = AssetDatabase.LoadAssetAtPath<NavMeshData>(navPath);
                if (existing != null) { EditorUtility.CopySerialized(data, existing); Object.DestroyImmediate(data); data = existing; EditorUtility.SetDirty(existing); }
                else AssetDatabase.CreateAsset(data, navPath);
                var surface = root.AddComponent<NavMeshSurface>(); surface.agentTypeID = settings.agentTypeID; surface.navMeshData = data;
                surface.AddData();
                var filter = new NavMeshQueryFilter { agentTypeID = settings.agentTypeID, areaMask = NavMesh.AllAreas };
                NavMeshHit start, finish;
                if (!NavMesh.SamplePosition(map.spawnPoint.position, out start, 2, filter) || !NavMesh.SamplePosition(map.goal.position, out finish, 2, filter))
                    throw new Exception("Entry or castle not on road: " + report.name);
                map.spawnPoint.position = start.position; map.goal.position = finish.position;
                var path = new NavMeshPath(); NavMesh.CalculatePath(start.position, finish.position, filter, path);
                if (path.status != NavMeshPathStatus.PathComplete) throw new Exception("Incomplete road: " + report.name);
                int count = root.GetComponentsInChildren<TowerSlot>().Length;
                if (count != report.chapter + 4 || count > 9) throw new Exception("Wrong platform count.");
                var prefab = PrefabUtility.SaveAsPrefabAsset(root, Folder + "/Chapter" + report.chapter.ToString("00") + ".prefab");
                maps.Add(prefab.GetComponent<CampaignMap>());
            }
            finally { Object.DestroyImmediate(root); }
        }
        if (maps.Count != 5) throw new Exception("All five maps must be present.");
        var loader = Object.FindObjectOfType<CampaignMapLoader>();
        if (loader == null) loader = new GameObject("Campaign Map Loader").AddComponent<CampaignMapLoader>();
        if (loader.editorPreview != null) Object.DestroyImmediate(loader.editorPreview);
        foreach (var old in scene.GetRootGameObjects().Where(g => g.name.StartsWith("Blackpine") || g.name == "towers")) old.SetActive(false);
        loader.maps = maps.ToArray();
        loader.editorPreview = PrefabUtility.InstantiatePrefab(maps[0].gameObject) as GameObject;
        Object.FindObjectOfType<WaveManager>().spawnPoint = loader.editorPreview.GetComponent<CampaignMap>().spawnPoint;
        string[] names = { "ÇAM NÖBETİ", "KIZIL KANYON", "SİS BATAKLIĞI", "BUZ GEÇİDİ", "TAÇ HARABELERİ" };
        foreach (var guid in AssetDatabase.FindAssets("t:ChapterData"))
        {
            var chapter = AssetDatabase.LoadAssetAtPath<ChapterData>(AssetDatabase.GUIDToAssetPath(guid));
            if (chapter.chapterNumber < 1 || chapter.chapterNumber > 5) continue;
            chapter.chapterName = names[chapter.chapterNumber - 1]; chapter.sceneName = "GameScane"; EditorUtility.SetDirty(chapter);
        }
        Object.FindObjectOfType<GameManager>().itemPlacementEnabled = true;
        Camera.main.orthographic = true; Camera.main.orthographicSize = 21;
        Camera.main.transform.rotation = Quaternion.Euler(55, 0, 0);
        Camera.main.transform.position = new Vector3(0, 0, 0) - Camera.main.transform.forward * 65;
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
        Debug.Log("Five Blender maps installed: 5, 6, 7, 8, 9 platforms, complete navigation routes.");
    }
}
