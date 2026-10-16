import json,time,pathlib
import run_stage1 as m
root=pathlib.Path(__file__).resolve().parents[2]
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'SkillDataValidation','version':'1'}})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
report={}
report['assets']=m.code('return new {skills=SkillTreeProgress.Catalog.Length,trees=Resources.LoadAll<SkillTreeData>("Data/SkillTrees").Length,errors=Resources.LoadAll<SkillTreeData>("Data/SkillTrees").Where(t=>t.ConfigurationError()!=null).Select(t=>t.ConfigurationError()).ToArray()};')
report['migration']=m.code('''string key="Skills.Test."+System.Guid.NewGuid();try{
PlayerPrefs.SetString(key,"{\\"version\\":1,\\"availablePoints\\":5,\\"unlockedSkills\\":[\\"archer.training\\",\\"archer.light\\"]}");
var p=new SkillTreeProgress(key);if(!p.IsUnlocked("archer.light")||p.AvailablePoints!=5||p.ResetTree(TowerType.ArrowTower)!=3||p.AvailablePoints!=8||p.ResetTree(TowerType.ArrowTower)!=0)throw new System.Exception("Legacy migration");
var source=Resources.LoadAll<SkillTreeData>("Data/SkillTrees").Single(t=>t.towerType==TowerType.ArrowTower);var tree=ScriptableObject.CreateInstance<SkillTreeData>();tree.towerType=TowerType.ArrowTower;var skill=UnityEngine.Object.Instantiate(source.nodes[0].skill);tree.nodes.Add(new SkillTreeData.Node{skill=skill});skill.pointCost=2;
if(!p.TryUnlock(tree,skill))throw new System.Exception("Purchase");skill.pointCost=9;if(p.ResetTree(TowerType.ArrowTower)!=2||p.AvailablePoints!=8)throw new System.Exception("Paid cost refund");
skill.prerequisites.Add(skill);if(tree.ConfigurationError()==null||p.TryUnlock(tree,skill))throw new System.Exception("Cycle validation");skill.prerequisites.Clear();skill.towerType=TowerType.MagicTower;if(tree.ConfigurationError()==null)throw new System.Exception("Wrong tower");
UnityEngine.Object.DestroyImmediate(skill);UnityEngine.Object.DestroyImmediate(tree);return "Legacy IDs/balance, historic refund, changed-cost receipt, cycle and wrong tower passed";
}finally{PlayerPrefs.DeleteKey(key);PlayerPrefs.Save();}''')
backup=m.code('return new {exists=PlayerPrefs.HasKey(SkillTreeProgress.SaveKey),value=PlayerPrefs.GetString(SkillTreeProgress.SaveKey,"")};')
try:
 m.code('PlayerPrefs.DeleteKey(SkillTreeProgress.SaveKey);PlayerPrefs.Save();UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/GameScane.unity");return true;')
 m.tool('manage_editor',action='play');time.sleep(2);m.wait_ready()
 m.code('var p=new SkillTreeProgress();for(int c=1;c<=7;c++)p.RecordResult(c,true,100,100);var go=UnityEngine.Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ArcherTower1.prefab"));go.name="ValidationArcher";return true;');time.sleep(.4)
 report['archer']=m.code((root/'Tools/Validation/skill_data_combat.cs').read_text(encoding='utf-8'))
 report['ui']=m.code('''var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.TogglePause();gm.OpenSkillTree();var ui=gm.skillTreePanel;int count=0;
for(int b=0;b<4;b++){ui.SelectBranch(b);var tree=ui.trees[b];for(int n=0;n<tree.nodes.Count;n++){ui.nodes[n].GetComponent<UnityEngine.UI.Button>().onClick.Invoke();if(ui.detailTitle.text!=tree.nodes[n].skill.displayName)throw new System.Exception("Wrong data binding");foreach(var t in ui.modal.GetComponentsInChildren<TMPro.TMP_Text>()){t.ForceMeshUpdate();if(t.isTextOverflowing)throw new System.Exception("Overflow "+t.name);}count++;}}
ui.SelectBranch(0);ui.SelectNode(7);return new {nodes=count,automaticLinks=ui.nodes[0].transform.parent.Find("DataConnections").childCount};''')
 m.tool('manage_camera',action='screenshot',output_folder='Docs/Validation',screenshot_file_name='skill-data-runtime-2026-10-15.png');time.sleep(.4)
 report['dynamicNodes']=m.code('''var ui=UnityEngine.Object.FindObjectOfType<GameManager>().skillTreePanel;var original=ui.trees[0];var tree=UnityEngine.Object.Instantiate(original);tree.nodes=new System.Collections.Generic.List<SkillTreeData.Node>(original.nodes);var a=UnityEngine.Object.Instantiate(original.nodes[0].skill);var b=UnityEngine.Object.Instantiate(a);a.id="validation.a";b.id="validation.b";a.displayName="Ek Data A";b.displayName="Ek Data B";tree.nodes.Add(new SkillTreeData.Node{skill=a,position=new Vector2(80,40)});tree.nodes.Add(new SkillTreeData.Node{skill=b,position=new Vector2(240,40)});ui.trees[0]=tree;ui.SelectBranch(0);ui.nodes[12].GetComponent<UnityEngine.UI.Button>().onClick.Invoke();if(ui.nodes.Count(n=>n.gameObject.activeSelf)!=13||ui.detailTitle.text!="Ek Data B"||ui.unlockButton.interactable)throw new System.Exception("Dynamic node or missing catalog rejection");ui.trees[0]=original;ui.SelectBranch(0);UnityEngine.Object.DestroyImmediate(a);UnityEngine.Object.DestroyImmediate(b);UnityEngine.Object.DestroyImmediate(tree);return "13th node created/bound automatically; asset outside catalog rejected";''')
 report['console']=m.tool('read_console',action='get',types=['error','warning'],count=10,include_stacktrace=False)
finally:
 m.tool('manage_editor',action='stop');time.sleep(.3)
 if backup['exists']:m.code('PlayerPrefs.SetString(SkillTreeProgress.SaveKey,'+json.dumps(backup['value'])+');PlayerPrefs.Save();return true;')
 else:m.code('PlayerPrefs.DeleteKey(SkillTreeProgress.SaveKey);PlayerPrefs.Save();return true;')
 m.code((root/'Tools/Validation/campaign_ready.cs').read_text(encoding='utf-8'))
 (root/'Docs/Validation/skill-data-2026-10-15.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps(report,ensure_ascii=False))
