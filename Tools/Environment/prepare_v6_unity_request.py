import json,re
from pathlib import Path
root=Path('C:/Users/Emir/BackPackDefance')
data=json.loads((root/'Assets/Art/BlackpineV6Test/materials.json').read_text())
def f(x):return format(float(x),'.8f')+'f'
lines=[]
for i,item in enumerate(data):
    name=json.dumps(item['name']);safe=re.sub(r'[^a-zA-Z0-9_-]','_',item['name']);color=','.join(map(f,item['color']))
    line='{var path=folder+"/Materials/%02d_%s.mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(m==null){m=new Material(shader);AssetDatabase.CreateAsset(m,path);}m.SetColor("_BaseColor",new Color(%s).gamma);m.SetFloat("_Smoothness",%s);m.SetFloat("_Metallic",%s);m.SetFloat("_Cull",0);m.enableInstancing=false;'%(i,safe,color,f(min(item['smoothness'],.25)),f(item['metallic']))
    if item['grass']:line+='m.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/BlackpineEnvironment/Textures/Grass_Albedo.png"));m.SetColor("_BaseColor",new Color(.78f,.86f,.63f,1));'
    if item['emissionStrength']>0:line+='m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",new Color('+','.join(map(f,item['emission'][:3]))+')*'+f(item['emissionStrength'])+');'
    line+='EditorUtility.SetDirty(m);mats['+name+']=m;mats[m.name]=m;}'
    lines.append(line)
code='''
if(EditorApplication.isPlaying)throw new System.Exception("Stop Play Mode first");
AssetDatabase.Refresh();
string folder="Assets/Art/BlackpineV6Test";
if(!AssetDatabase.IsValidFolder(folder+"/Materials"))AssetDatabase.CreateFolder(folder,"Materials");
var shader=Shader.Find("Universal Render Pipeline/Lit");if(shader==null)throw new System.Exception("URP Lit missing");
var mats=new System.Collections.Generic.Dictionary<string,Material>();
'''+ '\n'.join(lines)+'''
var asset=AssetDatabase.LoadAssetAtPath<GameObject>(folder+"/Blackpine_V6_Map.fbx");if(asset==null)throw new System.Exception("FBX not imported");
var importer=(ModelImporter)AssetImporter.GetAtPath(folder+"/Blackpine_V6_Map.fbx");
foreach(var r in asset.GetComponentsInChildren<Renderer>())foreach(var old in r.sharedMaterials){Material mapped;if(old!=null&&mats.TryGetValue(old.name,out mapped))importer.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material),old.name),mapped);}
importer.SaveAndReimport();AssetDatabase.SaveAssets();
var backups=new System.Collections.Generic.List<string>();
for(int i=0;i<UnityEngine.SceneManagement.SceneManager.sceneCount;i++){var s=UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);if(!s.isDirty)continue;if(!AssetDatabase.IsValidFolder("Assets/Scenes/Backups"))AssetDatabase.CreateFolder("Assets/Scenes","Backups");var p=AssetDatabase.GenerateUniqueAssetPath("Assets/Scenes/Backups/"+s.name+"_Before_BlackpineV6.unity");if(!UnityEditor.SceneManagement.EditorSceneManager.SaveScene(s,p,true))throw new System.Exception("Backup failed");backups.Add(p);}
var scene=UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,UnityEditor.SceneManagement.NewSceneMode.Single);
asset=AssetDatabase.LoadAssetAtPath<GameObject>(folder+"/Blackpine_V6_Map.fbx");var map=(GameObject)PrefabUtility.InstantiatePrefab(asset);map.name="Blackpine V6 - 84x72 Forest and Castle";map.transform.rotation=Quaternion.Euler(0,180,0);
foreach(var r in map.GetComponentsInChildren<Renderer>()){var mm=r.sharedMaterials;for(int i=0;i<mm.Length;i++){Material m;if(mm[i]!=null&&mats.TryGetValue(mm[i].name,out m))mm[i]=m;}r.sharedMaterials=mm;r.gameObject.isStatic=true;}
foreach(var f in map.GetComponentsInChildren<MeshFilter>()){if(f.name.StartsWith("Terrain")||f.name.StartsWith("Road")||f.name.StartsWith("Bridge")||f.name.StartsWith("Tower_Pad_")||f.name.StartsWith("V6_Entrance_Road")){var c=f.gameObject.AddComponent<MeshCollider>();c.sharedMesh=f.sharedMesh;}}
var cameraGO=new GameObject("Main Camera");cameraGO.tag="MainCamera";var cam=cameraGO.AddComponent<Camera>();cameraGO.AddComponent<AudioListener>();cam.orthographic=true;cam.orthographicSize=44;cam.nearClipPlane=.3f;cam.farClipPlane=320;cam.transform.position=new Vector3(62,96,-83);cam.transform.LookAt(Vector3.zero);cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(.37f,.42f,.42f);
var sunGO=new GameObject("Warm clear daylight");var sun=sunGO.AddComponent<Light>();sun.type=LightType.Directional;sun.color=new Color(1,.96f,.88f);sun.intensity=1.55f;sun.shadows=LightShadows.Soft;sun.shadowBias=.05f;sunGO.transform.rotation=Quaternion.Euler(52,-35,0);RenderSettings.sun=sun;
RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;RenderSettings.ambientSkyColor=new Color(.64f,.69f,.73f);RenderSettings.ambientEquatorColor=new Color(.47f,.50f,.39f);RenderSettings.ambientGroundColor=new Color(.25f,.28f,.19f);RenderSettings.fog=false;
PrefabUtility.SaveAsPrefabAsset(map,folder+"/BlackpineV6Map.prefab");
if(SceneView.lastActiveSceneView!=null)SceneView.lastActiveSceneView.LookAt(Vector3.zero,cam.transform.rotation,70);
if(!UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene,"Assets/Scenes/Blackpine_V6_ForestCastle_Test.unity"))throw new System.Exception("Save failed");
var rr=map.GetComponentsInChildren<Renderer>();var bounds=new Bounds(Vector3.zero,Vector3.zero);foreach(var r in rr)bounds.Encapsulate(r.bounds);
return new{scene=scene.path,backups=backups.ToArray(),renderers=rr.Length,pads=rr.Count(r=>r.name.StartsWith("Tower_Pad_")),bounds=bounds.ToString(),missing=rr.Sum(r=>r.sharedMaterials.Count(m=>m==null)),wrongShader=rr.Sum(r=>r.sharedMaterials.Count(m=>m!=null&&m.shader.name!="Universal Render Pipeline/Lit"))};
'''
request=[{'kind':'call','name':'set_active_instance','arguments':{'instance':'BackPackDefance@64cdf686e603a5ef'}},{'kind':'call','name':'execute_code','arguments':{'action':'execute','code':code}}]
(root/'Tools/Environment/mcp_build_v6.json').write_text(json.dumps(request,indent=2),encoding='utf-8')
print('Prepared material and scene creation request:',len(data),'materials')
