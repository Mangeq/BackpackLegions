import json,time,pathlib
import run_stage1 as m
root=pathlib.Path(__file__).resolve().parents[2]
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'SkillProgressChecks','version':'1'}})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
report={}
report['rewardRules']=m.code('''string key="BackpackLegions.Validation.SkillTree."+System.Guid.NewGuid().ToString();try{
int[] hp={0,1,49,50,89,90,100};int[] expected={0,1,1,2,2,3,3};
for(int i=0;i<hp.Length;i++)if(SkillTreeProgress.PointsForResult(true,hp[i],100)!=expected[i])throw new System.Exception("Threshold "+hp[i]);
if(SkillTreeProgress.PointsForResult(false,100,100)!=0||SkillTreeProgress.PointsForResult(true,1,0)!=0||SkillTreeProgress.PointsForResult(true,9,10)!=3)throw new System.Exception("Invalid outcome");
var p=new SkillTreeProgress(key);if(p.RecordResult(1,true,25,100)!=1)throw new System.Exception("Initial");
p=new SkillTreeProgress(key);if(p.AvailablePoints!=1||p.RecordResult(1,true,90,100)!=2)throw new System.Exception("Reload delta");
if(p.RecordResult(1,true,100,100)!=0||p.RecordResult(1,true,1,100)!=0||p.RecordResult(1,false,0,100)!=0)throw new System.Exception("Repeat");
if(p.RecordResult(2,true,50,100)!=2||p.RecordResult(0,true,100,100)!=0)throw new System.Exception("Chapter isolation");
p=new SkillTreeProgress(key);if(p.AvailablePoints!=5||p.BestPoints(1)!=3||p.BestPoints(2)!=2)throw new System.Exception("Persistence");return "Thresholds, replay, improvement delta, defeat, independent chapters and persisted reload passed";
}finally{PlayerPrefs.DeleteKey(key);PlayerPrefs.Save();}''')
backup=m.code('return new {exists=PlayerPrefs.HasKey(SkillTreeProgress.SaveKey),value=PlayerPrefs.GetString(SkillTreeProgress.SaveKey,"")};')
try:
 m.code('PlayerPrefs.DeleteKey(SkillTreeProgress.SaveKey);PlayerPrefs.Save();return true;')
 for hp in [25,90,0]:
  m.code('if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Already playing");UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/GameScane.unity");return true;')
  m.tool('manage_editor',action='play');time.sleep(2);m.wait_ready()
  report['result_'+str(hp)]=m.code('''var gm=UnityEngine.Object.FindObjectOfType<GameManager>();var castle=GameObject.FindGameObjectWithTag("Base").GetComponent<Base>();castle.currentHealth='''+str(hp)+''';GameManager.GameOver('''+('true' if hp else 'false')+''');
var before=new SkillTreeProgress().AvailablePoints;GameManager.GameOver(true);if(new SkillTreeProgress().AvailablePoints!=before)throw new System.Exception("Duplicate award");
var panel=gm.CurrentState==GameManager.GameState.Victory?gm.victoryPanel:gm.GameOverPanel;
foreach(var t in panel.GetComponentsInChildren<TMPro.TMP_Text>()){t.ForceMeshUpdate();if(t.isTextOverflowing)throw new System.Exception("Result overflow "+t.name);}
panel.transform.Find("Card/SkillTree").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();if(!SkillTreePanelUI.IsOpen)throw new System.Exception("Result access");
var ui=gm.skillTreePanel;int count=0;
for(int b=0;b<4;b++){ui.SelectBranch(b);if(ui.nodes.Length!=12)throw new System.Exception("Node count");for(int n=0;n<12;n++){ui.nodes[n].GetComponent<UnityEngine.UI.Button>().onClick.Invoke();if(ui.detailTitle.text!=ui.branches[b].names[n])throw new System.Exception("Wrong node");foreach(var t in ui.modal.GetComponentsInChildren<TMPro.TMP_Text>()){t.ForceMeshUpdate();if(t.isTextOverflowing)throw new System.Exception("Skill overflow "+t.name);}count++;}}
gm.HandleEscape();if(SkillTreePanelUI.IsOpen||!panel.activeSelf||Time.timeScale!=0)throw new System.Exception("Return to result");
return new {points=before,nodeSelections=count,summary=(gm.CurrentState==GameManager.GameState.Victory?gm.victorySummary:gm.defeatSummary).text};''')
  if hp==90:
   m.tool('manage_camera',action='screenshot',output_folder='Docs/Validation',screenshot_file_name='skill-points-victory-2026-10-14.png');time.sleep(.3)
   m.code('var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.OpenSkillTree();gm.skillTreePanel.SelectBranch(0);gm.skillTreePanel.SelectNode(11);return true;')
   m.tool('manage_camera',action='screenshot',output_folder='Docs/Validation',screenshot_file_name='skilltree-12-nodes-2026-10-14.png');time.sleep(.3)
  m.tool('manage_editor',action='stop');time.sleep(.5)
 report['console']=m.tool('read_console',action='get',types=['error','warning'],count=10,include_stacktrace=False)
finally:
 m.tool('manage_editor',action='stop');time.sleep(.3)
 if backup['exists']:m.code('PlayerPrefs.SetString(SkillTreeProgress.SaveKey,'+json.dumps(backup['value'])+');PlayerPrefs.Save();return true;')
 else:m.code('PlayerPrefs.DeleteKey(SkillTreeProgress.SaveKey);PlayerPrefs.Save();return true;')
 m.code((root/'Tools/Validation/campaign_ready.cs').read_text(encoding='utf-8'))
 (root/'Docs/Validation/skill-progress-2026-10-14.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps(report,ensure_ascii=False))
