var ui=UnityEngine.Object.FindObjectOfType<UİManager>();var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.enabled=false;
var tower=UnityEngine.Object.Instantiate(ui.ArcherTowerPrefab,new UnityEngine.Vector3(1000,0,1000),UnityEngine.Quaternion.identity).GetComponent<Tower>();
typeof(Tower).GetMethod("InitializeTower",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(tower,null);
typeof(Tower).GetField("towerLevel",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(tower,5);
var rows=new System.Collections.Generic.List<object>();
foreach(var element in new[]{ElementType.Fire,ElementType.Water}){
 var catalyst=System.Array.Find(UnityEngine.Resources.LoadAll<ItemData>("Data/Items/Elements"),x=>x.element==element);PlacedItem main=null;foreach(var p in tower.Backpack.Items)if(tower.IsMainItem(p.Data))main=p;
 PlacedItem ingredient=null;
 for(int y=0;y<5&&ingredient==null;y++)for(int x=0;x<5&&ingredient==null;x++){
  var cell=new UnityEngine.Vector2Int(x,y);if(!tower.Backpack.CanPlace(catalyst,cell))continue;
  var candidate=new PlacedItem(catalyst,cell);if(TowerItemEvolution.Adjacent(main,candidate))ingredient=tower.Backpack.Place(catalyst,cell);
 }
 tower.CalculateStats();rows.Add(new{test="Catalyst transforms main weapon to "+element,passed=ingredient!=null&&tower.SelectedElement==element&&tower.Backpack.Items.Count==1&&tower.MainItem.element==element});
}
UİManager.activeTower=tower;BackpackUIManager.Instance.OpenBackpack(tower);EconomyManager.Gold=1000;
tower.GenerateShopItems();tower.CompleteShopPurchase(0,tower.myShopItems[0]);
ui.OpenElementChange();
rows.Add(new{test="Actual water element warning",passed=ui.transitionDetails.text.Contains("Su")&&ui.transitionPanel.activeSelf});
return Newtonsoft.Json.JsonConvert.SerializeObject(rows);
