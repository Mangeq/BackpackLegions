"""Controlled scene/producer checks through the local Unity MCP bridge."""
import json,time,pathlib
import run_stage1 as m
ROOT=pathlib.Path(__file__).resolve().parents[2]
report={'checks':[]}
def add(name,ok):
    report['checks'].append({'test':name,'passed':bool(ok)})
def wait_scene(name):
    deadline=time.monotonic()+25
    while time.monotonic()<deadline:
        try:
            if m.code('return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="'+name+'" && !SceneLoadingUI.IsLoading && '+('ShopManager.Instance!=null;' if name=='GameScane' else 'UnityEngine.Object.FindObjectOfType<MainMenuManager>()!=null;')):return
        except RuntimeError as error:
            if "'message': None" not in str(error):raise
        time.sleep(.25)
    raise TimeoutError(name)
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'CampaignValidation','version':'1'}})
m.rpc('resources/read',{'uri':'mcpforunity://custom-tools'})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
before=m.code('return new{playing=UnityEditor.EditorApplication.isPlaying,dirty=UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty,background=Application.runInBackground};')
if before['playing'] or before['dirty']:raise RuntimeError('Save scene and stop Play first.')
try:
    report['data']=m.code((ROOT/'Tools/Validation/test_campaign_data.cs').read_text(encoding='utf-8'))
    m.code('UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");Application.runInBackground=true;return true;')
    m.tool('manage_editor',action='play');wait_scene('MainMenu')
    for i in range(5):
        m.code('var menu=UnityEngine.Object.FindObjectOfType<MainMenuManager>();menu.StartGame();menu.SelectChapter('+str(i)+');menu.PlaySelectedChapter();return true;')
        wait_scene('GameScane')
        state=m.code('var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.enabled=false;var wm=UnityEngine.Object.FindObjectOfType<WaveManager>();return new{chapter=wm.ActiveChapter.chapterNumber,rounds=wm.waves.Count,prep=gm.PreparationSecondsRemaining,events=UnityEngine.Object.FindObjectsOfType<UnityEngine.EventSystems.EventSystem>().Length};')
        add('Menu selection loads chapter '+str(i+1),state['chapter']==i+1 and state['rounds']==5+2*i and abs(state['prep']-(60+15*i))<5 and state['events']==1)
        if i<4:
            m.code('UnityEngine.Object.FindObjectOfType<GameManager>().ExitToMainMenu();return true;');wait_scene('MainMenu')
    # Actual three-type simultaneous spawn; all counts isolated from saved assets.
    first=m.code('var wm=UnityEngine.Object.FindObjectOfType<WaveManager>();var source=wm.waves[3];var wave=new WaveRound();foreach(var g in source.groups)wave.groups.Add(new WaveEnemyGroup{enemyPrefab=g.enemyPrefab,enemyCount=2,spawnInterval=.2f});wm.waves=new System.Collections.Generic.List<WaveRound>{wave};var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.StartGame();gm.TogglePause();return new{count=UnityEngine.Object.FindObjectsOfType<Enemy>().Length,types=UnityEngine.Object.FindObjectsOfType<Enemy>().Select(e=>e.enemyData).Distinct().Count()};')
    add('Three enemy types start together',first['count']==3 and first['types']==3)
    time.sleep(.5)
    add('Pause freezes group production',m.code('return UnityEngine.Object.FindObjectsOfType<Enemy>().Length==3;'))
    m.code('UnityEngine.Object.FindObjectOfType<GameManager>().ContinueGame();return true;');time.sleep(.7)
    add('All groups produce their full counts',m.code('return UnityEngine.Object.FindObjectsOfType<Enemy>().Length==6;'))
    m.code('var wm=UnityEngine.Object.FindObjectOfType<WaveManager>();wm.StopWaves();foreach(var e in UnityEngine.Object.FindObjectsOfType<Enemy>())UnityEngine.Object.DestroyImmediate(e.gameObject);wm.ConfigureSelectedChapter();typeof(WaveManager).GetField("currentWaveIndex",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(wm,3);UİManager.UpdateWaveText(4,wm.waves.Count);UnityEngine.Object.FindObjectOfType<GameManager>().BeginPreparation();return true;')
    add('Later preparation applies chapter plus wave increase',m.code('return Mathf.Approximately(UnityEngine.Object.FindObjectOfType<GameManager>().PreparationSecondsRemaining,135f);'))
    time.sleep(.3)
    add('Preparation preview opens with three correct types',m.code('var p=UnityEngine.Object.FindObjectOfType<WavePreviewUI>();return p.panel.activeSelf && p.details.text==WavePreviewUI.Describe(UnityEngine.Object.FindObjectOfType<WaveManager>().UpcomingWave);'))
    m.tool('manage_camera',action='screenshot',output_folder='Docs/Validation',screenshot_file_name='campaign-wave-preview-2026-10-13.png')
    time.sleep(.4)
    report['console']=m.tool('read_console',action='get',types=['error','warning'],count=30,include_stacktrace=False)
finally:
    try:m.tool('manage_editor',action='stop')
    finally:
        m.code('Application.runInBackground='+str(before['background']).lower()+';return true;')
        (ROOT/'Docs/Validation/campaign-2026-10-13.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps({'dataChecks':len(report.get('data',[])),'checks':report['checks'],'failed':[r for r in report.get('data',[])+report['checks'] if not r['passed']],'console':report.get('console')},ensure_ascii=False,indent=2))
