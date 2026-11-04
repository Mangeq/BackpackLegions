"""Local standard MCP SDK client for the configured Unity MCP endpoint."""
print('Loading MCP client',flush=True)
import asyncio,json,sys
from pathlib import Path
from mcp import ClientSession
from mcp.client.streamable_http import streamablehttp_client
async def main():
    request=json.loads(Path(sys.argv[1]).read_text(encoding='utf-8-sig'))
    print('Connecting to configured Unity MCP endpoint',flush=True)
    async with streamablehttp_client('http://127.0.0.1:8080/mcp') as (read,write,_):
        async with ClientSession(read,write) as session:
            await asyncio.wait_for(session.initialize(),30)
            print('MCP session initialized',flush=True)
            results=[]
            for item in request:
                print('Request: '+item.get('uri',item.get('name',item['kind'])),flush=True)
                if item['kind']=='resource':r=await session.read_resource(item['uri'])
                elif item['kind']=='tools':r=await session.list_tools()
                else:r=await session.call_tool(item['name'],item.get('arguments',{}))
                results.append(r.model_dump(mode='json'))
                if len(sys.argv)>2:Path(sys.argv[2]).write_text(json.dumps(results,ensure_ascii=False,indent=2),encoding='utf-8')
            output=json.dumps(results,ensure_ascii=False,indent=2)
            if len(sys.argv)>2:Path(sys.argv[2]).write_text(output,encoding='utf-8')
            else:print(output)
print('Starting MCP transport',flush=True)
asyncio.run(main())
