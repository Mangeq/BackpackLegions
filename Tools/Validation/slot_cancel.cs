UnityEngine.Object.FindObjectOfType<GameManager>().enabled=false;
var slots=UnityEngine.Object.FindObjectsOfType<TowerSlot>();var a=slots[0];var b=slots[1];var c=slots[2];var ui=UnityEngine.Object.FindObjectOfType<UİManager>();var rows=new System.Collections.Generic.List<object>();
a.SendMessage("OnMouseDown");var close=UnityEngine.GameObject.Find("TowerShopCloseBTN").GetComponent<UnityEngine.UI.Button>();close.onClick.Invoke();
rows.Add(new{test="close button releases first slot",passed=a.GetComponent<UnityEngine.Collider>().enabled&&!a.isOccupied&&UİManager.selectedTowerSlot==null});
b.SendMessage("OnMouseDown");ui.ArcherTowerBTN();int gold=EconomyManager.Gold;ui.BuyTower();
rows.Add(new{test="buy at second slot leaves first available",passed=b.isOccupied&&!b.GetComponent<UnityEngine.Collider>().enabled&&a.GetComponent<UnityEngine.Collider>().enabled&&EconomyManager.Gold==gold-ui.ArcherTowerPrefab.GetComponent<Tower>().data.cost});
a.SendMessage("OnMouseDown");rows.Add(new{test="original slot selectable after purchase elsewhere",passed=UİManager.selectedTowerSlot==a&&UİManager.TowerShopUI.activeSelf});ui.MegicTowerBTN();ui.BuyTower();rows.Add(new{test="purchase succeeds at original slot",passed=a.isOccupied});
c.SendMessage("OnMouseDown");slots[3].SendMessage("OnMouseDown");close.onClick.Invoke();rows.Add(new{test="switching before close releases both slots",passed=c.GetComponent<UnityEngine.Collider>().enabled&&slots[3].GetComponent<UnityEngine.Collider>().enabled});
c.SendMessage("OnMouseDown");UİManager.TowerShopUI.SetActive(false);typeof(UİManager).GetMethod("Update",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(ui,null);
rows.Add(new{test="external panel hide releases selection",passed=c.GetComponent<UnityEngine.Collider>().enabled&&UİManager.selectedTowerSlot==null});
for(int i=0;i<3;i++){c.SendMessage("OnMouseDown");close.onClick.Invoke();}
rows.Add(new{test="repeated open close retains availability",passed=c.GetComponent<UnityEngine.Collider>().enabled&&!c.isOccupied});
c.SendMessage("OnMouseDown");ui.ArcherTowerBTN();EconomyManager.Gold=0;ui.BuyTower();close.onClick.Invoke();
rows.Add(new{test="insufficient gold then close leaves slot usable",passed=!c.isOccupied&&c.GetComponent<UnityEngine.Collider>().enabled});
rows.Add(new{test="occupied slots remain locked",passed=a.isOccupied&&b.isOccupied&&!a.GetComponent<UnityEngine.Collider>().enabled&&!b.GetComponent<UnityEngine.Collider>().enabled});
return rows;
