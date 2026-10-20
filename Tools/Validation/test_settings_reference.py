"""Exercise native settings controls, engine state, persistence and cancel semantics."""
import json,time
from pathlib import Path
import run_stage1 as m
out=Path(__file__).resolve().parents[2]/'Docs/Validation/settings-reference'
rows=[]
prefix='var ui=UnityEngine.Object.FindObjectOfType<SettingsPanelUI>();var so=new SerializedObject(ui);'
def code(s):return m.code(s)
def setting(s):return code(prefix+s)
def check(name,value):
    rows.append({'test':name,'passed':bool(value)});print(name,bool(value),flush=True)
    if not value:raise AssertionError(name)
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
    m.tool('manage_camera',action='screenshot',include_image=False,output_folder='Docs/Validation/settings-reference',screenshot_file_name=name)

m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'SettingsReferenceChecks','version':'1.0'}})
m.rpc('resources/read',{'uri':'mcpforunity://custom-tools'})
before=code('var keys=new[]{"Volume","SFX","Quality","VSync","Width","Height","Fullscreen","FPS"};return new{playing=EditorApplication.isPlaying,path=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path,dirty=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().isDirty,start=AssetDatabase.GetAssetPath(UnityEditor.SceneManagement.EditorSceneManager.playModeStartScene),volume=AudioListener.volume,quality=QualitySettings.GetQualityLevel(),vsync=QualitySettings.vSyncCount,fps=Application.targetFrameRate,width=Screen.width,height=Screen.height,mode=(int)Screen.fullScreenMode,prefs=keys.Select(k=>new{name=k,has=PlayerPrefs.HasKey("BL.Settings."+k),value=k=="Volume"||k=="SFX"?PlayerPrefs.GetFloat("BL.Settings."+k):PlayerPrefs.GetInt("BL.Settings."+k)}).ToArray()};')
if before['playing']:raise RuntimeError('Exit Play first')
(out/'settings-backup.json').write_text(json.dumps(before,indent=2),encoding='utf-8')
try:
    code('UnityEditor.SceneManagement.EditorSceneManager.playModeStartScene=AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Boot.unity");return true;')
    m.tool('manage_editor',action='play')
    wait('return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="MainMenu"&&!SceneLoadingUI.IsLoading&&UnityEngine.Object.FindObjectOfType<MainMenuManager>()!=null;')
    runtime=code('return new{volume=AudioListener.volume,sfx=SoundEffectsVolume.Gain,quality=QualitySettings.GetQualityLevel(),vsync=QualitySettings.vSyncCount,fps=Application.targetFrameRate};')
    check('native menu settings opens approved reference panel',code('UnityEngine.Object.FindObjectOfType<MainMenuManager>().OpenSettings();return SettingsPanelUI.IsOpen&&UnityEngine.Object.FindObjectOfType<SettingsPanelUI>().GetComponent<SettingsReferenceSkin>()!=null;'))
    check('sliders update independent native percent captions',setting('((UnityEngine.UI.Slider)so.FindProperty("volume").objectReferenceValue).value=.71f;((UnityEngine.UI.Slider)so.FindProperty("effects").objectReferenceValue).value=.55f;return ((TMPro.TMP_Text)so.FindProperty("volumeText").objectReferenceValue).text=="71%"&&((TMPro.TMP_Text)so.FindProperty("effectsText").objectReferenceValue).text=="55%";'))
    time.sleep(.3);capture('settings-menu.png')
    check('draft does not change engine before Apply',code('return Mathf.Abs(AudioListener.volume-'+str(runtime['volume'])+'f)<.001f&&Mathf.Abs(SoundEffectsVolume.Gain-'+str(runtime['sfx'])+'f)<.001f;'))
    check('Cancel discards slider edits',setting('var modal=(GameObject)so.FindProperty("modal").objectReferenceValue;modal.GetComponentsInChildren<UnityEngine.UI.Button>().First(b=>b.name=="Cancel").onClick.Invoke();ui.Open();return Mathf.Abs(((UnityEngine.UI.Slider)so.FindProperty("volume").objectReferenceValue).value-AudioListener.volume)<.001f&&Mathf.Abs(((UnityEngine.UI.Slider)so.FindProperty("effects").objectReferenceValue).value-SoundEffectsVolume.Gain)<.001f;'))
    check('resolution and quality arrows cycle their drafts',setting('var modal=(GameObject)so.FindProperty("modal").objectReferenceValue;var r=(TMPro.TMP_Text)so.FindProperty("resolutionText").objectReferenceValue;var q=(TMPro.TMP_Text)so.FindProperty("qualityText").objectReferenceValue;string firstR=r.text,firstQ=q.text;modal.GetComponentsInChildren<UnityEngine.UI.Button>().First(b=>b.name=="ResolutionNext").onClick.Invoke();bool changedR=r.text!=firstR;modal.GetComponentsInChildren<UnityEngine.UI.Button>().First(b=>b.name=="ResolutionPrevious").onClick.Invoke();modal.GetComponentsInChildren<UnityEngine.UI.Button>().First(b=>b.name=="QualityNext").onClick.Invoke();return changedR&&r.text==firstR&&q.text!=firstQ;'))
    check('VSync disables conflicting FPS control',setting('var v=(UnityEngine.UI.Toggle)so.FindProperty("vsync").objectReferenceValue;v.isOn=true;return !((UnityEngine.UI.Button)so.FindProperty("fpsPrevious").objectReferenceValue).interactable&&!((UnityEngine.UI.Button)so.FindProperty("fpsNext").objectReferenceValue).interactable&&((TMPro.TMP_Text)so.FindProperty("fpsText").objectReferenceValue).text=="VSYNC";'))
    check('FPS arrows work with VSync off including unlimited',setting('((UnityEngine.UI.Toggle)so.FindProperty("vsync").objectReferenceValue).isOn=false;var values=new System.Collections.Generic.HashSet<string>();for(int i=0;i<8;i++){values.Add(((TMPro.TMP_Text)so.FindProperty("fpsText").objectReferenceValue).text);((UnityEngine.UI.Button)so.FindProperty("fpsNext").objectReferenceValue).onClick.Invoke();}return values.Count==8&&values.Contains("SINIRSIZ")&&values.Contains("60");'))
    time.sleep(.3)
    check('all nine active buttons receive native raycasts',setting('var modal=(GameObject)so.FindProperty("modal").objectReferenceValue;Canvas.ForceUpdateCanvases();bool ok=true;int count=0;foreach(var b in modal.GetComponentsInChildren<UnityEngine.UI.Button>()){if(!b.interactable||b.name=="DismissBackdrop")continue;count++;var r=(RectTransform)b.transform;var e=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){position=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center))};var hits=new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();UnityEngine.EventSystems.EventSystem.current.RaycastAll(e,hits);ok&=hits.Count>0&&hits[0].gameObject.GetComponentInParent<UnityEngine.UI.Button>()==b;}return ok&&count==9;'))
    check('SFX slider accepts actual pointer positions',setting('var s=(UnityEngine.UI.Slider)so.FindProperty("effects").objectReferenceValue;var area=(RectTransform)s.handleRect.parent;var p=area.rect;var e=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){button=UnityEngine.EventSystems.PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(null,area.TransformPoint(new Vector2(p.xMin+p.width*.25f,p.center.y)))};s.OnPointerDown(e);s.OnPointerUp(e);return Mathf.Abs(s.value-.25f)<.02f;'))
    check('language row has no interactive controls',setting('var p=((GameObject)so.FindProperty("modal").objectReferenceValue).transform.Find("ReferenceSettingsCard");return p.Find("LanguagePreviousDisabled").GetComponent<UnityEngine.UI.Button>()==null&&p.Find("LanguageNextDisabled").GetComponent<UnityEngine.UI.Button>()==null;'))
    applied=setting('((UnityEngine.UI.Slider)so.FindProperty("volume").objectReferenceValue).value=.4f;((UnityEngine.UI.Slider)so.FindProperty("effects").objectReferenceValue).value=.25f;((UnityEngine.UI.Toggle)so.FindProperty("fullscreen").objectReferenceValue).isOn=true;var q=typeof(SettingsPanelUI).GetField("qualityIndex",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(ui);var fps=((TMPro.TMP_Text)so.FindProperty("fpsText").objectReferenceValue).text;ui.Apply();return new{quality=(int)q,fps=fps};')
    check('Apply changes master, effects, quality and saves display flags',code('return !SettingsPanelUI.IsOpen&&Mathf.Abs(AudioListener.volume-.4f)<.001f&&Mathf.Abs(SoundEffectsVolume.Gain-.25f)<.001f&&QualitySettings.GetQualityLevel()=='+str(applied['quality'])+'&&PlayerPrefs.GetInt("BL.Settings.Fullscreen")==1&&PlayerPrefs.GetInt("BL.Settings.Width")>0&&PlayerPrefs.GetInt("BL.Settings.Height")>0&&PlayerPrefs.GetFloat("BL.Settings.Volume")==.4f&&PlayerPrefs.GetFloat("BL.Settings.SFX")==.25f&&Application.targetFrameRate==PlayerPrefs.GetInt("BL.Settings.FPS");'))
    code('var go=new GameObject("SettingsAudioProbe");var s=go.AddComponent<AudioSource>();s.volume=.8f;SoundEffectsVolume.SetGain(.25f);return true;')
    time.sleep(.3)
    check('SFX scales real sources without compounding',code('var s=GameObject.Find("SettingsAudioProbe").GetComponent<AudioSource>();bool first=Mathf.Abs(s.volume-.2f)<.001f;SoundEffectsVolume.SetGain(0);bool silent=s.volume==0;SoundEffectsVolume.SetGain(.5f);bool restored=Mathf.Abs(s.volume-.4f)<.001f;SoundEffectsVolume.SetGain(.25f);UnityEngine.Object.Destroy(s.gameObject);return first&&silent&&restored;'))
    check('VSync Apply sets sync and clears conflicting frame cap',setting('ui.Open();((UnityEngine.UI.Toggle)so.FindProperty("vsync").objectReferenceValue).isOn=true;ui.Apply();return QualitySettings.vSyncCount==1&&Application.targetFrameRate==-1&&PlayerPrefs.GetInt("BL.Settings.VSync")==1;'))
    check('saved settings reapply after engine values are reset',code('AudioListener.volume=1;SoundEffectsVolume.SetGain(1);QualitySettings.vSyncCount=0;Application.targetFrameRate=30;typeof(SettingsPanelUI).GetMethod("ApplySaved",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic).Invoke(null,null);return Mathf.Abs(AudioListener.volume-.4f)<.001f&&Mathf.Abs(SoundEffectsVolume.Gain-.25f)<.001f&&QualitySettings.vSyncCount==1&&Application.targetFrameRate==-1;'))
    code('SceneLoadingUI.LoadScene("GameScane");return true;')
    wait('return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="GameScane"&&!SceneLoadingUI.IsLoading&&ShopManager.Instance!=null;')
    check('settings persist across real scene transition',code('return Mathf.Abs(AudioListener.volume-.4f)<.001f&&Mathf.Abs(SoundEffectsVolume.Gain-.25f)<.001f&&QualitySettings.GetQualityLevel()=='+str(applied['quality'])+';'))
    check('game pause opens same reference settings',code('var g=UnityEngine.Object.FindObjectOfType<GameManager>();g.enabled=false;g.TogglePause();g.OpenSettings();return SettingsPanelUI.IsOpen&&g.IsPaused&&Time.timeScale==0;'))
    time.sleep(.3);capture('settings-game.png')
    check('Close discards edits and preserves game pause',setting('((UnityEngine.UI.Slider)so.FindProperty("volume").objectReferenceValue).value=.9f;var p=(GameObject)so.FindProperty("modal").objectReferenceValue;p.GetComponentsInChildren<UnityEngine.UI.Button>().First(b=>b.name=="Close").onClick.Invoke();var g=UnityEngine.Object.FindObjectOfType<GameManager>();return !SettingsPanelUI.IsOpen&&g.IsPaused&&Time.timeScale==0&&Mathf.Abs(AudioListener.volume-.4f)<.001f;'))
