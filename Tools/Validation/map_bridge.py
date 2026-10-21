import sys,json
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parent))
import run_stage1 as m
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'MapSetup','version':'1'}})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
if sys.argv[1]=='code':
 print(json.dumps(m.code(Path(sys.argv[2]).read_text(encoding='utf-8-sig')),ensure_ascii=False,indent=2))
else:
 print(json.dumps(m.tool(sys.argv[1],**json.loads(sys.argv[2])),ensure_ascii=False,indent=2))
