import run_stage1 as m,json,time
from pathlib import Path
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'UnitRouteObservation','version':'1'}})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
m.tool('manage_editor',action='play')
try:
 time.sleep(3);m.wait_ready()
 print(m.code('var ui=UnityEngine.Object.FindObjectOfType<UİManager>();var list=new System.Collections.Generic.List<TowerSlot>(UnityEngine.Object.FindObjectsOfType<TowerSlot>());list.Sort((a,b)=>b.name.CompareTo(a.name));int n=0;foreach(var slot in list){if(n++==3)break;UİManager.selectedTowerSlot=slot;UİManager.selectedTowerPlace=slot.transform;UİManager.selectedTowerType="UnitTower";UİManager.TowerShopUI.SetActive(true);ui.PreparePurchasePanel();ui.BuyTower();}return EconomyManager.Gold;'),flush=True)
 time.sleep(1)
 result=m.code(Path('Tools/Validation/fullplay_unit_positions.cs').read_text(encoding='utf-8-sig'))
 Path('Docs/Playtests/2026-10-27/unit-route-observation.json').write_text(result,encoding='utf-8');print(result,flush=True)
finally:m.tool('manage_editor',action='stop')
