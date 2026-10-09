// Runtime-only capture of real BackpackDraggable interactions.
var gm=UnityEngine.Object.FindObjectOfType<GameManager>();
var bags=BackpackUIManager.Instance;
var t=UnityEngine.Object.FindObjectOfType<ArcherTower>();
var dir="C:/Users/Emir/BackPackDefance/Marketing/Steam/2026-10-09/raw/inventory";
System.IO.Directory.CreateDirectory(dir);
BackpackDraggable drag=null;
UnityEngine.Vector2 start=UnityEngine.Vector2.zero,finish=UnityEngine.Vector2.zero;
System.Func<UnityEngine.Vector2,UnityEngine.EventSystems.PointerEventData> pointer=p=>{var e=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);e.position=p;return e;};
System.Func<int,int,UnityEngine.Vector2> target=(x,y)=>{
    var anchor=ItemShapeUtility.RotateCell(drag.Item.centerOffset,ItemShapeUtility.GetBoundingSize(drag.Item.itemShape),drag.Rotation);
    return UnityEngine.RectTransformUtility.WorldToScreenPoint(null,bags.gridContainer.TransformPoint(new UnityEngine.Vector3((x+anchor.x+.5f)*bags.cellSize,-(y+anchor.y+.5f)*bags.cellSize,0)));
};
var sequence=System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0,241),i=>{
    if(!UnityEditor.EditorApplication.isPlaying)return (object)null;
    if(i==30||i==135){
        drag=System.Array.Find(UnityEngine.Object.FindObjectsOfType<BackpackDraggable>(),d=>d.Item!=null&&d.Item.Name=="Çevik Eldivenler");
        start=UnityEngine.RectTransformUtility.WorldToScreenPoint(null,drag.transform.position);
        drag.BeginDrag();drag.UpdateDrag(pointer(start));drag.Rotate(i==30?1:-1);
        finish=i==30?target(3,3):target(3,0);
    }
    if(i>=30&&i<=90)drag.UpdateDrag(pointer(UnityEngine.Vector2.Lerp(start,finish,(i-30)/60f)));
    if(i==90)drag.FinishDrag(pointer(finish));
    if(i>=135&&i<=200)drag.UpdateDrag(pointer(UnityEngine.Vector2.Lerp(start,finish,(i-135)/65f)));
    if(i==200)drag.FinishDrag(pointer(finish));
    if(i>0){var tex=UnityEngine.ScreenCapture.CaptureScreenshotAsTexture();System.IO.File.WriteAllBytes(dir+"/"+(i-1).ToString("D5")+".jpg",UnityEngine.ImageConversion.EncodeToJPG(tex,94));UnityEngine.Object.Destroy(tex);}
    if(i==240)System.IO.File.WriteAllText(dir+"/complete.txt","240 frames; genuine item rotation, movement and recipe adjacency");
    return (object)new UnityEngine.WaitForEndOfFrame();
});
gm.StartCoroutine(sequence.GetEnumerator());
return "Recording eight seconds of inventory interaction";
