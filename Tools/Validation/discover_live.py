import run_stage1 as m,json
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'TransitionValidation','version':'1'}})
r=m.rpc('tools/list',{})
print(json.dumps([x for x in r['tools'] if any(s in x['name'] for s in ['camera','screenshot','editor'])],ensure_ascii=False))
