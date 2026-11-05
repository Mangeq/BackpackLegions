using UnityEngine;

// Only the art is animated. Navigation, colliders, health bars and combat stay on the owner.
public sealed class LegionFigure : MonoBehaviour
{
    AnimationClip atlas;
    float phase, hitTime, deathTime;
    bool dying, flying;
    Quaternion facing;
    Transform model;
    float height;
    public Vector3 Center => transform.position+Vector3.up*(height*.55f);
    public float HealthBarY => transform.position.y+height;
    Renderer[] surfaces;
    MaterialPropertyBlock[] original;
    bool flashing;
    MaterialPropertyBlock tint;
    static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");

    public static LegionFigure Attach(Transform owner, Transform anchor, GameObject asset, float height, bool air)
    {
        if (!asset) return null;
        foreach (var r in owner.GetComponentsInChildren<MeshRenderer>()) r.enabled = false;
        var holder = new GameObject(asset.name + " Figure");
        holder.transform.SetParent(anchor ? anchor : owner, false);
        var scale = holder.transform.parent.lossyScale;
        holder.transform.localScale = new Vector3(height/scale.x,height/scale.y,height/scale.z);
        holder.transform.position += Vector3.down * (air ? height*.5f : height*.16f);
        var fig = holder.AddComponent<LegionFigure>();
        fig.model = Instantiate(asset,holder.transform,false).transform;
        var animation = fig.model.GetComponentInChildren<Animation>();
        if (animation)
        {
            foreach (AnimationState state in animation) { fig.atlas = state.clip; break; }
            animation.enabled = false;
        }
        fig.surfaces = fig.model.GetComponentsInChildren<Renderer>();
        fig.tint = new MaterialPropertyBlock();
        fig.original=new MaterialPropertyBlock[fig.surfaces.Length];
        fig.phase = Mathf.Repeat(owner.GetInstanceID()*.618034f,1);
        fig.height=height*(air?1:1.28f);
        fig.flying = air;
        fig.facing = holder.transform.rotation;
        return fig;
    }

    void Pose(int sequence, float t)
    {
        if (atlas) atlas.SampleAnimation(model.gameObject,(sequence*24+Mathf.Clamp01(t)*23)/24f);
    }

    public void Tick(bool moving, Vector3 direction, float speed, float remaining = -1, float interval = 1)
    {
        if (dying) return;
        direction.y = 0;
        if (direction.sqrMagnitude > .0001f)
            facing = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,facing,Time.deltaTime*700);
        phase += Time.deltaTime * (moving ? Mathf.Clamp(speed*.55f,.7f,4f) : .8f);
        if (remaining >= 0)
        {
            float progress = 1-Mathf.Clamp01(remaining/Mathf.Max(.01f,interval));
            // Recover from the previous contact, then prepare for the next.
            if (progress < .38f) Pose(3,Mathf.Lerp(.25f,1,progress/.38f));
            else Pose(2,(progress-.38f)/.62f);
        }
        else Pose(moving || flying ? 1 : 0, Mathf.Repeat(phase,1));
        if (hitTime > 0)
        {
            hitTime -= Time.deltaTime;
            if(hitTime<.07f && flashing) Flash(false);
            if (remaining < 0) Pose(4,1-hitTime/.14f);
        }
    }

    public void Contact() { if(!dying) Pose(3,.25f); }

    public void Hit() { hitTime=.14f; Flash(true); }

    void Flash(bool on)
    {
        if(flashing==on)return;
        flashing=on;
        for(int i=0;i<surfaces.Length;i++)
        {
            if(on)
            {
                if(original[i]==null)original[i]=new MaterialPropertyBlock();
                surfaces[i].GetPropertyBlock(original[i]);
                tint.Clear();tint.SetColor(BaseColor,Color.white);tint.SetColor(ColorId,Color.white);
                surfaces[i].SetPropertyBlock(tint);
            }
            else surfaces[i].SetPropertyBlock(original[i]);
        }
    }

    public void Accent(Color accent)
    {
        foreach (var surface in surfaces)
        {
            var mats=surface.sharedMaterials;
            for(int i=0;i<mats.Length;i++)
            {
                var m=mats[i];
                if (!m || !(m.name.StartsWith("brass") || m.name.StartsWith("mist"))) continue;
                tint.Clear(); tint.SetColor(BaseColor,accent); tint.SetColor(ColorId,accent);
                surface.SetPropertyBlock(tint,i);
            }
        }
    }

    public void Fall()
    {
        transform.SetParent(null,true);
        Flash(false);
        dying=true;
    }

    void Update()
    {
        if (!dying) return;
        deathTime += GameManager.EffectDelta;
        Pose(5,deathTime/.42f);
        if (flying) transform.position += Vector3.down*GameManager.EffectDelta*1.4f;
        if (deathTime>=.46f) Destroy(gameObject);
    }
}
