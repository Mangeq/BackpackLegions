var rows=new System.Collections.Generic.List<object>();
var ui=UnityEngine.Object.FindObjectOfType<UİManager>();var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.enabled=false;EconomyManager.Gold=10000;
var init=typeof(Tower).GetMethod("InitializeTower",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);
var level=typeof(Tower).GetField("towerLevel",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);
var state=typeof(GameManager).GetField("<CurrentState>k__BackingField",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);
foreach(var prefab in new[]{ui.ArcherTowerPrefab,ui.MegicTowerPrefab,ui.MortarTowerPrefab,ui.UnitTowerPrefab}){
 var tower=UnityEngine.Object.Instantiate(prefab,new UnityEngine.Vector3(1000,0,1000),UnityEngine.Quaternion.identity).GetComponent<Tower>();init.Invoke(tower,null);level.SetValue(tower,5);tower.GenerateShopItems();tower.CompleteShopPurchase(0,tower.myShopItems[0]);
 string name=tower.data.type.ToString();
 foreach(var element in new[]{ElementType.Fire,ElementType.Water}){
  var catalyst=System.Array.Find(UnityEngine.Resources.LoadAll<ItemData>("Data/Items/Elements"),x=>x.element==element);PlacedItem main=null;foreach(var item in tower.Backpack.Items)if(tower.IsMainItem(item.Data))main=item;
  PlacedItem ingredient=null;
  for(int y=0;y<5&&ingredient==null;y++)for(int x=0;x<5&&ingredient==null;x++){
   var cell=new UnityEngine.Vector2Int(x,y);if(!tower.Backpack.CanPlace(catalyst,cell))continue;
   if(TowerItemEvolution.Adjacent(main,new PlacedItem(catalyst,cell)))ingredient=tower.Backpack.Place(catalyst,cell);
  }
  if(element==ElementType.Water)tower.NotifyElementCatalystPlaced(ingredient);
  tower.CalculateStats();rows.Add(new{test=name+" catalyst -> "+element,passed=ingredient!=null&&tower.SelectedElement==element});
  if(element==ElementType.Fire){
   EconomyManager.Gold=tower.ElementChangeCost;int before=EconomyManager.Gold;
   UİManager.activeTower=tower;BackpackUIManager.Instance.OpenBackpack(tower);ui.OpenElementChange();ui.CloseTransition();
   rows.Add(new{test=name+" reject keeps old element",passed=tower.SelectedElement==ElementType.Fire&&EconomyManager.Gold==before});
   ui.OpenElementChange();ui.ConfirmTransition();
   rows.Add(new{test=name+" accept neutralizes main and spends exact remaining gold",passed=tower.SelectedElement==ElementType.Normal&&tower.MainItem==tower.data.defaultWeapon&&tower.Backpack.Items.Count==1&&tower.Backpack.Items[0].Data==tower.data.defaultWeapon&&EconomyManager.Gold==0&&tower.IsElementShop&&tower.Level==5});
   tower.CalculateStats();rows.Add(new{test=name+" stat refresh stays neutral",passed=tower.SelectedElement==ElementType.Normal});
   state.SetValue(gm,GameManager.GameState.Combat);tower.CalculateStats();
   rows.Add(new{test=name+" wave without catalyst stays neutral",passed=tower.SelectedElement==ElementType.Normal&&tower.Backpack.Items[0].Data==tower.data.defaultWeapon});
   state.SetValue(gm,GameManager.GameState.Preparation);
  }
 }
 ui.CloseTowerPanel();UnityEngine.Object.Destroy(tower.gameObject);
}
return Newtonsoft.Json.JsonConvert.SerializeObject(rows);
