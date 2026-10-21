var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
if(scene.path!="Assets/Scenes/GameScane.unity"||UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Expected edit-mode GameScane");
UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene,"Docs/Validation/new-map/before-integration.unity",true);
var map=scene.GetRootGameObjects().First(g=>g.name.StartsWith("Blackpine"));
var backup=scene.GetRootGameObjects().Where(g=>g.name=="PlaneTest"||g.name=="PlaneTestMVP").Select(g=>new{name=g.name,active=g.activeSelf,json=UnityEditor.EditorJsonUtility.ToJson(g)}).ToArray();
var gameplay=new GameObject("NewMap Gameplay");gameplay.transform.SetParent(map.transform,false);
foreach(var pad in map.GetComponentsInChildren<MeshRenderer>().Where(r=>r.name.StartsWith("Tower_Pad_")).ToArray()){
 var oldSlot=pad.GetComponent<TowerSlot>();if(oldSlot!=null)UnityEngine.Object.DestroyImmediate(oldSlot);
 var button=pad.GetComponent<UnityEngine.UI.Button>();if(button!=null)UnityEngine.Object.DestroyImmediate(button);
 foreach(var c in pad.GetComponents<Collider>())c.enabled=false;
 var slot=new GameObject(pad.name.Substring(0,12)+" Slot");slot.transform.SetParent(gameplay.transform);slot.transform.position=new Vector3(pad.bounds.center.x,pad.bounds.max.y,pad.bounds.center.z);
 var col=slot.AddComponent<BoxCollider>();col.size=new Vector3(3.25f,.35f,3.25f);col.center=new Vector3(0,-.08f,0);slot.AddComponent<TowerSlot>();
}
var filter=new UnityEngine.AI.NavMeshQueryFilter{agentTypeID=-1372625422,areaMask=-1};UnityEngine.AI.NavMeshHit spawnHit,baseHit;
var spawn=map.transform.Find("EnemySpawner");UnityEngine.AI.NavMesh.SamplePosition(spawn.position,out spawnHit,5,filter);spawn.position=spawnHit.position;
var baseBounds=map.transform.Find("V4_Compact_Dark_Base").GetComponent<Renderer>().bounds;
UnityEngine.AI.NavMesh.SamplePosition(baseBounds.center,out baseHit,8,filter);
var home=new GameObject("NewMap Base");home.tag="Base";home.transform.SetParent(gameplay.transform);home.transform.position=baseHit.position;home.AddComponent<Base>().maxHealth=100;
var trigger=home.AddComponent<BoxCollider>();trigger.isTrigger=true;trigger.center=Vector3.up;trigger.size=new Vector3(3,3,3);var rb=home.AddComponent<Rigidbody>();rb.isKinematic=true;rb.useGravity=false;
UnityEngine.Object.FindObjectOfType<WaveManager>().spawnPoint=spawn;
if(!UnityEditor.AssetDatabase.IsValidFolder("Assets/Prefabs/NewMap"))UnityEditor.AssetDatabase.CreateFolder("Assets/Prefabs","NewMap");
var ui=UnityEngine.Object.FindObjectOfType<UİManager>();
var sources=new[]{"Assets/Prefabs/ArcherTower1.prefab","Assets/Prefabs/MegicTower1.prefab","Assets/Prefabs/UnitTower1.prefab"};var models=new[]{"Assets/ArcherTower.fbx","Assets/MagicV1.fbx","Assets/UnityTower.fbx"};var names=new[]{"ArcherTower","MagicTower","UnitTower"};
var results=new System.Collections.Generic.List<object>();
for(int i=0;i<3;i++){
 var root=UnityEditor.PrefabUtility.LoadPrefabContents(sources[i]);
 try{
  root.name=names[i];root.transform.localScale=Vector3.one;
  foreach(var r in root.GetComponentsInChildren<Renderer>(true))r.enabled=false;
  var visual=UnityEngine.Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(models[i]),root.transform);visual.name="Visual";visual.transform.localPosition=Vector3.zero;visual.transform.localRotation=Quaternion.identity;visual.transform.localScale=Vector3.one;
  var bounds=visual.GetComponentsInChildren<Renderer>().First().bounds;foreach(var r in visual.GetComponentsInChildren<Renderer>())bounds.Encapsulate(r.bounds);
  float scale=2.8f/Mathf.Max(bounds.size.x,bounds.size.z);visual.transform.localScale=Vector3.one*scale;visual.transform.localPosition=new Vector3(-bounds.center.x*scale,-bounds.min.y*scale,-bounds.center.z*scale);
  var box=root.GetComponent<BoxCollider>();box.center=new Vector3(0,bounds.size.y*scale*.5f,0);box.size=new Vector3(bounds.size.x*scale,bounds.size.y*scale,bounds.size.z*scale);
  var prefab=UnityEditor.PrefabUtility.SaveAsPrefabAsset(root,"Assets/Prefabs/NewMap/"+names[i]+".prefab");
  if(i==0)ui.ArcherTowerPrefab=prefab;else if(i==1)ui.MegicTowerPrefab=prefab;else ui.UnitTowerPrefab=prefab;
  results.Add(new{name=names[i],scale,height=box.size.y});
 }finally{UnityEditor.PrefabUtility.UnloadPrefabContents(root);}
}
// Keep manually placed model previews available, but outside gameplay.
foreach(var g in scene.GetRootGameObjects().Where(g=>g.name=="MagicV1"||g.name=="ArcherTower"))g.SetActive(false);
var camera=Camera.main;var focus=new Vector3(5,0,1);camera.transform.position=focus-camera.transform.forward*65;camera.orthographicSize=25;
UnityEditor.EditorUtility.SetDirty(ui);UnityEditor.EditorUtility.SetDirty(UnityEngine.Object.FindObjectOfType<WaveManager>());UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);UnityEditor.AssetDatabase.SaveAssets();
return new{slots=UnityEngine.Object.FindObjectsOfType<TowerSlot>().Length,spawn=spawn.position.ToString(),goal=home.transform.position.ToString(),prefabs=results,backups=backup};

