import json,time,pathlib
import run_stage1 as m
root=pathlib.Path(__file__).resolve().parents[2]
report=[]
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'HardBalanceFlow','version':'1'}})
m.rpc('resources/read',{'uri':'mcpforunity://custom-tools'})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
def wait(scene):
    deadline=time.monotonic()+30
    while time.monotonic()<deadline:
        try:
            if m.code('return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="'+scene+'" && !SceneLoadingUI.IsLoading && '+('ShopManager.Instance!=null;' if scene=='GameScane' else 'UnityEngine.Object.FindObjectOfType<MainMenuManager>()!=null;')):return
        except RuntimeError as e:
            if "'message': None" not in str(e):raise
        time.sleep(.3)
    raise TimeoutError(m.code('return new{scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,frames=Time.frameCount,shop=ShopManager.Instance!=null,paused=UnityEditor.EditorApplication.isPaused};'))
try:
    m.code('if(UnityEditor.EditorApplication.isPlaying||UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)throw new System.Exception("Unsafe test start");UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");return true;')
    m.tool('manage_editor',action='play');time.sleep(2);wait('MainMenu')
    for c in range(5):
        m.code('EconomyManager.Gold=99999;var menu=UnityEngine.Object.FindObjectOfType<MainMenuManager>();menu.StartGame();menu.SelectChapter('+str(c)+');menu.PlaySelectedChapter();return true;')
        wait('GameScane')
        state=m.code('var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.enabled=false;return new{gold=EconomyManager.Gold,chapter=UnityEngine.Object.FindObjectOfType<WaveManager>().ActiveChapter.chapterNumber};')
        report.append({'test':'Chapter '+str(c+1)+' replaces previous gold','passed':state['gold']==700+150*c and state['chapter']==c+1})
        if c<4:
            m.code('UnityEngine.Object.FindObjectOfType<GameManager>().ExitToMainMenu();return true;');wait('MainMenu')
    m.code('EconomyManager.Gold=1;UnityEngine.Object.FindObjectOfType<GameManager>().RestartGame();return true;')
    time.sleep(1);wait('GameScane')
    report.append({'test':'Restart restores selected chapter budget','passed':m.code('return EconomyManager.Gold==1300 && UnityEngine.Object.FindObjectOfType<WaveManager>().ActiveChapter.chapterNumber==5;')})
    m.code('var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.enabled=false;var wm=UnityEngine.Object.FindObjectOfType<WaveManager>();typeof(WaveManager).GetField("currentWaveIndex",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(wm,wm.waves.Count-1);UİManager.UpdateWaveText(wm.waves.Count,wm.waves.Count);return true;')
    time.sleep(.3)
    report.append({'test':'Preview shows boss warning and actual multipliers','passed':m.code('var p=UnityEngine.Object.FindObjectOfType<WavePreviewUI>();return p.details.text.Contains("BOSS") && p.details.text.Contains("×2") && p.details.text.Contains("10200");')})
    report.append({'test':'Preview text fits panel','passed':m.code('var p=UnityEngine.Object.FindObjectOfType<WavePreviewUI>();p.details.ForceMeshUpdate();return !p.details.isTextOverflowing;')})
    m.tool('manage_camera',action='screenshot',output_folder='Docs/Validation',screenshot_file_name='hard-balance-preview-2026-10-13.png')
    time.sleep(.4)
finally:
    m.tool('manage_editor',action='stop')
    (root/'Docs/Validation/hard-balance-flow-2026-10-13.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps(report,ensure_ascii=False,indent=2))
