using UnityEngine;

/// <summary>Resolution-independent sword, hourglass, critical hit and target symbols.</summary>
[RequireComponent(typeof(CanvasRenderer))]
public sealed class TowerStatIcon : UnityEngine.UI.Graphic
{
    public int kind;
    protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vh)
    {
        vh.Clear();
        var gold = new Color(1, .72f, .25f);
        var steel = new Color(.72f, .85f, .91f);
        if (kind == 0)
        {
            Polygon(vh, steel, new Vector2(.35f,.35f), new Vector2(.47f,.3f), new Vector2(.9f,.78f), new Vector2(.93f,.96f), new Vector2(.75f,.92f));
            Line(vh, new Vector2(.13f,.1f), new Vector2(.39f,.38f), .14f, gold);
            Line(vh, new Vector2(.18f,.48f), new Vector2(.52f,.18f), .12f, gold);
        }
        else if (kind == 1)
        {
            Polygon(vh, steel, new Vector2(.2f,.85f),new Vector2(.8f,.85f),new Vector2(.58f,.5f),new Vector2(.8f,.15f),new Vector2(.2f,.15f),new Vector2(.42f,.5f));
            Polygon(vh, gold,new Vector2(.27f,.2f),new Vector2(.73f,.2f),new Vector2(.5f,.45f));
            Line(vh,new Vector2(.15f,.92f),new Vector2(.85f,.92f),.12f,gold);
            Line(vh,new Vector2(.15f,.08f),new Vector2(.85f,.08f),.12f,gold);
        }
        else if (kind == 2)
        {
            var points = new Vector2[16];
            for(int i=0;i<16;i++){float a=i*Mathf.PI/8;points[i]=Vector2.one*.5f+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*(i%2==0?.48f:.21f);}
            Polygon(vh,new Color(1,.38f,.08f),points);
            Ring(vh,.16f,.16f,gold);
        }
        else
        {
            Ring(vh,.44f,.1f,new Color(.9f,.24f,.18f)); Ring(vh,.27f,.08f,steel); Ring(vh,.1f,.1f,gold);
            Line(vh,new Vector2(.5f,.5f),new Vector2(.94f,.94f),.08f,gold);
        }
    }
    void Ring(UnityEngine.UI.VertexHelper vh,float radius,float thickness,Color c)
    {
        for(int i=0;i<32;i++){float a=i*Mathf.PI/16,b=(i+1)*Mathf.PI/16;
            Vector2 p=new Vector2(Mathf.Cos(a),Mathf.Sin(a)),q=new Vector2(Mathf.Cos(b),Mathf.Sin(b));
            Polygon(vh,c,Vector2.one*.5f+p*radius,Vector2.one*.5f+q*radius,Vector2.one*.5f+q*(radius-thickness),Vector2.one*.5f+p*(radius-thickness));}
    }
    void Line(UnityEngine.UI.VertexHelper vh,Vector2 a,Vector2 b,float width,Color c)
    {var d=(b-a).normalized;var n=new Vector2(-d.y,d.x)*width*.5f;Polygon(vh,c,a-n,b-n,b+n,a+n);}
    void Polygon(UnityEngine.UI.VertexHelper vh,Color c,params Vector2[] points)
    {
        int start=vh.currentVertCount; var r=rectTransform.rect;
        // Triangle fan is sufficient for these simple icon silhouettes.
        foreach(var p in points)vh.AddVert(new Vector3(r.xMin+p.x*r.width,r.yMin+p.y*r.height),c,Vector2.zero);
        for(int i=1;i<points.Length-1;i++)vh.AddTriangle(start,start+i,start+i+1);
    }
}
