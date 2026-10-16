import json,time,pathlib
import run_stage1 as m
root=pathlib.Path(__file__).resolve().parents[2]
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'BalanceMenuCheck','version':'1'}})
m.rpc('resources/read',{'uri':'mcpforunity://custom-tools'})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
m.code('if(UnityEditor.EditorApplication.isPlaying||UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)throw new System.Exception("Save and stop first");UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");return true;')
try:
    m.tool('manage_editor',action='play');time.sleep(3)
    result=m.code('var menu=UnityEngine.Object.FindObjectOfType<MainMenuManager>();menu.StartGame();var field=typeof(MainMenuManager).GetField("chapterDescription",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);var rows=new System.Collections.Generic.List<object>();for(int i=0;i<5;i++){menu.SelectChapter(i);var text=(TMPro.TMP_Text)field.GetValue(menu);text.ForceMeshUpdate();rows.Add(new{chapter=i+1,passed=!text.isTextOverflowing&&text.text.Contains((700+150*i).ToString())});}return rows;')
    (root/'Docs/Validation/hard-balance-menu-2026-10-14.json').write_text(json.dumps(result,indent=2),encoding='utf-8')
    print(json.dumps(result))
finally:
    m.tool('manage_editor',action='stop')
    m.code((root/'Tools/Validation/campaign_ready.cs').read_text(encoding='utf-8'))
