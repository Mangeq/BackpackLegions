var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Public;
var rows=new System.Collections.Generic.List<object>();var towers=UnityEngine.Object.FindObjectsOfType<Tower>();var t=towers[0];var other=towers[1];var bags=BackpackUIManager.Instance;var shop=UnityEngine.Object.FindObjectOfType<ShopUIManager>();var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.enabled=false;UİManager.activeTower=t;bags.OpenBackpack(t);
var damage=typeof(Tower).GetField("currentDamage",flags);float original=(float)damage.GetValue(t);
var d=UnityEngine.Object.Instantiate(ShopManager.Instance.allItems[0]);d.Name="Depo Test Eşyası";d.cost=100;d.damageBonus=.5f;d.attackSpeedBonus=0;d.criticalChanceBonus=0;d.healthBonus=0;d.itemShape=new System.Collections.Generic.List<UnityEngine.Vector2Int>{UnityEngine.Vector2Int.zero,new UnityEngine.Vector2Int(0,1)};d.centerOffset=UnityEngine.Vector2Int.zero;
t.myShopItems[0]=d;shop.UpdateShopUI();UnityEngine.Canvas.ForceUpdateCanvases();
System.Func<UnityEngine.RectTransform,int,int,UnityEngine.EventSystems.PointerEventData> pointer=(grid,x,y)=>{var ev=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);ev.position=UnityEngine.RectTransformUtility.WorldToScreenPoint(null,grid.TransformPoint(new UnityEngine.Vector3((x+.5f)*bags.cellSize,-(y+.5f)*bags.cellSize,0)));return ev;};
System.Func<BackpackDraggable> visual=()=>{var dict=(System.Collections.Generic.Dictionary<PlacedItem,UnityEngine.GameObject>)typeof(BackpackUIManager).GetField("itemVisuals",flags).GetValue(bags);foreach(var pair in dict)if(pair.Key.Data==d)return pair.Value.GetComponent<BackpackDraggable>();return null;};
EconomyManager.Gold=1000;var ev0=pointer(bags.storageGridContainer,0,0);var starter=shop.shopSlots[0].GetComponent<ShopSlotDragStarter>();starter.OnBeginDrag(ev0);starter.OnDrag(ev0);starter.OnEndDrag(ev0);
rows.Add(new{test="shop to storage: one charge, no bonus",passed=t.Storage.Items.Count==1&&EconomyManager.Gold==900&&(float)damage.GetValue(t)==original&&t.myShopItems[0]==null});
var v=visual();v.BeginDrag();var equip=pointer(bags.gridContainer,0,0);v.UpdateDrag(equip);bool ok=v.FinishDrag(equip);float equipped=(float)damage.GetValue(t);
rows.Add(new{test="storage to backpack: free, bonus active",passed=ok&&t.Storage.Items.Count==0&&equipped>original&&EconomyManager.Gold==900});
v=visual();v.BeginDrag();v.UpdateDrag(ev0);v.Rotate(1);var rotated=pointer(bags.storageGridContainer,1,0);v.UpdateDrag(rotated);ok=v.FinishDrag(rotated);
rows.Add(new{test="backpack to rotated storage: bonus removed",passed=ok&&t.Storage.Items.Count==1&&t.Storage.Items[0].Rotation==1&&(float)damage.GetValue(t)==original&&EconomyManager.Gold==900});
v=visual();v.BeginDrag();var outside=pointer(bags.storageGridContainer,2,4);v.UpdateDrag(outside);v.Rotate(-1);ok=v.FinishDrag(outside);
rows.Add(new{test="invalid rotated storage drop restores source and orientation",passed=!ok&&t.Storage.Items[0].Rotation==1&&t.Storage.Items[0].Origin==UnityEngine.Vector2Int.zero});
v=visual();v.BeginDrag();v.UpdateDrag(equip);gm.TogglePause();rows.Add(new{test="pause restores storage item",passed=t.Storage.Items.Count==1&&!GameManager.CanEditItems});gm.ContinueGame();
v=visual();v.BeginDrag();UİManager.activeTower=other;bags.OpenBackpack(other);rows.Add(new{test="tower switch cancels to owner; no shared storage",passed=t.Storage.Items.Count==1&&other.Storage.Items.Count==0&&t.Storage!=other.Storage});
UİManager.activeTower=t;bags.OpenBackpack(t);rows.Add(new{test="reopen preserves storage rotation",passed=visual().Rotation==1});
// Teklif sayısı geçici runtime dizisinde artırılır; asset/denge değişmez.
var oldOffers=t.myShopItems;t.myShopItems=new ItemData[8];for(int i=0;i<8;i++)t.myShopItems[i]=d;shop.UpdateShopUI();UnityEngine.Canvas.ForceUpdateCanvases();
rows.Add(new{test="eight offers create eight cards with scroll overflow",passed=shop.shopSlots.Length==8&&shop.offersScroll.content.rect.height>shop.offersScroll.viewport.rect.height});
t.myShopItems=oldOffers;shop.UpdateShopUI();int visible=0;foreach(var slot in shop.shopSlots)if(slot.gameObject.activeSelf)visible++;rows.Add(new{test="return to four offers hides extra cards",passed=visible==4});
v=visual();v.BeginDrag();v.UpdateDrag(equip);gm.StartGame();rows.Add(new{test="combat cancels transfer into owner storage",passed=gm.CurrentState==GameManager.GameState.Combat&&t.Storage.Items.Count==1&&(float)damage.GetValue(t)==original});
return rows;

