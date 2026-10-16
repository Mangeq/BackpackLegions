import json,time,pathlib
import run_stage1 as m
root=pathlib.Path(__file__).resolve().parents[2]
rows=[]
def check(name,value):rows.append({'test':name,'passed':bool(value)})
def code(s):return m.code(s)
def wait(scene):
    deadline=time.monotonic()+35
    while time.monotonic()<deadline:
        try:
            if code('return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="'+scene+'" && !SceneLoadingUI.IsLoading && '+('ShopManager.Instance!=null;' if scene=='GameScane' else 'UnityEngine.Object.FindObjectOfType<MainMenuManager>()!=null;')):return
        except RuntimeError as e:
            if "'message': None" not in str(e):raise
        time.sleep(.3)
    raise TimeoutError(scene)
def shot(name):
    m.tool('manage_camera',action='screenshot',output_folder='Docs/Validation',screenshot_file_name=name+'-2026-10-14.png');time.sleep(.4)
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'ResultsSkillsValidation','version':'1'}})
m.rpc('resources/read',{'uri':'mcpforunity://custom-tools'})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
try:
    code('if(UnityEditor.EditorApplication.isPlaying||UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)throw new System.Exception("Save and stop first");UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");return true;')
    m.tool('manage_editor',action='play');time.sleep(3);wait('MainMenu')
    code('UnityEngine.Object.FindObjectOfType<MainMenuManager>().OpenSkillTree();return true;')
    check('Menu opens skill tree',code('return SkillTreePanelUI.IsOpen;'))
    for branch in range(4):
        check('Six selectable nodes branch '+str(branch),code('var ui=UnityEngine.Object.FindObjectOfType<SkillTreePanelUI>();ui.SelectBranch('+str(branch)+');bool ok=true;for(int n=0;n<6;n++){ui.SelectNode(n);ok&=ui.detailTitle.text==ui.branches['+str(branch)+'].names[n]&&ui.detailBody.text==ui.branches['+str(branch)+'].descriptions[n];ui.detailBody.ForceMeshUpdate();ok&=!ui.detailBody.isTextOverflowing;ui.nodeLabels[n].ForceMeshUpdate();ok&=!ui.nodeLabels[n].isTextOverflowing;}return ok;'))
    code('var ui=UnityEngine.Object.FindObjectOfType<SkillTreePanelUI>();ui.SelectBranch(0);ui.SelectNode(5);return true;');shot('skilltree-archer')
    code('var ui=UnityEngine.Object.FindObjectOfType<SkillTreePanelUI>();ui.SelectBranch(2);ui.SelectNode(2);return true;');shot('skilltree-mortar')
    check('Backdrop closes tree',code('var ui=UnityEngine.Object.FindObjectOfType<SkillTreePanelUI>();ui.modal.transform.Find("Backdrop").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();return !SkillTreePanelUI.IsOpen;'))
    code('var menu=UnityEngine.Object.FindObjectOfType<MainMenuManager>();menu.StartGame();menu.SelectChapter(0);menu.PlaySelectedChapter();return true;');wait('GameScane')
    check('Game starts with both results hidden',code('var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.enabled=false;return !gm.GameOverPanel.activeSelf&&!gm.victoryPanel.activeSelf;'))
    check('Gameplay cannot open skill tree without pause',code('var ui=UnityEngine.Object.FindObjectOfType<SkillTreePanelUI>();ui.Open();return !SkillTreePanelUI.IsOpen;'))
    check('Paused skill tree ESC preserves pause',code('var gm=UnityEngine.Object.FindObjectOfType<GameManager>();int gold=EconomyManager.Gold;gm.TogglePause();gm.OpenSkillTree();bool opened=SkillTreePanelUI.IsOpen;gm.HandleEscape();return opened&&!SkillTreePanelUI.IsOpen&&gm.IsPaused&&Time.timeScale==0&&EconomyManager.Gold==gold;'))
    code('UnityEngine.Object.FindObjectOfType<GameManager>().ContinueGame();GameManager.GameOver(true);return true;')
    check('Victory panel with next chapter',code('var gm=UnityEngine.Object.FindObjectOfType<GameManager>();return gm.victoryPanel.activeSelf&&!gm.GameOverPanel.activeSelf&&gm.nextChapterButton.interactable&&gm.victorySummary.text.Contains("1")&&Time.timeScale==0;'))
    shot('result-victory')
    code('UnityEngine.Object.FindObjectOfType<GameManager>().nextChapterButton.onClick.Invoke();return true;');wait('GameScane')
    check('Next button loads chapter two and its budget',code('var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.enabled=false;return UnityEngine.Object.FindObjectOfType<WaveManager>().ActiveChapter.chapterNumber==2&&EconomyManager.Gold==1000&&gm.CurrentState==GameManager.GameState.Preparation;'))
    code('GameManager.GameOver(false);return true;')
    check('Defeat separate with no next progression',code('var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.NextChapter();return gm.GameOverPanel.activeSelf&&!gm.victoryPanel.activeSelf&&ChapterData.SelectedIndex==1&&!SceneLoadingUI.IsLoading;'))
    shot('result-defeat')
    code('var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.GameOverPanel.GetComponentsInChildren<UnityEngine.UI.Button>(true).First(b=>b.name=="NewGameButton").onClick.Invoke();return true;');wait('GameScane')
    check('Defeat retry restores same chapter',code('var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.enabled=false;return UnityEngine.Object.FindObjectOfType<WaveManager>().ActiveChapter.chapterNumber==2&&!gm.GameOverPanel.activeSelf&&!gm.victoryPanel.activeSelf&&EconomyManager.Gold==1000;'))
    code('UnityEngine.Object.FindObjectOfType<GameManager>().ExitToMainMenu();return true;');wait('MainMenu')
    code('var menu=UnityEngine.Object.FindObjectOfType<MainMenuManager>();menu.StartGame();menu.SelectChapter(4);menu.PlaySelectedChapter();return true;');wait('GameScane')
    code('UnityEngine.Object.FindObjectOfType<GameManager>().enabled=false;GameManager.GameOver(true);return true;')
    check('Last chapter has no invalid next destination',code('var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.NextChapter();return !gm.nextChapterButton.interactable&&ChapterData.SelectedIndex==4&&!SceneLoadingUI.IsLoading;'))
    check('Victory controls fit',code('return UnityEngine.Object.FindObjectOfType<GameManager>().victoryPanel.GetComponentsInChildren<TMPro.TMP_Text>().All(t=>{t.ForceMeshUpdate();return !t.isTextOverflowing;});'))
    print(json.dumps({'checks':rows,'console':m.tool('read_console',action='get',types=['error','warning'],count=20,include_stacktrace=False)},ensure_ascii=False))
finally:
    m.tool('manage_editor',action='stop')
    (root/'Docs/Validation/results-skills-2026-10-14.json').write_text(json.dumps(rows,ensure_ascii=False,indent=2),encoding='utf-8')
