"""Drive the project's existing local Unity MCP integration."""
import sys, json
from pathlib import Path
import run_stage1 as m

m.rpc('initialize', {'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'LoadingReference','version':'1.0'}})
m.rpc('resources/read', {'uri':'mcpforunity://custom-tools'})
action = sys.argv[1]
if action == 'code':
    print(json.dumps(m.code(Path(sys.argv[2]).read_text(encoding='utf-8-sig')),ensure_ascii=False))
elif action == 'tool':
    args=Path(sys.argv[3]).read_text(encoding='utf-8-sig') if Path(sys.argv[3]).is_file() else sys.argv[3]
    result=m.tool(sys.argv[2],**json.loads(args))
    # Screenshots stay on disk; do not print inline base64 data.
    print(json.dumps(result,ensure_ascii=False))
