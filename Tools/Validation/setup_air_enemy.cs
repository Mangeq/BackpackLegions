if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Stop Play");
var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
// Kullanıcının mevcut kayıtlı olmayan sahne düzenlemelerini de geri alınabilir bir kopyada tut.
UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene,"Backups/2026-10-08-air-enemy/BeforeAirEnemy.unity",true);
var wm=UnityEngine.Object.FindObjectOfType<WaveManager>();if(wm.waves.Count<3)throw new System.Exception("Third wave missing");
var source=wm.waves[2].enemyPrefab;var sourceEnemy=source.GetComponent<Enemy>();
string dataPath="Assets/Resources/Data/EnemyData/Chapter01/SkyWisp.asset";var data=UnityEditor.AssetDatabase.LoadAssetAtPath<EnemyData>(dataPath);
if(data==null){data=UnityEngine.Object.Instantiate(sourceEnemy.enemyData);UnityEditor.AssetDatabase.CreateAsset(data,dataPath);}data.enemyName="Gök Perisi";data.movementType=EnemyData.MovementType.Air;data.flightHeight=1f;UnityEditor.EditorUtility.SetDirty(data);
string matPath="Assets/Prefabs/Chapter01/SkyWisp.mat";var mat=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Material>(matPath);if(mat==null){mat=new UnityEngine.Material(UnityEngine.Shader.Find("Universal Render Pipeline/Lit"));UnityEditor.AssetDatabase.CreateAsset(mat,matPath);}mat.SetColor("_BaseColor",new UnityEngine.Color(.25f,.7f,.95f));mat.SetFloat("_Smoothness",.35f);UnityEditor.EditorUtility.SetDirty(mat);
var root=UnityEngine.Object.Instantiate(source);root.name="SkyWisp";
try{
    var oldRenderer=root.GetComponent<UnityEngine.MeshRenderer>();if(oldRenderer!=null)UnityEngine.Object.DestroyImmediate(oldRenderer);var oldMesh=root.GetComponent<UnityEngine.MeshFilter>();if(oldMesh!=null)UnityEngine.Object.DestroyImmediate(oldMesh);
    var visual=new UnityEngine.GameObject("FlightVisual");visual.transform.SetParent(root.transform,false);
    // Basit, belirgin hava silueti: küre gövde ve iki kanat. Dünya boyutu root ölçeğine göre ayarlanır.
    var scale=root.transform.lossyScale;var inverse=new UnityEngine.Vector3(1/scale.x,1/scale.y,1/scale.z);visual.transform.localScale=inverse;
    System.Action<string,UnityEngine.Vector3,UnityEngine.Vector3,float> part=(name,pos,size,angle)=>{var p=UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Sphere);p.name=name;p.transform.SetParent(visual.transform,false);p.transform.localPosition=pos;p.transform.localScale=size;p.transform.localRotation=UnityEngine.Quaternion.Euler(0,0,angle);UnityEngine.Object.DestroyImmediate(p.GetComponent<UnityEngine.Collider>());p.GetComponent<UnityEngine.Renderer>().sharedMaterial=mat;};
    part("Body",UnityEngine.Vector3.zero,new UnityEngine.Vector3(.35f,.45f,.35f),0);part("LeftWing",new UnityEngine.Vector3(-.32f,.08f,0),new UnityEngine.Vector3(.62f,.09f,.28f),-20);part("RightWing",new UnityEngine.Vector3(.32f,.08f,0),new UnityEngine.Vector3(.62f,.09f,.28f),20);
    var hover=visual.AddComponent<UnityEngine.BoxCollider>();hover.size=new UnityEngine.Vector3(1.15f,.5f,.4f);hover.isTrigger=true;visual.tag="Untagged";
    var enemy=root.GetComponent<Enemy>();enemy.enemyData=data;enemy.visualRoot=visual.transform;
    root.GetComponent<UnityEngine.Collider>().isTrigger=true;var rb=root.GetComponent<UnityEngine.Rigidbody>();if(rb!=null){rb.isKinematic=true;rb.useGravity=false;}
    var agent=root.GetComponent<UnityEngine.AI.NavMeshAgent>();agent.obstacleAvoidanceType=UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;agent.radius=.05f;
    var prefab=UnityEditor.PrefabUtility.SaveAsPrefabAsset(root,"Assets/Prefabs/Chapter01/SkyWisp.prefab");wm.waves[2].enemyPrefab=prefab;
}finally{UnityEngine.Object.DestroyImmediate(root);}
UnityEditor.AssetDatabase.SaveAssets();UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
return new{name=data.enemyName,health=data.health,speed=data.speed,damage=data.damage,reward=data.goldReward,count=wm.waves[2].enemyCount,interval=wm.waves[2].spawnInterval};
