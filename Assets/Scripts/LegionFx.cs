using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class LegionFx : MonoBehaviour
{
    static Material ink;
    public static Material Ink
    {
        get
        {
            if (!ink) ink=new Material(Resources.Load<Shader>("BlackpineLegion/Stroke"));
            return ink;
        }
    }
    public static Color Gold => new Color(1,.72f,.24f);
    public static Color Hue(ElementType e)
    {
        switch(e)
        {
            case ElementType.Fire: return new Color(1,.34f,.08f);
            case ElementType.Water: return new Color(.17f,.76f,1);
            case ElementType.Earth: return new Color(.74f,.52f,.29f);
            case ElementType.Air: return new Color(.56f,1,.85f);
            default: return new Color(1,.87f,.57f);
        }
    }
    public static LineRenderer Stroke(Transform parent, Color color, float width, int points)
    {
        var go=new GameObject("Stroke");
        go.transform.SetParent(parent,false);
        var lr=go.AddComponent<LineRenderer>();
        lr.sharedMaterial=Ink;
        lr.useWorldSpace=false;
        lr.startColor=lr.endColor=color;
        lr.startWidth=lr.endWidth=width;
        lr.positionCount=points;
        lr.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows=false;
        return lr;
    }

    public static void Burst(Vector3 pos, ElementType element, bool critical=false)
    {
        EmitShape(pos,element.ToString(),critical ? .95f : .68f);
        if(critical) EmitShape(pos,"Critical",1.06f);
    }

    // Scene-local instances are recycled per silhouette. Unity destroys them on scene unload.
    static readonly Dictionary<string,Stack<LegionFx>> impactPools=new Dictionary<string,Stack<LegionFx>>();
    static void EmitShape(Vector3 pos,string shape,float size)
    {
        if(!impactPools.TryGetValue(shape,out var pool)) impactPools[shape]=pool=new Stack<LegionFx>();
        LegionFx fx=null;
        while(pool.Count>0 && !fx) fx=pool.Pop();
        if(!fx)
        {
            var asset=Resources.Load<GameObject>("BlackpineLegion/Effects/"+shape);
            if(!asset)return;
            fx=Instantiate(asset).AddComponent<LegionFx>();
        }
        fx.transform.position=pos+Vector3.up*.08f;
        fx.gameObject.SetActive(true);
        fx.StartCoroutine(fx.ShapeLife(pool,size));
    }
    IEnumerator ShapeLife(Stack<LegionFx> pool,float size)
    {
        for(float t=0;t<1;t+=GameManager.EffectDelta/.24f)
        {
            float radius=size*Mathf.Lerp(.18f,1,Mathf.Sqrt(t));
            transform.localScale=new Vector3(radius,Mathf.Max(.02f,radius*(1-t)),radius);
            yield return null;
        }
        if(pool.Count>=24){Destroy(gameObject);yield break;}
        gameObject.SetActive(false);
        pool.Push(this);
    }

    public static void Slash(Vector3 from,Vector3 to)
    {
        Vector3 forward=Vector3.ProjectOnPlane(to-from,Vector3.up);
        if(forward.sqrMagnitude<.001f)return;
        var go=new GameObject("Kritik silah izi");
        go.transform.position=Vector3.Lerp(from,to,.6f)+Vector3.up*.22f;
        go.transform.rotation=Quaternion.LookRotation(forward);
        var fx=go.AddComponent<LegionFx>();
        var line=Stroke(go.transform,Gold,.048f,7);
        for(int i=0;i<7;i++)
        {
            float t=i/6f;
            line.SetPosition(i,new Vector3((t-.5f)*.60f,.15f*(1-t),.24f*Mathf.Sin(t*Mathf.PI)));
        }
        line.endWidth=.005f;
        fx.StartCoroutine(fx.Expand(.16f,0,false));
    }
    public static void Ring(Vector3 pos,float radius,Color color,float rise=0,bool realtime=false)
    {
        var go=new GameObject("Ripple");
        go.transform.position=pos+Vector3.up*.04f;
        var fx=go.AddComponent<LegionFx>();
        var lr=Stroke(go.transform,color,.045f,32);
        lr.loop=true;
        for(int i=0;i<32;i++)
        {
            float a=i*Mathf.PI/16;
            lr.SetPosition(i,new Vector3(Mathf.Cos(a),0,Mathf.Sin(a))*radius);
        }
        fx.StartCoroutine(fx.Expand(.38f,rise,realtime));
    }
    IEnumerator Expand(float duration,float rise,bool realtime)
    {
        var strokes=GetComponentsInChildren<LineRenderer>();
        var colors=new Color[strokes.Length];
        for(int i=0;i<strokes.Length;i++) colors[i]=strokes[i].startColor;
        for(float t=0;t<1;)
        {
            transform.localScale=Vector3.one*Mathf.Lerp(.16f,1,Mathf.Sqrt(t));
            float dt=realtime?Time.unscaledDeltaTime:GameManager.EffectDelta;
            transform.position+=Vector3.up*(rise*dt/duration);
            for(int i=0;i<strokes.Length;i++){
                Color c=colors[i];c.a=1-t;strokes[i].startColor=strokes[i].endColor=c;
            }
            t+=dt/duration;
            yield return null;
        }
        Destroy(gameObject);
    }

    public static void Trail(GameObject go,Color color,float width)
    {
        var tr=go.AddComponent<TrailRenderer>();
        tr.sharedMaterial=Ink;tr.startColor=color;tr.endColor=new Color(color.r,color.g,color.b,0);
        tr.startWidth=width;tr.endWidth=0;tr.time=.10f;tr.minVertexDistance=.025f;
    }
}
