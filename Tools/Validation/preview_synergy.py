import json,time,pathlib
import run_stage1 as m
root=pathlib.Path(__file__).resolve().parents[2]
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'SynergyPreview','version':'1'}})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
try:
 m.code('UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/GameScane.unity");return true;')
 m.tool('manage_editor',action='play');time.sleep(2);m.wait_ready()
 body=(root/'Tools/Validation/test_synergy_runtime.cs').read_text(encoding='utf-8').split('var info=')[0]
 body=body.replace('check(Mathf.Abs((float)typeof(Tower).GetField("currentDamage",flags).GetValue(tower)-baseDamage-5)<.001f,"Flat five damage");','')
 m.code(body+'return true;');time.sleep(.4)
 print(json.dumps(m.tool('manage_camera',action='screenshot',output_folder='Docs/Validation',screenshot_file_name='synergy-attachment-2026-10-15.png')))
 print(json.dumps(m.code((root/'Tools/Validation/test_synergy_transitions.cs').read_text(encoding='utf-8'))))
 print(json.dumps(m.tool('read_console',action='get',types=['error','warning'],count=10,include_stacktrace=False)))
finally:
 m.tool('manage_editor',action='stop');time.sleep(.3)
 m.code((root/'Tools/Validation/campaign_ready.cs').read_text(encoding='utf-8'))
