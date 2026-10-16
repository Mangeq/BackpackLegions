var rows=new System.Collections.Generic.List<object>();System.Action<string,bool> check=(n,v)=>rows.Add(new{test=n,passed=v});
var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
var ui=UnityEngine.Object.FindObjectOfType<UİManager>();ui.CloseTowerPanel();var gm=UnityEngine.Object.FindObjectOfType<GameManager>();var state=typeof(GameManager).GetProperty("CurrentState");state.SetValue(gm,GameManager.GameState.Preparation,null);Time.timeScale=0;
var t=UnityEngine.Object.Instantiate(ui.MortarTowerPrefab,new Vector3(1000,0,1000),Quaternion.identity).GetComponent<MortarTower>();t.enabled=false;typeof(Tower).GetMethod("InitializeTower",flags).Invoke(t,null);
try{
var offers=t.myShopItems.ToArray();t.ToggleShopFreeze();t.AddExperience(140);check("Mortar reaches level-five element gate",t.AwaitingElement&&t.Level==5);
check("First water choice evolves main freely and keeps offers frozen",t.TrySelectElement(ElementType.Water)&&t.MainItem==t.data.waterWeapon&&t.IsShopFrozen&&t.myShopItems.SequenceEqual(offers));
t.GenerateShopItems();check("Refresh offers are all water mortar gear",t.myShopItems.All(i=>i!=null&&i.element==ElementType.Water&&i.itemOwners.Contains(TowerType.CannonTower)));
int gold=EconomyManager.Gold;int level=t.Level;float xp=t.Experience;
check("Paid element change preserves level XP and refreshes shop",t.TryChangeElement(ElementType.Fire)&&t.Level==level&&t.Experience==xp&&EconomyManager.Gold==gold-100&&!t.IsShopFrozen&&t.myShopItems.All(i=>i.element==ElementType.Fire));
foreach(var recipe in t.evolutionRecipes)
{
    typeof(Tower).GetProperty("SelectedElement").SetValue(t,recipe.first.element,null);
    foreach(var placed in t.Backpack.Items.ToArray())t.Backpack.Remove(placed);
    t.Backpack.Place(recipe.first,Vector2Int.zero);t.Backpack.Place(recipe.second,new Vector2Int(2,0));
    var evo=new TowerItemEvolution(t);evo.BeginWave(1);bool pending=evo.Pending.Count==1;evo.CompleteWave(1);
    check("Successful wave crafts mortar recipe "+recipe.first.element,pending&&t.Backpack.Items.Any(p=>p.Data==recipe.result));
}
typeof(Tower).GetProperty("SelectedElement").SetValue(t,ElementType.Normal,null);
foreach(var placed in t.Backpack.Items.ToArray())t.Backpack.Remove(placed);
var normal=t.evolutionRecipes[0];var first=t.Backpack.Place(normal.first,Vector2Int.zero);var second=t.Backpack.Place(normal.second,new Vector2Int(2,0));t.CalculateStats();float withGear=(float)typeof(Tower).GetField("currentDamage",flags).GetValue(t);t.Backpack.Remove(first);t.Backpack.Remove(second);t.Storage.Place(normal.first,Vector2Int.zero);t.Storage.Place(normal.second,new Vector2Int(0,2));t.CalculateStats();check("Stored mortar equipment supplies no buff",withGear>(float)typeof(Tower).GetField("currentDamage",flags).GetValue(t));
var slot=UnityEngine.Object.FindObjectsOfType<TowerSlot>().First(s=>!s.isOccupied);UİManager.selectedTowerSlot=slot;UİManager.selectedTowerPlace=slot.transform;UİManager.TowerShopUI.SetActive(true);EconomyManager.Gold=0;ui.CannonTowerBTN();int count=UnityEngine.Object.FindObjectsOfType<MortarTower>().Length;ui.BuyTower();check("Insufficient money reports failure and keeps slot free",!slot.isOccupied&&ui.towerPurchaseFeedback.text.Contains("Yetersiz")&&UnityEngine.Object.FindObjectsOfType<MortarTower>().Length==count);
EconomyManager.Gold=10000;state.SetValue(gm,GameManager.GameState.Combat,null);ui.BuyTower();check("Combat blocks mortar purchase",!slot.isOccupied&&EconomyManager.Gold==10000&&UnityEngine.Object.FindObjectsOfType<MortarTower>().Length==count);
state.SetValue(gm,GameManager.GameState.Preparation,null);ui.BuyTower();ui.BuyTower();check("One purchase charges exactly once and occupies slot",slot.isOccupied&&EconomyManager.Gold==9650&&UnityEngine.Object.FindObjectsOfType<MortarTower>().Length==count+1);
}finally{UnityEngine.Object.DestroyImmediate(t.gameObject);state.SetValue(gm,GameManager.GameState.Preparation,null);}
return rows;
