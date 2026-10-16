import json,time,pathlib
import run_stage1 as m
root=pathlib.Path(__file__).resolve().parents[2]
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'SynergyValidation','version':'1'}})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
backup=m.code('return new {exists=PlayerPrefs.HasKey(SkillTreeProgress.SaveKey),value=PlayerPrefs.GetString(SkillTreeProgress.SaveKey,"")};')
report={}
try:
 m.code('PlayerPrefs.DeleteKey(SkillTreeProgress.SaveKey);PlayerPrefs.Save();UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/GameScane.unity");return true;')
 m.tool('manage_editor',action='play');time.sleep(2);m.wait_ready()
 report['runtime']=m.code((root/'Tools/Validation/test_synergy_runtime.cs').read_text(encoding='utf-8'))
 report['console']=m.tool('read_console',action='get',types=['error','warning'],count=10,include_stacktrace=False)
finally:
 m.tool('manage_editor',action='stop');time.sleep(.3)
 if backup['exists']:m.code('PlayerPrefs.SetString(SkillTreeProgress.SaveKey,'+json.dumps(backup['value'])+');PlayerPrefs.Save();return true;')
 else:m.code('PlayerPrefs.DeleteKey(SkillTreeProgress.SaveKey);PlayerPrefs.Save();return true;')
 m.code((root/'Tools/Validation/campaign_ready.cs').read_text(encoding='utf-8'))
 (root/'Docs/Validation/synergy-runtime-2026-10-15.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps(report,ensure_ascii=False))
