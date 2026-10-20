"""Check shipped font atlases and the real menu/runtime-created interfaces."""
import json,time
from pathlib import Path
import run_stage1 as m
out=Path(__file__).resolve().parents[2]/'Docs/Validation/game-fonts';rows=[];overflows={}
def code(s):return m.code(s)
def check(name,ok):
    rows.append({'test':name,'passed':bool(ok)});print(name,bool(ok),flush=True)
    if not ok:raise AssertionError(name)
def wait(s):
    end=time.monotonic()+35
    while time.monotonic()<end:
        try:
            if code(s):return
        except RuntimeError:pass
        time.sleep(.4)
    raise TimeoutError(s)
def capture(name):
    code('var t=typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView");var v=UnityEditor.EditorWindow.GetWindow(t);v.Focus();v.Repaint();Canvas.ForceUpdateCanvases();return true;')
    time.sleep(.3)
    m.tool('manage_camera',action='screenshot',include_image=False,output_folder='Docs/Validation/game-fonts',screenshot_file_name=name+'.png')
    overflows[name]=code('var result=new System.Collections.Generic.List<object>();foreach(var t in UnityEngine.Object.FindObjectsOfType<TMPro.TMP_Text>()){if(!t.enabled)continue;t.ForceMeshUpdate();if(t.isTextOverflowing)result.Add(new{name=t.name,text=t.text,font=t.font.name});}return result;')

m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'GameFontsCheck','version':'1'}})
m.rpc('resources/read',{'uri':'mcpforunity://custom-tools'})
before=code('return new{playing=EditorApplication.isPlaying,path=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path,start=AssetDatabase.GetAssetPath(UnityEditor.SceneManagement.EditorSceneManager.playModeStartScene)};')
if before['playing']:raise RuntimeError('Exit Play first')
try:
    check('both static atlases include Turkish and numeric glyphs',code('return GameFonts.Title.HasCharacters("ÇĞİÖŞÜçğıöşü0123456789")&&GameFonts.Body.HasCharacters("ÇĞİÖŞÜçğıöşü0123456789")&&GameFonts.Title.atlasPopulationMode==TMPro.AtlasPopulationMode.Static&&GameFonts.Body.atlasPopulationMode==TMPro.AtlasPopulationMode.Static;'))
    code('UnityEditor.SceneManagement.EditorSceneManager.playModeStartScene=AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Boot.unity");return true;');m.tool('manage_editor',action='play')
    wait('return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="MainMenu"&&!SceneLoadingUI.IsLoading&&UnityEngine.Object.FindObjectOfType<MainMenuManager>()!=null;')
    check('menu TMP components use the two new shared fonts',code('return UnityEngine.Object.FindObjectsOfType<TMPro.TMP_Text>().All(t=>t.font==GameFonts.Title||t.font==GameFonts.Body);'))
    code('UnityEngine.Object.FindObjectOfType<MainMenuManager>().StartGame();return true;');time.sleep(.3);capture('chapter-fonts')
    check('chapter titles use Cinzel and details use Alegreya',code('return GameObject.Find("ReferenceName").GetComponent<TMPro.TMP_Text>().font==GameFonts.Title&&GameObject.Find("ReferenceDetail0").GetComponent<TMPro.TMP_Text>().font==GameFonts.Body;'))
    code('UnityEngine.Object.FindObjectOfType<MainMenuManager>().ShowHome();UnityEngine.Object.FindObjectOfType<MainMenuManager>().OpenSkillTree();return true;');time.sleep(.3);capture('skill-fonts')
    check('skill buttons created on selection use Cinzel',code('var skill=UnityEngine.Object.FindObjectOfType<SkillTreePanelUI>();return skill.nodeLabels.Where(t=>t.enabled&&t.gameObject.activeInHierarchy).All(t=>t.font==GameFonts.Title)&&skill.tabs.SelectMany(t=>t.GetComponentsInChildren<TMPro.TMP_Text>()).Where(t=>t.enabled).All(t=>t.font==GameFonts.Title);'))
    code('SkillTreePanelUI.TryCloseOpen();SceneLoadingUI.LoadScene("GameScane");return true;')
    wait('return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="GameScane"&&!SceneLoadingUI.IsLoading&&ShopManager.Instance!=null;')
    code('UnityEngine.Object.FindObjectOfType<GameManager>().enabled=false;return true;')
    check('HUD and runtime TMP values use the two shared fonts',code('return UnityEngine.Object.FindObjectsOfType<TMPro.TMP_Text>().Where(t=>t.enabled).All(t=>t.font==GameFonts.Title||t.font==GameFonts.Body);'))
    code('var ui=UnityEngine.Object.FindObjectOfType<UİManager>();if(UnityEngine.Object.FindObjectOfType<Tower>()==null)UnityEngine.Object.Instantiate(ui.MegicTowerPrefab);return true;');time.sleep(.5)
    code('var tower=UnityEngine.Object.FindObjectOfType<Tower>();UİManager.activeTower=tower;BackpackUIManager.Instance.OpenBackpack(tower);return true;');time.sleep(.5);capture('inventory-fonts')
    check('tower panel shares static fonts rather than generating dynamic atlases',code('var layout=BackpackUIManager.Instance.GetComponent<TowerPanelLayout>();var so=new SerializedObject(layout);return so.FindProperty("serif").objectReferenceValue==GameFonts.Title&&so.FindProperty("body").objectReferenceValue==GameFonts.Body;'))
    check('Alegreya legacy source is used for future item badges',code('return BackpackUIManager.Instance.itemStatusFont==GameFonts.BodySource;'))
    check('shared fonts remain alive when tower panel closes',code('BackpackUIManager.Instance.CloseBackpack();return GameFonts.Title!=null&&GameFonts.Body!=null&&GameFonts.Title.material.mainTexture!=null&&GameFonts.Body.material.mainTexture!=null;'))
finally:
    m.tool('manage_editor',action='stop');code('UnityEditor.SceneManagement.EditorSceneManager.playModeStartScene=AssetDatabase.LoadAssetAtPath<SceneAsset>('+json.dumps(before['start'])+');return true;')
    final=code('return new{playing=EditorApplication.isPlaying,path=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path,dirty=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().isDirty};')
    (out/'results.json').write_text(json.dumps({'checks':rows,'overflows':overflows,'final':final},ensure_ascii=False,indent=2),encoding='utf-8')
print('Game font checks complete',flush=True)
