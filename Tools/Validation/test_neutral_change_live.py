import run_stage1 as m,json,time
from pathlib import Path
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'NeutralElementRegression','version':'1'}})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
m.code('UnityEngine.Application.runInBackground=true;return true;')
m.tool('manage_editor',action='play')
try:
 time.sleep(3);m.wait_ready()
 print(m.code('return UnityEngine.Time.frameCount;'),flush=True);time.sleep(1);print(m.code('return UnityEngine.Time.frameCount;'),flush=True)
 result=m.code(Path('Tools/Validation/validate_neutral_change_live.cs').read_text(encoding='utf-8-sig'))
 Path('Docs/Validation/neutral-element-2026-10-27.json').write_text(result,encoding='utf-8')
 print(result,flush=True)
 print(m.tool('read_console',action='get',types=['error'],count=20),flush=True)
finally:
 print(m.tool('manage_editor',action='stop'),flush=True)