finally:
    restore=[]
    for p in before['prefs']:
        key=json.dumps('BL.Settings.'+p['name'])
        if not p['has']:restore.append('PlayerPrefs.DeleteKey('+key+');')
        elif p['name'] in ('Volume','SFX'):restore.append('PlayerPrefs.SetFloat('+key+','+str(p['value'])+'f);')
        else:restore.append('PlayerPrefs.SetInt('+key+','+str(int(p['value']))+');')
    restore.append('PlayerPrefs.Save();AudioListener.volume='+str(before['volume'])+'f;QualitySettings.SetQualityLevel('+str(before['quality'])+');QualitySettings.vSyncCount='+str(before['vsync'])+';Application.targetFrameRate='+str(before['fps'])+';Screen.SetResolution('+str(before['width'])+','+str(before['height'])+',(FullScreenMode)'+str(before['mode'])+');return true;')
    code(''.join(restore))
    m.tool('manage_editor',action='stop')
    code('UnityEditor.SceneManagement.EditorSceneManager.playModeStartScene=AssetDatabase.LoadAssetAtPath<SceneAsset>('+json.dumps(before['start'])+');return true;')
    final=code('return new{playing=EditorApplication.isPlaying,path=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path,dirty=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().isDirty};')
    (out/'results.json').write_text(json.dumps({'checks':rows,'final':final},ensure_ascii=False,indent=2),encoding='utf-8')
print('Settings reference checks complete',flush=True)
