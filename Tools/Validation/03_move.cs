var rows=new System.Collections.Generic.List<object>();var bags=BackpackUIManager.Instance;
foreach(var tower in UnityEngine.Object.FindObjectsOfType<Tower>()){
UİManager.activeTower=tower;bags.OpenBackpack(tower);UnityEngine.Canvas.ForceUpdateCanvases();var item=tower.Backpack.GetItemAt(UnityEngine.Vector2Int.zero).Data;
BackpackDraggable visual=null;foreach(var v in bags.gridContainer.GetComponentsInChildren<BackpackDraggable>())if(v.Item==item)visual=v;
int gold=EconomyManager.Gold;var ev=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);ev.position=UnityEngine.RectTransformUtility.WorldToScreenPoint(null,bags.gridContainer.TransformPoint(new UnityEngine.Vector3((1.5f+item.centerOffset.x)*bags.cellSize,-(.5f+item.centerOffset.y)*bags.cellSize,0)));
visual.BeginDrag();visual.UpdateDrag(ev);bool moved=visual.FinishDrag(ev);rows.Add(new{test=tower.name+" move item without charge",passed=moved&&tower.Backpack.GetItemAt(UnityEngine.Vector2Int.zero)==null&&tower.Backpack.GetItemAt(new UnityEngine.Vector2Int(1,0)).Data==item&&EconomyManager.Gold==gold});bags.CloseBackpack();}
return rows;
