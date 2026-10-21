var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();var map=scene.GetRootGameObjects().First(g=>g.name.StartsWith("Blackpine"));
var sources=new System.Collections.Generic.List<UnityEngine.AI.NavMeshBuildSource>();foreach(var name in new[]{"Road","Bridge"}){var t=map.transform.Find(name);sources.Add(new UnityEngine.AI.NavMeshBuildSource{shape=UnityEngine.AI.NavMeshBuildSourceShape.Mesh,sourceObject=t.GetComponent<MeshFilter>().sharedMesh,transform=t.localToWorldMatrix,area=0});}
var settings=UnityEngine.AI.NavMesh.GetSettingsByID(-1372625422);settings.overrideVoxelSize=true;settings.voxelSize=.06f;settings.agentClimb=.4f;
var data=UnityEngine.AI.NavMeshBuilder.BuildNavMeshData(settings,sources,new Bounds(new Vector3(6,2,0),new Vector3(90,15,70)),Vector3.zero,Quaternion.identity);
if(data==null)throw new System.Exception("NavMesh bake failed");
UnityEditor.AssetDatabase.CreateAsset(data,"Assets/Prefabs/NewMap/PlayableMapNavMesh.asset");
foreach(var old in map.GetComponentsInChildren<Unity.AI.Navigation.NavMeshSurface>())old.enabled=false;
var surface=map.transform.Find("NewMap Gameplay").gameObject.AddComponent<Unity.AI.Navigation.NavMeshSurface>();surface.agentTypeID=-1372625422;surface.navMeshData=data;surface.AddData();
var filter=new UnityEngine.AI.NavMeshQueryFilter{agentTypeID=-1372625422,areaMask=-1};var path=new UnityEngine.AI.NavMeshPath();var spawn=UnityEngine.Object.FindObjectOfType<WaveManager>().spawnPoint;var goal=UnityEngine.Object.FindObjectOfType<Base>().transform;
UnityEngine.AI.NavMeshHit a,b;UnityEngine.AI.NavMesh.SamplePosition(spawn.position,out a,2,filter);UnityEngine.AI.NavMesh.SamplePosition(goal.position,out b,2,filter);spawn.position=a.position;goal.position=b.position;
UnityEngine.AI.NavMesh.CalculatePath(spawn.position,goal.position,filter,path);
var prefab=UnityEditor.PrefabUtility.LoadPrefabContents("Assets/Prefabs/NewMap/MagicTower.prefab");try{var tower=prefab.GetComponent<Tower>();var copy=UnityEngine.Object.Instantiate(tower.data);copy.name="NewMap MagicTower Data";copy.range=5.2f;UnityEditor.AssetDatabase.CreateAsset(copy,"Assets/Prefabs/NewMap/MagicTowerData.asset");tower.data=copy;UnityEditor.PrefabUtility.SaveAsPrefabAsset(prefab,"Assets/Prefabs/NewMap/MagicTower.prefab");}finally{UnityEditor.PrefabUtility.UnloadPrefabContents(prefab);}
Camera.main.transform.rotation=Quaternion.Euler(55,0,0);Camera.main.transform.position=new Vector3(5,0,1)-Camera.main.transform.forward*65;Camera.main.orthographicSize=22;
UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);UnityEditor.AssetDatabase.SaveAssets();return new{path=path.status.ToString(),corners=path.corners.Select(p=>p.ToString()).ToArray()};
