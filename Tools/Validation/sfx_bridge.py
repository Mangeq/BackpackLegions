import sys,json
sys.path.insert(0,'Tools/Validation')
import run_stage1 as m
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'SfxValidation','version':'1'}})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
mode=sys.argv[1] if len(sys.argv)>1 else 'inspect'
if mode=='inspect':
 print(json.dumps(m.code('return new { playing=UnityEditor.EditorApplication.isPlaying, compiling=UnityEditor.EditorApplication.isCompiling, dirty=UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty };')))
 print(json.dumps([t for t in m.rpc('tools/list',{})['tools'] if t['name'] in ['refresh_unity','read_console']]))
elif mode=='refresh': print(json.dumps(m.tool('refresh_unity',mode='force',scope='all',compile='request',wait_for_ready=True)))
elif mode=='validate':
 print(json.dumps(m.code('SfxValidation.Validate(); return "Validation completed";')))
 print(json.dumps(m.tool('read_console',action='get',types=['error'],count=20,include_stacktrace=False)))
elif mode=='runtime':
 import time
 from pathlib import Path
 before=m.code('return new { playing=UnityEditor.EditorApplication.isPlaying, dirty=UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty, background=UnityEngine.Application.runInBackground };')
 if before['playing'] or before['dirty']: raise RuntimeError('Play test requires stopped, saved scene')
 report={}
 def ready_code(code):
  end=time.monotonic()+45
  while True:
   try:
    m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
    return m.code(code)
   except Exception:
    if time.monotonic()>end: raise
    time.sleep(3)
 try:
  m.code('UnityEngine.Application.runInBackground=true; return true;')
  m.tool('manage_editor',action='play')
  start=ready_code('return UnityEngine.Time.frameCount;')
  time.sleep(1)
  frame=m.code('return UnityEngine.Time.frameCount;')
  report['frames_advance']=frame>start
  report['bootstrap']=m.code('var a=UnityEngine.Object.FindObjectOfType<GameAudio>(); return new { exists=a!=null, voices=a!=null?a.GetComponents<UnityEngine.AudioSource>().Length:0, buttons=UnityEngine.Object.FindObjectsOfType<ButtonAudio>(true).Length };')
  report['paused_ui_audio']=m.code('UnityEngine.Time.timeScale=0; GameAudio.Play("panel_open"); var a=UnityEngine.Object.FindObjectOfType<GameAudio>(); int playing=0;foreach(var s in a.GetComponents<UnityEngine.AudioSource>())if(s.isPlaying)playing++;return playing>0;')
  report['mute']=m.code('SoundEffectsVolume.SetGain(0);GameAudio.Play("arrow_tower_attack",.3f,true);var a=UnityEngine.Object.FindObjectOfType<GameAudio>();foreach(var s in a.GetComponents<UnityEngine.AudioSource>())if(s.volume!=0)return false;return true;')
  report['restore_volume']=m.code('SoundEffectsVolume.SetGain(.5f);var a=UnityEngine.Object.FindObjectOfType<GameAudio>();foreach(var s in a.GetComponents<UnityEngine.AudioSource>())if(UnityEngine.Mathf.Abs(s.volume-.5f)>.001f)return false;return true;')
  report['bounded_combat']=m.code('var a=UnityEngine.Object.FindObjectOfType<GameAudio>();foreach(var s in a.GetComponents<UnityEngine.AudioSource>())s.Stop();for(int i=0;i<100;i++)GameAudio.Play("arrow_tower_attack",.2f,true,0);int n=0;foreach(var s in a.GetComponents<UnityEngine.AudioSource>())if(s.isPlaying)n++;return n==12;')
  report['result_stops_combat']=m.code('GameAudio.Result(false);var a=UnityEngine.Object.FindObjectOfType<GameAudio>();int n=0;foreach(var s in a.GetComponents<UnityEngine.AudioSource>())if(s.isPlaying)n++;return n<=1;')
  report['console']=m.tool('read_console',action='get',types=['error'],count=20,include_stacktrace=False)
  Path('Docs/Validation/sfx/runtime.json').write_text(json.dumps(report,indent=2))
  print(json.dumps(report),flush=True)
  assert report['frames_advance'] and report['bootstrap']['voices']==19 and all(report[k] for k in ['paused_ui_audio','mute','restore_volume','bounded_combat','result_stops_combat'])
 finally:
  m.tool('manage_editor',action='stop')
  ready_code('UnityEngine.Application.runInBackground='+str(before['background']).lower()+'; return true;')
