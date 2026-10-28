import run_stage1 as m,json,time,traceback
from pathlib import Path
out=Path('Docs/Playtests/2026-10-27/retry-front-blocker');out.mkdir(parents=True,exist_ok=True)
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'FullCampaignPlaythrough','version':'1'}})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
def codefile(name):return m.code(Path('Tools/Validation/'+name+'.cs').read_text(encoding='utf-8-sig'))
def status():return json.loads(codefile('fullplay_status'))
def log(kind,data):
 row={'elapsed':round(time.monotonic()-start,1),'kind':kind,'data':data};events.append(row)
 (out/'events.json').write_text(json.dumps(events,ensure_ascii=False,indent=2),encoding='utf-8')
 print(json.dumps(row,ensure_ascii=False),flush=True)
def capture(name):return m.tool('manage_camera',action='screenshot',screenshot_file_name=name,output_folder='Docs/Playtests/2026-10-27/retry-front-blocker')
def wait_scene(name,timeout=45):
 deadline=time.monotonic()+timeout
 while time.monotonic()<deadline:
  s=status()
  if s['scene']==name and not s['loading']:return s
  time.sleep(.5)
 raise TimeoutError(name)
start=time.monotonic();events=[];results=[]
before=json.loads(codefile('playthrough_inspect'));(out/'before.json').write_text(json.dumps(before,ensure_ascii=False,indent=2),encoding='utf-8')
if before['playing'] or before['dirty']:raise RuntimeError('Open Play or unsaved scene; not changed')
try:
 m.code('UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/Boot.unity");UnityEngine.Application.runInBackground=true;return true;')
 m.tool('manage_editor',action='play');time.sleep(3)
 log('boot_to_menu',wait_scene('MainMenu'));capture('01-main-menu')
 m.code('UnityEngine.Object.FindObjectOfType<MainMenuManager>().StartGame();return true;');capture('02-chapter-map')
 for chapter in [0]:
  if chapter==0 or status()['scene']=='MainMenu':
   m.code('var menu=UnityEngine.Object.FindObjectOfType<MainMenuManager>();menu.StartGame();menu.SelectChapter('+str(chapter)+');menu.PlaySelectedChapter();return true;')
  else:
   s=status()
   if s['state']=='Victory':m.code('UnityEngine.Object.FindObjectOfType<GameManager>().NextChapter();return true;')
   else:
    m.code('UnityEngine.Object.FindObjectOfType<GameManager>().ExitToMainMenu();return true;');wait_scene('MainMenu')
    m.code('var menu=UnityEngine.Object.FindObjectOfType<MainMenuManager>();menu.StartGame();menu.SelectChapter('+str(chapter)+');menu.PlaySelectedChapter();return true;')
  time.sleep(1);s=wait_scene('GameScane');log('chapter_start',s);capture('chapter-'+str(chapter+1)+'-start')
  deadline=time.monotonic()+900;lastkey=None;lastshot=0;prep_wave=-1
  while time.monotonic()<deadline:
   s=status();key=(s['state'],s['wave'],s['hp'])
   if key!=lastkey:log('state',s);lastkey=key
   if s['state'] in ['Victory','Defeat']:
    log('result',s);results.append(s);capture('chapter-'+str(chapter+1)+'-'+s['state'].lower());break
   if s['state']=='Preparation' and s['wave']!=prep_wave:
    prep_wave=s['wave'];log('buy_towers',codefile('fullplay_buy_towers'));time.sleep(.5)
    log('shop',codefile('fullplay_shop'));capture('chapter-'+str(chapter+1)+'-wave-'+str(s['wave']+1)+'-ready')
    log('prepared',status())
    m.code('var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.StartGame();gm.SetGameSpeed(3);return gm.CurrentState.ToString();')
   if time.monotonic()-lastshot>35:
    capture('chapter-'+str(chapter+1)+'-wave-'+str(s['wave']+1)+'-combat');lastshot=time.monotonic();log('combat_observation',s)
   time.sleep(5)
  else:
   log('timeout',status());results.append(dict(status(),result='timeout'))
 (out/'results.json').write_text(json.dumps(results,ensure_ascii=False,indent=2),encoding='utf-8')
 log('console',m.tool('read_console',action='get',types=['error','warning'],count=100))
 m.code('UnityEngine.Object.FindObjectOfType<GameManager>().ExitToMainMenu();return true;');log('return_to_menu',wait_scene('MainMenu'));capture('99-final-menu')
except Exception:
 log('runner_error',traceback.format_exc());raise
finally:
 try:m.tool('manage_editor',action='stop')
 finally:
  script='UnityEngine.Application.runInBackground='+str(before['background']).lower()+';UnityEditor.PlayerSettings.runInBackground='+str(before['background']).lower()+';'
  if before['save']:script+='UnityEngine.PlayerPrefs.SetString(SkillTreeProgress.SaveKey,'+json.dumps(before['save'])+');'
  else:script+='UnityEngine.PlayerPrefs.DeleteKey(SkillTreeProgress.SaveKey);'
  script+='UnityEngine.PlayerPrefs.Save();UnityEditor.SceneManagement.EditorSceneManager.OpenScene('+json.dumps(before['scene'])+');UnityEditor.AssetDatabase.SaveAssets();return true;'
  m.code(script)
 print('PLAYTHROUGH_FINISHED',flush=True)


