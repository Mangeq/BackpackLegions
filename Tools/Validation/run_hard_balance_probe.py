import json,time,pathlib
import run_stage1 as m
root=pathlib.Path(__file__).resolve().parents[2]
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'BalanceProbe','version':'1'}})
m.rpc('resources/read',{'uri':'mcpforunity://custom-tools'})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
report=[]
try:
    for scenario in ['starter','poor','strong']:
        if m.code('return UnityEditor.EditorApplication.isPlaying;'):m.tool('manage_editor',action='stop')
        m.tool('manage_editor',action='play')
        time.sleep(2)
        m.wait_ready()
        data=m.code((root/'Tools/Validation/hard_balance_setup.cs').read_text(encoding='utf-8').replace('SCENARIO',scenario))
        deadline=time.monotonic()+75
        while time.monotonic()<deadline:
            time.sleep(2)
            state=m.code('var gm=UnityEngine.Object.FindObjectOfType<GameManager>();return new{state=gm.CurrentState.ToString(),wave=UnityEngine.Object.FindObjectOfType<WaveManager>().CurrentWaveIndex,health=GameObject.FindWithTag("Base").GetComponent<Base>().currentHealth,enemies=UnityEngine.Object.FindObjectsOfType<Enemy>().Select(e=>new{e.enemyData.enemyName,e.CurrentHealth}).ToArray()};')
            if state['state']!='Combat':break
        data['result']=state;report.append(data)
        print(json.dumps(data,ensure_ascii=False),flush=True)
finally:
    m.tool('manage_editor',action='stop')
    (root/'Docs/Validation/hard-balance-combat-2026-10-13.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
