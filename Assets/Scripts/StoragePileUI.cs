using System.Collections.Generic;
using UnityEngine;

/// <summary>Local UI gravity and box collisions, independent of the combat time scale.</summary>
public sealed class StoragePileUI : MonoBehaviour
{
    sealed class Body { public PlacedItem item; public RectTransform rect; public Vector2 p,v,half; }
    readonly List<Body> bodies=new List<Body>();
    public void Prepare(int count)
    {
        bodies.Clear();
        var rect=(RectTransform)transform;
        rect.sizeDelta=new Vector2(rect.sizeDelta.x,Mathf.Max(455,count*45));
    }
    public void Register(PlacedItem item,RectTransform rect)
    {
        var area=(RectTransform)transform;float unit=Mathf.Min(55,area.rect.width/Mathf.Max(2,item.BoundingSize.x+1));
        var original=ItemShapeUtility.GetBoundingSize(item.Data.itemShape);
        rect.sizeDelta=new Vector2(original.x,original.y)*unit;
        // Fit collision bounds to the actual drawn image rather than unused
        // inventory cells, so narrow gloves/bows do not rest on invisible boxes.
        Vector2 drawn=rect.sizeDelta;var image=rect.GetComponent<UnityEngine.UI.Image>();
        if(image!=null&&image.sprite!=null&&image.preserveAspect)
        {
            float aspect=image.sprite.rect.width/image.sprite.rect.height;
            if(drawn.x/drawn.y>aspect)drawn.x=drawn.y*aspect;else drawn.y=drawn.x/aspect;
        }
        if(item.Rotation%2!=0)drawn=new Vector2(drawn.y,drawn.x);
        var b=new Body{item=item,rect=rect,half=drawn*.48f};
        b.p=item.HasStoragePosition?item.StoragePosition:new Vector2(Random.Range(b.half.x,area.rect.width-b.half.x),b.half.y+10);
        bodies.Add(b);Draw(b);
    }
    void Update()
    {
        float elapsed=Mathf.Min(Time.unscaledDeltaTime,.05f);
        int steps=Mathf.Max(1,Mathf.CeilToInt(elapsed*120));
        for(int i=0;i<steps;i++)Simulate(elapsed/steps);
    }
    public void Simulate(float dt)
    {
        var area=((RectTransform)transform).rect;
        bodies.RemoveAll(b=>b.rect==null||b.item.Bag==null||!b.item.Bag.IsLooseStorage||b.rect.parent!=transform);
        foreach(var b in bodies){b.v.y+=900*dt;b.p+=b.v*dt;}
        for(int pass=0;pass<8;pass++)
        {
            foreach(var b in bodies)
            {
                b.p.x=Mathf.Clamp(b.p.x,b.half.x,area.width-b.half.x);
                if(b.p.y+b.half.y>area.height){b.p.y=area.height-b.half.y;b.v.y=0;b.v.x*=.8f;}
                b.p.y=Mathf.Max(b.half.y,b.p.y);
            }
            for(int i=0;i<bodies.Count;i++)for(int j=i+1;j<bodies.Count;j++)
            {
                var a=bodies[i];var b=bodies[j];var d=b.p-a.p;
                float x=a.half.x+b.half.x-Mathf.Abs(d.x),y=a.half.y+b.half.y-Mathf.Abs(d.y);
                if(x<=0||y<=0)continue;
                if(x<y){float shift=x*.51f*(d.x>=0?1:-1);a.p.x-=shift;b.p.x+=shift;a.v.x=b.v.x=0;}
                else{float shift=y*.51f*(d.y>=0?1:-1);a.p.y-=shift;b.p.y+=shift;a.v.y=b.v.y=0;}
            }
        }
        foreach(var b in bodies)Draw(b);
    }
    static void Draw(Body b)
    {b.rect.anchoredPosition=new Vector2(b.p.x,-b.p.y);b.item.StoragePosition=b.p;b.item.HasStoragePosition=true;}
}
