import json,time,pathlib
import run_stage1 as m
root=pathlib.Path(__file__).resolve().parents[2]
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'HoverCheck','version':'1'}})
m.rpc('resources/read',{'uri':'mcpforunity://custom-tools'})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
m.code('if(UnityEditor.EditorApplication.isPlaying||UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)throw new System.Exception("Save and stop first");UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/GameScane.unity");return true;')
try:
    m.tool('manage_editor',action='play');time.sleep(3);m.wait_ready()
    result=m.code((root/'Tools/Validation/test_hover_only.cs').read_text(encoding='utf-8'))
    (root/'Docs/Validation/hover-only-2026-10-14.json').write_text(json.dumps(result,indent=2),encoding='utf-8')
    print(json.dumps({'count':len(result),'failed':[r for r in result if not r['passed']]}))
    print(json.dumps(m.tool('read_console',action='get',types=['error','warning'],count=10,include_stacktrace=False)))
finally:
    m.tool('manage_editor',action='stop')
    m.code((root/'Tools/Validation/campaign_ready.cs').read_text(encoding='utf-8'))
