"""Test the five reference panels against real game values and callbacks."""
import json,time
from pathlib import Path
import run_stage1 as m
out=Path(__file__).resolve().parents[2]/'Docs/Validation/hud-reference';rows=[]
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
    code('var t=typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView");var v=UnityEditor.EditorWindow.GetWindow(t);v.Focus();v.Repaint();Canvas.ForceUpdateCanvases();UnityEditor.EditorApplication.QueuePlayerLoopUpdate();return true;')
    time.sleep(.3)
    m.tool('manage_camera',action='screenshot',include_image=False,output_folder='Docs/Validation/hud-reference',screenshot_file_name=name)

m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'HudReferenceChecks','version':'1.0'}})
m.rpc('resources/read',{'uri':'mcpforunity://custom-tools'})
before=code('return new{playing=EditorApplication.isPlaying,path=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path,dirty=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().isDirty,start=AssetDatabase.GetAssetPath(UnityEditor.SceneManagement.EditorSceneManager.playModeStartScene)};')
if before['playing']:raise RuntimeError('Exit Play first')
try:
    code('UnityEditor.SceneManagement.EditorSceneManager.playModeStartScene=AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/GameScane.unity");return true;')
    m.tool('manage_editor',action='play')
    wait('return UnityEngine.Object.FindObjectOfType<GameManager>()!=null&&ShopManager.Instance!=null;')
    code('UnityEngine.Object.FindObjectOfType<GameManager>().enabled=false;return true;')
    check('gold reads actual fresh chapter budget',code('var ui=UnityEngine.Object.FindObjectOfType<UİManager>();ui.GetComponent<HudReferenceSkin>().Refresh();return GameObject.Find("GoldBar").transform.Find("ReferenceGold").GetComponent<TMPro.TMP_Text>().text==EconomyManager.Gold.ToString()&&EconomyManager.Gold==UnityEngine.Object.FindObjectOfType<WaveManager>().ActiveChapter.startingGold;'))
    check('gold updates on spending and earning',code('int old=EconomyManager.Gold;bool spent=EconomyManager.TrySpendGold(10);EconomyManager.AddGold(20);UnityEngine.Object.FindObjectOfType<HudReferenceSkin>().Refresh();bool shown=GameObject.Find("GoldBar").transform.Find("ReferenceGold").GetComponent<TMPro.TMP_Text>().text==(old+10).ToString();EconomyManager.Gold=old;UİManager.UpdateGoldText();return spent&&shown;'))
    check('wave panel updates both current and total values',code('UİManager.UpdateWaveText(2,7);UnityEngine.Object.FindObjectOfType<HudReferenceSkin>().Refresh();bool ok=GameObject.Find("WaveBar").transform.Find("ReferenceWave").GetComponent<TMPro.TMP_Text>().text=="WAVE:"+((char)10)+"2/7";UİManager.UpdateWaveText(1,UnityEngine.Object.FindObjectOfType<WaveManager>().ActiveChapter.waves.Count);return ok;'))
    check('three native speed buttons stay locked in preparation',code('return UnityEngine.Object.FindObjectOfType<UİManager>().speedButtons.All(x=>!x.interactable)&&Time.timeScale==0;'))
    capture('hud-preparation.png')
    seconds=code('var g=UnityEngine.Object.FindObjectOfType<GameManager>();typeof(GameManager).GetMethod("TickPreparation",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(g,new object[]{1.25f});return Mathf.CeilToInt(g.PreparationSecondsRemaining);')
    time.sleep(.3)
    check('countdown reflects real preparation timer',code('var ui=UnityEngine.Object.FindObjectOfType<UİManager>();return ui.preparationCountdownText.text.Contains("'+str(seconds)+'")&&ui.preparationCountdownText.transform.parent.Find("ReferenceCountdown").GetComponent<TMPro.TMP_Text>().text==ui.preparationCountdownText.text;'))
    code('UnityEngine.Object.FindObjectOfType<GameManager>().TogglePause();return true;')
    time.sleep(.3)
    check('countdown reflects paused preparation state',code('return UnityEngine.Object.FindObjectOfType<UİManager>().preparationCountdownText.text.Contains("duraklatıldı");'))
    code('UnityEngine.Object.FindObjectOfType<GameManager>().ContinueGame();var h=UnityEngine.Object.FindObjectOfType<HoverInfoController>();h.enabled=false;return true;')
    check('item tooltip displays real item name icon and shape',code('var item=AssetDatabase.FindAssets("t:ItemData").Select(x=>AssetDatabase.LoadAssetAtPath<ItemData>(AssetDatabase.GUIDToAssetPath(x))).First(x=>x.cost==0&&x.itemOwners!=null&&x.itemOwners.Contains(TowerType.MagicTower)&&!x.isElementCatalyst);var info=UnityEngine.Object.FindObjectOfType<InfoPanelUI>();info.Show(item,new Vector2(Screen.width*.55f,Screen.height*.7f));return info.IsVisible&&info.icon.sprite==item.itemIcon&&info.panel.Find("ReferenceTitle").GetComponent<TMPro.TMP_Text>().text==item.Name&&info.panel.Find("ReferenceBody").GetComponent<TMPro.TMP_Text>().text==info.bodyText.text;'))
    time.sleep(.3);capture('hud-item-tooltip.png')
    check('tooltip stays inside the screen at every corner',code('var info=UnityEngine.Object.FindObjectOfType<InfoPanelUI>();var root=(RectTransform)info.panel.parent;var c=new Vector3[4];bool ok=true;foreach(var point in new[]{Vector2.zero,new Vector2(Screen.width,0),new Vector2(0,Screen.height),new Vector2(Screen.width,Screen.height)}){info.MoveTo(point);info.panel.GetWorldCorners(c);foreach(var p in c){var local=root.InverseTransformPoint(p);ok&=local.x>=root.rect.xMin-.1f&&local.x<=root.rect.xMax+.1f&&local.y>=root.rect.yMin-.1f&&local.y<=root.rect.yMax+.1f;}}return ok;'))
    check('all existing item descriptions retain native content',code('var info=UnityEngine.Object.FindObjectOfType<InfoPanelUI>();bool ok=true;foreach(var id in AssetDatabase.FindAssets("t:ItemData")){var item=AssetDatabase.LoadAssetAtPath<ItemData>(AssetDatabase.GUIDToAssetPath(id));info.Show(item,new Vector2(Screen.width*.5f,Screen.height*.6f));ok&=info.panel.Find("ReferenceTitle").GetComponent<TMPro.TMP_Text>().text==item.Name&&info.panel.Find("ReferenceBody").GetComponent<TMPro.TMP_Text>().text==info.bodyText.text;}ok&=info.panel.GetComponent<UnityEngine.UI.Image>().type==UnityEngine.UI.Image.Type.Sliced;info.Hide();return ok;'))
    code('var g=UnityEngine.Object.FindObjectOfType<GameManager>();UnityEngine.Object.FindObjectOfType<UİManager>().startGameButton.onClick.Invoke();return true;')
    time.sleep(.3)
    check('native Start begins combat and hides countdown',code('var ui=UnityEngine.Object.FindObjectOfType<UİManager>();return UnityEngine.Object.FindObjectOfType<GameManager>().CurrentState==GameManager.GameState.Combat&&!ui.preparationCountdownText.transform.parent.gameObject.activeSelf;'))
    for index in range(3):
        check('native '+str(index+1)+'x selects actual speed and only its gold highlight',code('var ui=UnityEngine.Object.FindObjectOfType<UİManager>();ui.speedButtons['+str(index)+'].onClick.Invoke();return UnityEngine.Object.FindObjectOfType<GameManager>().SelectedSpeed=='+str(index+1)+'&&Time.timeScale=='+str(index+1)+'&&ui.speedHighlights['+str(index)+'].enabled&&ui.speedHighlights.Count(x=>x.enabled)==1;'))
    time.sleep(.3);capture('hud-speed-3.png')
    check('all speed buttons receive real UI raycasts',code('var ui=UnityEngine.Object.FindObjectOfType<UİManager>();bool ok=true;foreach(var b in ui.speedButtons){var r=(RectTransform)b.transform;var e=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){position=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center))};var hits=new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();UnityEngine.EventSystems.EventSystem.current.RaycastAll(e,hits);ok&=hits.Count>0&&hits[0].gameObject.GetComponentInParent<UnityEngine.UI.Button>()==b;}return ok;'))
    check('all five raster sources have transparent exterior',code('return new[]{"Tooltip","Gold","Wave","Countdown","Speed"}.All(n=>AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Resources/UI/HudReference/"+n+".png").GetPixel(0,0).a<.01f);'))
    code('UnityEngine.Object.FindObjectOfType<GameManager>().TogglePause();return true;');time.sleep(.2)
    check('pause locks speed without changing selected multiplier',code('return UnityEngine.Object.FindObjectOfType<GameManager>().SelectedSpeed==3&&Time.timeScale==0&&UnityEngine.Object.FindObjectOfType<UİManager>().speedButtons.All(x=>!x.interactable);'))
finally:
    m.tool('manage_editor',action='stop')
    code('UnityEditor.SceneManagement.EditorSceneManager.playModeStartScene=AssetDatabase.LoadAssetAtPath<SceneAsset>('+json.dumps(before['start'])+');return true;')
    final=code('return new{playing=EditorApplication.isPlaying,path=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path,dirty=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().isDirty};')
    (out/'results.json').write_text(json.dumps({'before':before,'checks':rows,'final':final},ensure_ascii=False,indent=2),encoding='utf-8')
print('HUD reference checks complete',flush=True)
