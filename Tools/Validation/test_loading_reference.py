"""Check the real Boot/scene loader plus reference art in the connected editor."""
import json,time
from pathlib import Path
import run_stage1 as m

root=Path(__file__).resolve().parents[2]
out=root/'Docs/Validation/loading-reference'
out.mkdir(parents=True,exist_ok=True)
rows=[]
def check(name,passed):
    rows.append({'test':name,'passed':bool(passed)})
    print(name, bool(passed),flush=True)
    if not passed:raise AssertionError(name)
def wait(code):
    deadline=time.monotonic()+35
    while time.monotonic()<deadline:
        try:
            if m.code(code):return
        except RuntimeError:
            pass
        time.sleep(.4)
    raise TimeoutError(code)
def screenshot(name):
    m.code('var t=typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView");var v=UnityEditor.EditorWindow.GetWindow(t);v.Focus();v.Repaint();Canvas.ForceUpdateCanvases();UnityEditor.EditorApplication.QueuePlayerLoopUpdate();return true;')
    time.sleep(.3)
    return m.tool('manage_camera',action='screenshot',include_image=False,output_folder='Docs/Validation/loading-reference',screenshot_file_name=name)

m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'LoadingReferenceChecks','version':'1.0'}})
m.rpc('resources/read',{'uri':'mcpforunity://custom-tools'})
before=m.code('return new {playing=EditorApplication.isPlaying,scene=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path,dirty=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().isDirty,background=Application.runInBackground,start=AssetDatabase.GetAssetPath(UnityEditor.SceneManagement.EditorSceneManager.playModeStartScene)};')
if before['playing']:raise RuntimeError('Exit Play before validation')
try:
    check('saved loader prefab has native reference artwork',m.code('var p=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/UI/SceneLoadingUI.prefab");return p.GetComponent<LoadingReferenceSkin>()!=null&&p.transform.Find("ReferenceArtwork").GetComponent<UnityEngine.UI.Image>().sprite.name=="LoadingBackground";'))
    m.code('Application.runInBackground=true;UnityEditor.SceneManagement.EditorSceneManager.playModeStartScene=AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Boot.unity");return true;')
    m.tool('manage_editor',action='play')
    wait('return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="MainMenu"&&!SceneLoadingUI.IsLoading&&UnityEngine.Object.FindObjectOfType<MainMenuManager>()!=null;')
    check('real Boot loads MainMenu and removes loading overlay',m.code('return UnityEngine.Object.FindObjectOfType<SceneLoadingUI>()==null;'))
    check('progress controls fiery chain at zero half and full',m.code('var p=UnityEngine.Object.Instantiate(Resources.Load<SceneLoadingUI>("UI/SceneLoadingUI"));p.name="LoadingReferencePreview";var so=new SerializedObject(p);var s=(UnityEngine.UI.Slider)so.FindProperty("progress").objectReferenceValue;var image=s.fillRect.GetComponent<UnityEngine.UI.Image>();bool ok=true;foreach(float value in new[]{0f,.5f,1f}){s.value=value;ok&=Mathf.Abs(image.fillAmount-value)<.001f;}s.value=.75f;return ok&&image.sprite.name=="LoadingChain";'))
    time.sleep(.5)
    check('artwork keeps original proportions and complete frame',m.code('var p=UnityEngine.Object.FindObjectOfType<SceneLoadingUI>();var stage=(RectTransform)p.transform.Find("ReferenceArtwork");var parent=((RectTransform)stage.parent).rect;var scale=stage.localScale;return Mathf.Abs(scale.x-scale.y)<.001f&&stage.rect.width*scale.x<=parent.width+.1f&&stage.rect.height*scale.y<=parent.height+.1f;'))
    screenshot('loading-75.png')
    check('failed load enters native error state',m.code('var p=UnityEngine.Object.FindObjectOfType<SceneLoadingUI>();typeof(SceneLoadingUI).GetMethod("Begin",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(p,new object[]{"MissingLoadingValidationScene"});return true;'))
    wait('var p=UnityEngine.Object.FindObjectOfType<SceneLoadingUI>();return p!=null&&!SceneLoadingUI.IsLoading&&p.GetComponentsInChildren<UnityEngine.UI.Button>(true).Any(b=>b.name=="Retry"&&b.gameObject.activeInHierarchy);')
    check('error caption and retry are visible',m.code('var p=UnityEngine.Object.FindObjectOfType<SceneLoadingUI>();var so=new SerializedObject(p);return ((TMPro.TMP_Text)so.FindProperty("status").objectReferenceValue).text=="YÜKLEME BAŞLATILAMADI"&&((UnityEngine.UI.Button)so.FindProperty("retry").objectReferenceValue).GetComponent<UnityEngine.UI.Image>().enabled;'))
    screenshot('loading-error.png')
    m.code('var p=UnityEngine.Object.FindObjectOfType<SceneLoadingUI>();typeof(SceneLoadingUI).GetField("target",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(p,"GameScane");var so=new SerializedObject(p);((UnityEngine.UI.Button)so.FindProperty("retry").objectReferenceValue).onClick.Invoke();return true;')
    wait('return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="GameScane"&&!SceneLoadingUI.IsLoading&&ShopManager.Instance!=null;')
    check('retry uses native async loading and removes overlay',m.code('var g=UnityEngine.Object.FindObjectOfType<GameManager>();g.enabled=false;return UnityEngine.Object.FindObjectOfType<SceneLoadingUI>()==null;'))
    m.code('SceneLoadingUI.LoadScene("MainMenu");return true;')
    wait('return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="MainMenu"&&!SceneLoadingUI.IsLoading&&UnityEngine.Object.FindObjectOfType<MainMenuManager>()!=null;')
    check('normal return to menu uses same loader successfully',m.code('return UnityEngine.Object.FindObjectOfType<SceneLoadingUI>()==null;'))
finally:
    m.tool('manage_editor',action='stop')
    m.code('Application.runInBackground='+str(before['background']).lower()+';UnityEditor.SceneManagement.EditorSceneManager.playModeStartScene=AssetDatabase.LoadAssetAtPath<SceneAsset>('+json.dumps(before['start'])+');return true;')
    final=m.code('return new{playing=EditorApplication.isPlaying,scene=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path,dirty=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().isDirty};')
    report={'before':before,'checks':rows,'final':final}
    (out/'results.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print('Loading reference checks complete',flush=True)
