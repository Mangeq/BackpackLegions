var ui=UnityEngine.Object.FindObjectOfType<UİManager>();var bags=BackpackUIManager.Instance;var shop=UnityEngine.Object.FindObjectOfType<ShopUIManager>();var wm=UnityEngine.Object.FindObjectOfType<WaveManager>();
if(!GameManager.CanEditItems)return "not preparation";
var actions=new System.Collections.Generic.List<string>();
var bossElement=ElementType.Normal;float highest=0;foreach(var g in wm.waves[wm.waves.Count-1].GetGroups())if(g.enemyPrefab!=null){var e=g.enemyPrefab.GetComponent<Enemy>();if(e.enemyData.health>highest){highest=e.enemyData.health;bossElement=e.enemyData.element;}}
var preferred=bossElement==ElementType.Fire?ElementType.Water:bossElement==ElementType.Water?ElementType.Earth:bossElement==ElementType.Earth?ElementType.Air:ElementType.Fire;
var ordered=UnityEngine.Object.FindObjectsOfType<Tower>();System.Array.Sort(ordered,(a,b)=>{int p=(a is ArcherTower?0:a is MortarTower?1:a is MegicTower?2:3).CompareTo(b is ArcherTower?0:b is MortarTower?1:b is MegicTower?2:3);return p!=0?p:UnityEngine.Vector3.Distance(a.transform.position,wm.spawnPoint.position).CompareTo(UnityEngine.Vector3.Distance(b.transform.position,wm.spawnPoint.position));});
foreach(var tower in ordered){
 UİManager.activeTower=tower;bags.OpenBackpack(tower);shop.UpdateShopUI();UnityEngine.Canvas.ForceUpdateCanvases();
 int rerolls=0;for(int attempt=0;attempt<10;attempt++){
 int chosen=-1;float best=0;UnityEngine.Vector2Int origin=UnityEngine.Vector2Int.zero;
 for(int i=0;i<tower.myShopItems.Length;i++){
  var item=tower.myShopItems[i];if(item==null||item.cost>EconomyManager.Gold)continue;
  if(item.isElementCatalyst&&(!tower.IsElementShop||item.element!=preferred))continue;
  if(!tower.IsItemActive(item))continue;
  float score=item.isElementCatalyst?1000:(item.damageBonus+item.attackSpeedBonus+item.criticalChanceBonus*1.5f+(tower is UnitTower?item.healthBonus*.4f:0))/UnityEngine.Mathf.Max(1,item.cost);
  if(score<=best)continue;
  bool fit=false;UnityEngine.Vector2Int cell=UnityEngine.Vector2Int.zero;PlacedItem main=null;foreach(var p in tower.Backpack.Items)if(tower.IsMainItem(p.Data))main=p;
  for(int y=0;y<tower.Backpack.Height&&!fit;y++)for(int x=0;x<tower.Backpack.Width&&!fit;x++){
   var c=new UnityEngine.Vector2Int(x,y);if(!tower.Backpack.CanPlace(item,c))continue;
   if(item.isElementCatalyst&&(main==null||!TowerItemEvolution.Adjacent(main,new PlacedItem(item,c))))continue;
   fit=true;cell=c;
  }
  if(fit){chosen=i;best=score;origin=cell;}
 }
 if(chosen<0){if(!tower.IsElementShop&&rerolls<1&&EconomyManager.Gold>shop.rerollCost+100){shop.RefreshShop();rerolls++;actions.Add(tower.data.type+" reroll -"+shop.rerollCost);continue;}break;}
 var buy=tower.myShopItems[chosen];int gold=EconomyManager.Gold;
 var ev=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
 ev.position=UnityEngine.RectTransformUtility.WorldToScreenPoint(null,bags.gridContainer.TransformPoint(new UnityEngine.Vector3((origin.x+.5f+buy.centerOffset.x)*bags.cellSize,-(origin.y+.5f+buy.centerOffset.y)*bags.cellSize,0)));
 var drag=shop.shopSlots[chosen].GetComponent<ShopSlotDragStarter>();drag.OnBeginDrag(ev);drag.OnDrag(ev);drag.OnEndDrag(ev);
 actions.Add(tower.data.type+" buy="+buy.Name+" spent="+(gold-EconomyManager.Gold)+" at="+origin);
 if(gold==EconomyManager.Gold)break;
 }
 ui.CloseTowerPanel();
}
return string.Join("\n",actions);



