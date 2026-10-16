import json,time,pathlib
import run_stage1 as m
root=pathlib.Path(__file__).resolve().parents[2]
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'FinalSkillPreview','version':'1'}})
m.rpc('resources/read',{'uri':'mcpforunity://custom-tools'})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
try:
    m.code('if(UnityEditor.EditorApplication.isPlaying||UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)throw new System.Exception("Save and stop first");UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");return true;')
    m.tool('manage_editor',action='play');time.sleep(3)
    m.code('UnityEngine.Object.FindObjectOfType<MainMenuManager>().OpenSkillTree();var ui=UnityEngine.Object.FindObjectOfType<SkillTreePanelUI>();ui.SelectBranch(0);ui.SelectNode(5);return ui.branches.All(b=>b.icons.All(i=>i!=null));')
    m.tool('manage_camera',action='screenshot',output_folder='Docs/Validation',screenshot_file_name='skilltree-final-2026-10-14.png');time.sleep(.4)
    m.code('var ui=UnityEngine.Object.FindObjectOfType<SkillTreePanelUI>();ui.SelectBranch(2);ui.SelectNode(5);return true;')
    m.tool('manage_camera',action='screenshot',output_folder='Docs/Validation',screenshot_file_name='skilltree-havan-final-2026-10-14.png');time.sleep(.4)
    print(json.dumps(m.tool('read_console',action='get',types=['error','warning'],count=10,include_stacktrace=False)))
finally:
    m.tool('manage_editor',action='stop')
    m.code((root/'Tools/Validation/campaign_ready.cs').read_text(encoding='utf-8'))
