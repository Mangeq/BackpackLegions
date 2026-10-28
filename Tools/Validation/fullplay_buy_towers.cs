var ui=UnityEngine.Object.FindObjectOfType<UİManager>();var wm=UnityEngine.Object.FindObjectOfType<WaveManager>();
if(!GameManager.CanBuyTowers)return "not preparation";
ui.CloseTowerPanel();ui.ClosePurchasePanel();
var slots=new System.Collections.Generic.List<TowerSlot>(UnityEngine.Object.FindObjectsOfType<TowerSlot>());
slots.Sort((a,b)=>UnityEngine.Vector3.Distance(a.transform.position,wm.spawnPoint.position).CompareTo(UnityEngine.Vector3.Distance(b.transform.position,wm.spawnPoint.position)));
var owned=UnityEngine.Object.FindObjectsOfType<Tower>();var sequence=new[]{"ArcherTower","UnitTower","MegicTower","ArcherTower","CannonTower"};
var actions=new System.Collections.Generic.List<string>();int n=owned.Length;
foreach(var slot in slots){
 if(slot.isOccupied||n>=sequence.Length)continue;
 if(n>=3&&wm.CurrentWaveIndex==0)break;
 string type=sequence[n];var prefab=type=="ArcherTower"?ui.ArcherTowerPrefab:type=="CannonTower"?ui.MortarTowerPrefab:type=="UnitTower"?ui.UnitTowerPrefab:ui.MegicTowerPrefab;
 if(EconomyManager.Gold<prefab.GetComponent<Tower>().data.cost+(n>=3?300:0))break;
 UİManager.selectedTowerSlot=slot;UİManager.selectedTowerPlace=slot.transform;UİManager.selectedTowerType=type;UİManager.TowerShopUI.SetActive(true);ui.PreparePurchasePanel();
 int before=EconomyManager.Gold;ui.BuyTower();actions.Add(type+" slot="+slot.name+" position="+slot.transform.position+" spent="+(before-EconomyManager.Gold));n++;
}
return string.Join("\n",actions);



