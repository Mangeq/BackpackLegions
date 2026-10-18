using UnityEngine;
using UnityEngine.UI;

/// <summary>Unbounded storage list, displayed as two columns of single-cell icons.</summary>
public sealed class StorageGridUI : MonoBehaviour
{
    public const int VisibleCapacity=12;
    public float CellWidth { get; private set; }=121;
    public float CellHeight { get; private set; }=83;
    public Sprite cellSprite;
    int cursor;
    public void Prepare(int count)
    {
        cursor=0;
        var rect=(RectTransform)transform;
        var viewport=GetComponentInParent<ScrollRect>(true).viewport;
        CellWidth=viewport.rect.width/2;CellHeight=viewport.rect.height/6;
        rect.sizeDelta=new Vector2(CellWidth*2,Mathf.Max(6,Mathf.CeilToInt(count/2f))*CellHeight);
        var scroll=GetComponentInParent<ScrollRect>(true);
        if(scroll!=null)
        {
            scroll.horizontal=false;scroll.vertical=count>VisibleCapacity;
            scroll.StopMovement();
            if(!scroll.vertical)rect.anchoredPosition=Vector2.zero;
        }
        var background=transform.Find("StorageCells") as RectTransform;
        if(background==null)
        {
            background=new GameObject("StorageCells",typeof(RectTransform),typeof(Image)).GetComponent<RectTransform>();
            background.SetParent(transform,false);
        }
        background.anchorMin=background.anchorMax=background.pivot=new Vector2(0,1);
        background.anchoredPosition=Vector2.zero;background.sizeDelta=rect.sizeDelta;
        var image=background.GetComponent<Image>();image.sprite=cellSprite;
        image.type=Image.Type.Tiled;image.raycastTarget=false;image.color=Color.white;
        background.SetAsFirstSibling();
    }
    public void Register(PlacedItem item,RectTransform rect)
    {
        int index=cursor++;
        rect.pivot=new Vector2(.5f,.5f);rect.localRotation=Quaternion.identity;
        rect.sizeDelta=new Vector2(CellWidth-18,CellHeight-16);
        rect.anchoredPosition=new Vector2((index%2+.5f)*CellWidth,-(index/2+.5f)*CellHeight);
        var image=rect.GetComponent<Image>();image.preserveAspect=true;
    }
}
