import json,time,pathlib
import run_stage1 as m
root=pathlib.Path(__file__).resolve().parents[2]
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'ArcherFinalUI','version':'1'}})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
try:
 m.code('UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");return true;')
 m.tool('manage_editor',action='play');time.sleep(3)
 result=m.code('''UnityEngine.Object.FindObjectOfType<MainMenuManager>().OpenSkillTree();var ui=UnityEngine.Object.FindObjectOfType<SkillTreePanelUI>();ui.SelectBranch(0);ui.SelectNode(7);Canvas.ForceUpdateCanvases();var text=ui.resetButton.GetComponentInChildren<TMPro.TMP_Text>();text.ForceMeshUpdate();if(text.isTextOverflowing||text.rectTransform.rect.width>ui.resetButton.GetComponent<RectTransform>().rect.width)throw new System.Exception("Reset label overflow");return new {nodes=ui.nodes.Count(n=>n.gameObject.activeSelf),labelWidth=text.rectTransform.rect.width,buttonWidth=ui.resetButton.GetComponent<RectTransform>().rect.width};''')
 m.tool('manage_camera',action='screenshot',output_folder='Docs/Validation',screenshot_file_name='archer-skills-final-2026-10-15.png');time.sleep(.5)
 print(json.dumps({'ui':result,'console':m.tool('read_console',action='get',types=['error','warning'],count=10,include_stacktrace=False)}))
finally:
 m.tool('manage_editor',action='stop');time.sleep(.3)
 m.code((root/'Tools/Validation/campaign_ready.cs').read_text(encoding='utf-8'))
