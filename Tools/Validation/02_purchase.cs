var rows=new System.Collections.Generic.List<object>();var bags=BackpackUIManager.Instance;var shop=UnityEngine.Object.FindObjectOfType<ShopUIManager>();
foreach(var tower in UnityEngine.Object.FindObjectsOfType<Tower>()){
rows.Add(new{test=tower.name+" default weapon",passed=tower.Backpack.Items.Count==1&&tower.Backpack.Items[0].Data==tower.data.defaultWeapon});
UİManager.activeTower=tower;bags.OpenBackpack(tower);
var item=ShopManager.Instance.allItems.Find(x=>ItemShapeUtility.GetBoundingSize(x.itemShape).x==1&&System.Array.Exists(x.itemOwners,o=>o==tower.data.type));
if(item==null)throw new System.Exception("No narrow test item for "+tower.data.type);
tower.myShopItems[0]=item;shop.UpdateShopUI();UnityEngine.Canvas.ForceUpdateCanvases();
var ev=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);ev.position=UnityEngine.RectTransformUtility.WorldToScreenPoint(null,bags.gridContainer.TransformPoint(new UnityEngine.Vector3((.5f+item.centerOffset.x)*bags.cellSize,-(.5f+item.centerOffset.y)*bags.cellSize,0)));
EconomyManager.Gold=1000;int gold=EconomyManager.Gold;var drag=shop.shopSlots[0].GetComponent<ShopSlotDragStarter>();drag.OnBeginDrag(ev);drag.OnDrag(ev);drag.OnEndDrag(ev);
rows.Add(new{test=tower.name+" item purchase",passed=tower.Backpack.Items.Count==2&&tower.Backpack.GetItemAt(UnityEngine.Vector2Int.zero).Data==item&&EconomyManager.Gold==gold-item.cost&&tower.myShopItems[0]==null});
bags.CloseBackpack();}
return rows;


