import json,time,pathlib
import run_stage1 as m
root=pathlib.Path(__file__).resolve().parents[2]
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'ArcherSkillValidation','version':'1'}})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
report={}
report['economy']=m.code('''var key="BackpackLegions.Validation."+System.Guid.NewGuid();try{
var p=new SkillTreeProgress(key);if(p.TryUnlockArcher(0)||p.TryUnlockArcher(10))throw new System.Exception("Empty wallet");
for(int c=1;c<=5;c++)p.RecordResult(c,true,100,100);
if(p.TryUnlockArcher(1))throw new System.Exception("Prerequisite");
if(!p.TryUnlockArcher(0)||p.TryUnlockArcher(0))throw new System.Exception("Duplicate");
var stale=new SkillTreeProgress(key);if(!p.TryUnlockArcher(1)||!stale.TryUnlockArcher(2))throw new System.Exception("Branch choice");
p=new SkillTreeProgress(key);if(p.AvailablePoints!=10||!p.IsUnlocked("archer.distance"))throw new System.Exception("Persisted cost");
if(p.ResetArcher()!=5||p.ResetArcher()!=0||p.AvailablePoints!=15||p.BestPoints(1)!=3)throw new System.Exception("Refund");
if(SkillTreeProgress.ArcherSkills.Length!=11)throw new System.Exception("Extra arrow retained");return "Funds, parents, duplicate prevention, stale-instance reload, persistence, refund and record preservation passed";
}finally{PlayerPrefs.DeleteKey(key);PlayerPrefs.Save();}''')
backup=m.code('return new {exists=PlayerPrefs.HasKey(SkillTreeProgress.SaveKey),value=PlayerPrefs.GetString(SkillTreeProgress.SaveKey,"")};')
try:
 m.code('PlayerPrefs.DeleteKey(SkillTreeProgress.SaveKey);PlayerPrefs.Save();UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/GameScane.unity");return true;')
 m.tool('manage_editor',action='play');time.sleep(2);m.wait_ready()
 m.code('var p=new SkillTreeProgress();for(int c=1;c<=7;c++)p.RecordResult(c,true,100,100);var go=UnityEngine.Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ArcherTower1.prefab"));go.name="ValidationArcher";return true;')
 time.sleep(.4)
 report['combat']=m.code((root/'Tools/Validation/archer_skill_combat.cs').read_text(encoding='utf-8'))
 report['ui']=m.code('''var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.TogglePause();gm.OpenSkillTree();var ui=gm.skillTreePanel;int count=0;
for(int b=0;b<4;b++){ui.SelectBranch(b);int length=b==0?11:12;for(int n=0;n<length;n++){ui.nodes[n].GetComponent<UnityEngine.UI.Button>().onClick.Invoke();foreach(var t in ui.modal.GetComponentsInChildren<TMPro.TMP_Text>()){t.ForceMeshUpdate();if(t.isTextOverflowing)throw new System.Exception("Overflow "+t.name+" branch "+b+" node "+n);}count++;}if(b!=0&&ui.unlockButton.interactable)throw new System.Exception("Draft unlock");}
ui.SelectBranch(0);ui.SelectNode(10);return new {selections=count,balance=new SkillTreeProgress().AvailablePoints};''')
 m.tool('manage_camera',action='screenshot',output_folder='Docs/Validation',screenshot_file_name='archer-skills-active-2026-10-14.png');time.sleep(.3)
 report['refundLive']=m.code('''var gm=UnityEngine.Object.FindObjectOfType<GameManager>();var ui=gm.skillTreePanel;ui.resetButton.onClick.Invoke();var p=new SkillTreeProgress();if(p.AvailablePoints!=21||p.IsUnlocked("archer.training"))throw new System.Exception("UI refund");ui.SelectNode(0);if(!ui.unlockButton.interactable)throw new System.Exception("UI funds");ui.unlockButton.onClick.Invoke();if(new SkillTreeProgress().AvailablePoints!=20)throw new System.Exception("UI purchase");ui.unlockButton.onClick.Invoke();if(new SkillTreeProgress().AvailablePoints!=20)throw new System.Exception("UI duplicate");ui.resetButton.onClick.Invoke();gm.HandleEscape();if(SkillTreePanelUI.IsOpen||!gm.IsPaused)throw new System.Exception("Pause close");return "Refund, purchase, duplicate and pause return passed";''')
 report['console']=m.tool('read_console',action='get',types=['error','warning'],count=10,include_stacktrace=False)
finally:
 m.tool('manage_editor',action='stop');time.sleep(.3)
 if backup['exists']:m.code('PlayerPrefs.SetString(SkillTreeProgress.SaveKey,'+json.dumps(backup['value'])+');PlayerPrefs.Save();return true;')
 else:m.code('PlayerPrefs.DeleteKey(SkillTreeProgress.SaveKey);PlayerPrefs.Save();return true;')
 m.code((root/'Tools/Validation/campaign_ready.cs').read_text(encoding='utf-8'))
 (root/'Docs/Validation/archer-skills-2026-10-14.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps(report,ensure_ascii=False))
