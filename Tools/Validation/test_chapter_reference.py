"""Exercise the real menu callbacks and persistent map presentation."""
import json,time
from pathlib import Path
import run_stage1 as m

out=Path(__file__).resolve().parents[2]/'Docs/Validation/chapter-reference'
rows=[]
def check(name,ok):
    rows.append({'test':name,'passed':bool(ok)})
    print(name,bool(ok),flush=True)
    if not ok:raise AssertionError(name)
def code(s):return m.code(s)
def wait(s):
    end=time.monotonic()+35
    while time.monotonic()<end:
        try:
            if code(s):return
        except RuntimeError:pass
        time.sleep(.4)
    raise TimeoutError(s)
def capture(name):
    code('var t=typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView");var v=UnityEditor.EditorWindow.GetWindow(t);v.Focus();v.Repaint();Canvas.ForceUpdateCanvases();UnityEditor.EditorApplication.QueuePlayerLoopUpdate();return true;')
    time.sleep(.3)
    m.tool('manage_camera',action='screenshot',include_image=False,output_folder='Docs/Validation/chapter-reference',screenshot_file_name=name)

m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'ChapterReferenceChecks','version':'1.0'}})
m.rpc('resources/read',{'uri':'mcpforunity://custom-tools'})
before=code('return new{playing=EditorApplication.isPlaying,path=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path,dirty=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().isDirty,start=AssetDatabase.GetAssetPath(UnityEditor.SceneManagement.EditorSceneManager.playModeStartScene)};')
if before['playing']:raise RuntimeError('Exit Play first')
try:
    code('UnityEditor.SceneManagement.EditorSceneManager.playModeStartScene=AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Boot.unity");return true;')
    m.tool('manage_editor',action='play')
    wait('return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="MainMenu"&&!SceneLoadingUI.IsLoading&&UnityEngine.Object.FindObjectOfType<MainMenuManager>()!=null;')
    check('home starts with chapter map closed',code('var m=UnityEngine.Object.FindObjectOfType<MainMenuManager>();var s=new SerializedObject(m);return !((GameObject)s.FindProperty("chapterPanel").objectReferenceValue).activeSelf;'))
    check('native Play opens reference chapter map',code('var m=UnityEngine.Object.FindObjectOfType<MainMenuManager>();m.StartGame();var p=(GameObject)new SerializedObject(m).FindProperty("chapterPanel").objectReferenceValue;return p.activeInHierarchy&&p.GetComponent<ChapterReferenceSkin>()!=null;'))
    time.sleep(.4)
    capture('chapter-1.png')
    for index in range(5):
        check('chapter '+str(index+1)+' native selection updates real data and shield',code('var m=UnityEngine.Object.FindObjectOfType<MainMenuManager>();var so=new SerializedObject(m);var panel=(GameObject)so.FindProperty("chapterPanel").objectReferenceValue;var markers=so.FindProperty("chapterMarkers");var marker=(UnityEngine.UI.Image)markers.GetArrayElementAtIndex('+str(index)+').objectReferenceValue;marker.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();var chapter=(ChapterData)so.FindProperty("chapters").GetArrayElementAtIndex('+str(index)+').objectReferenceValue;return panel.transform.Find("ReferenceName").GetComponent<TMPro.TMP_Text>().text==chapter.chapterName&&panel.transform.Find("ReferenceDetail0").GetComponent<TMPro.TMP_Text>().text=="DALGA: "+chapter.waves.Count&&panel.transform.Find("ReferenceDetail3").GetComponent<TMPro.TMP_Text>().text=="BAŞLANGIÇ: "+chapter.startingGold+" altın"&&marker.sprite.name=="SelectedNode"&&panel.GetComponentsInChildren<UnityEngine.UI.Image>().Count(x=>x.sprite!=null&&x.sprite.name=="SelectedNode")==1;'))
    time.sleep(.3)
    capture('chapter-5.png')
    check('all seven visible buttons receive real EventSystem raycasts',code('var m=UnityEngine.Object.FindObjectOfType<MainMenuManager>();var panel=(GameObject)new SerializedObject(m).FindProperty("chapterPanel").objectReferenceValue;Canvas.ForceUpdateCanvases();int count=0;bool ok=true;foreach(var b in panel.GetComponentsInChildren<UnityEngine.UI.Button>()){if(!b.interactable)continue;count++;var r=(RectTransform)b.transform;var e=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){position=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center))};var hits=new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();UnityEngine.EventSystems.EventSystem.current.RaycastAll(e,hits);ok&=hits.Count>0&&hits[0].gameObject.GetComponentInParent<UnityEngine.UI.Button>()==b;}return ok&&count==7;'))
    check('star and talent counters read actual saved progress',code('var m=UnityEngine.Object.FindObjectOfType<MainMenuManager>();var so=new SerializedObject(m);var p=(GameObject)so.FindProperty("chapterPanel").objectReferenceValue;var a=so.FindProperty("chapters");var save=new SkillTreeProgress();int total=0;for(int i=0;i<a.arraySize;i++)total+=save.BestPoints(((ChapterData)a.GetArrayElementAtIndex(i).objectReferenceValue).chapterNumber);return p.transform.Find("ReferenceTotalStars").GetComponent<TMPro.TMP_Text>().text==total+"/15"&&p.transform.Find("ReferencePoints").GetComponent<TMPro.TMP_Text>().text==save.AvailablePoints+" YP";'))
    check('back callback returns to original home',code('var m=UnityEngine.Object.FindObjectOfType<MainMenuManager>();var so=new SerializedObject(m);var p=(GameObject)so.FindProperty("chapterPanel").objectReferenceValue;p.transform.Find("Back").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();return !p.activeSelf&&((GameObject)so.FindProperty("homePanel").objectReferenceValue).activeSelf;'))
    code('var m=UnityEngine.Object.FindObjectOfType<MainMenuManager>();m.StartGame();m.SelectChapter(4);var p=(GameObject)new SerializedObject(m).FindProperty("chapterPanel").objectReferenceValue;p.transform.Find("StartChapter").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();return true;')
    wait('return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="GameScane"&&!SceneLoadingUI.IsLoading&&ShopManager.Instance!=null;')
    check('native OYNA loads selected fifth chapter through loader',code('var g=UnityEngine.Object.FindObjectOfType<GameManager>();g.enabled=false;return ChapterData.SelectedIndex==4&&UnityEngine.Object.FindObjectOfType<WaveManager>().ActiveChapter.chapterNumber==5;'))
finally:
    m.tool('manage_editor',action='stop')
    code('UnityEditor.SceneManagement.EditorSceneManager.playModeStartScene=AssetDatabase.LoadAssetAtPath<SceneAsset>('+json.dumps(before['start'])+');return true;')
    final=code('return new{playing=EditorApplication.isPlaying,path=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path,dirty=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().isDirty};')
    (out/'results.json').write_text(json.dumps({'before':before,'checks':rows,'final':final},ensure_ascii=False,indent=2),encoding='utf-8')
print('Chapter reference checks complete',flush=True)
