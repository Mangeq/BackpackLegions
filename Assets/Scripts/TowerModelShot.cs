using UnityEngine;

// References are assigned on the battle prefabs to the moving parts of their authored models.
public sealed class TowerModelShot : MonoBehaviour
{
    public Transform bowPivot, bolt;
    public SkinnedMeshRenderer bow;
    public Vector3 boltTravel;
    public MeshRenderer crystal;

    Quaternion restRotation;
    Vector3 boltDrawn, restForward, crystalSize;
    Renderer[] boltParts;
    Material gem;
    Color gemColor;
    Enemy target;
    float elapsed, duration;
    bool charging, active;

    void Awake()
    {
        if (bow)
        {
            restRotation=bowPivot.localRotation;
            boltDrawn=bolt.localPosition;
            restForward=bowPivot.parent.InverseTransformDirection(bowPivot.TransformVector(boltTravel)).normalized;
            // boltTravel points from drawn to relaxed, along the direction of fire.
            boltParts=bolt.GetComponentsInChildren<Renderer>();
            SetBow(0,true);
        }
        if (crystal)
        {
            crystalSize=crystal.transform.localScale;
            gem=crystal.material;
            gemColor=gem.GetColor("_BaseColor");
            gem.EnableKeyword("_EMISSION");
            SetGem(0);
        }
    }

    public void Charge(Enemy enemy,float seconds)
    {
        target=enemy;duration=seconds;elapsed=0;charging=true;active=true;
        Aim();
        if(bow)SetBow(0,true);
    }

    public Vector3 Fire(Enemy enemy)
    {
        target=enemy;Aim();
        charging=false;active=true;elapsed=0;duration=.19f;
        if(bow)
        {
            SetBow(1,true);
            var origin=boltParts[0].bounds.center;
            SetBow(1,false);
            return origin;
        }
        SetGem(1);
        return crystal.bounds.center;
    }

    void Update()
    {
        if(!active)return;
        elapsed+=GameManager.EffectDelta;
        float t=Mathf.Clamp01(elapsed/Mathf.Max(.01f,duration));
        if(charging)
        {
            if(!target || target.CurrentHealth<=0 || elapsed>duration+.12f){Rest();return;}
            Aim();
            if(bow)SetBow(Mathf.SmoothStep(0,1,t),true);
            else SetGem(t);
        }
        else
        {
            if(bow)SetBow(1-Mathf.Clamp01(t*3),false);
            else SetGem(1-t);
            if(t>=1)Rest();
        }
    }
    void Aim()
    {
        if(!bow || !target)return;
        Vector3 d=Vector3.ProjectOnPlane(target.AimPosition-bowPivot.position,Vector3.up);
        if(d.sqrMagnitude<.0001f)return;
        Vector3 forward=bowPivot.parent.TransformDirection(restForward);
        float angle=Vector3.SignedAngle(forward,d,Vector3.up);
        bowPivot.rotation=Quaternion.AngleAxis(angle,Vector3.up)*bowPivot.parent.rotation*restRotation;
    }
    void SetBow(float draw,bool boltVisible)
    {
        bow.SetBlendShapeWeight(0,100*draw);
        bolt.localPosition=boltDrawn+boltTravel*(1-draw);
        foreach(var part in boltParts)part.enabled=boltVisible;
    }
    void SetGem(float power)
    {
        crystal.transform.localScale=crystalSize*(1+.19f*power);
        gem.SetColor("_BaseColor",Color.Lerp(gemColor,new Color(1,.88f,1),power));
        gem.SetColor("_EmissionColor",Color.Lerp(gemColor*.22f,new Color(3.4f,1.6f,4.4f),power*power));
    }
    void Rest()
    {
        active=false;
        if(bow && boltParts!=null)SetBow(0,true);
        if(gem)SetGem(0);
    }
    void OnDisable(){Rest();}
    void OnDestroy(){if(gem)Destroy(gem);}
}
