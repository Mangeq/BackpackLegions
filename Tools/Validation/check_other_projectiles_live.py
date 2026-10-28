import run_stage1 as m,time,json
from pathlib import Path
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'ProjectileVisibility','version':'1'}})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
m.tool('manage_editor',action='play')
try:
 time.sleep(3);m.wait_ready()
 m.code('UnityEngine.Object.FindObjectOfType<GameManager>().enabled=false;return true;')
 print(m.code(Path('Tools/Validation/preview_projectiles_live.cs').read_text(encoding='utf-8-sig')),flush=True)
 print(m.tool('manage_camera',action='screenshot',screenshot_file_name='other-projectiles-enlarged',output_folder='Docs/Validation/projectiles-2026-10-27'),flush=True)
 time.sleep(1)
finally:
 print(m.tool('manage_editor',action='stop'),flush=True)
