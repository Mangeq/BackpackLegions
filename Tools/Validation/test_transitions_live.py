import run_stage1 as m,json,time
from pathlib import Path
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'TransitionValidation','version':'1'}})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
m.code('UnityEngine.Application.runInBackground=true; return true;')
m.tool('manage_editor',action='play')
time.sleep(3)
m.wait_ready()
print(m.code('return "frame="+UnityEngine.Time.frameCount;'))
time.sleep(1)
print(m.code('return "frame="+UnityEngine.Time.frameCount;'))
r=m.code(Path('Tools/Validation/validate_transitions_live.cs').read_text(encoding='utf-8-sig'))
print(r)
Path('Docs/Validation/transitions-2026-10-27.json').write_text(r,encoding='utf-8')
