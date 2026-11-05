using UnityEngine;

// A separate world-space weapon overlay leaves tower pivots, triggers and pooled shells untouched.
public sealed class LegionWeapon : MonoBehaviour
{
    Tower tower;
    LineRenderer limb, stringLine;
    GameObject weapon;
    Transform barrel;
    Vector3 restBarrel, localOrigin;
    Enemy target;
    float since, period;
    bool drawing;
    public static float Prepare(Tower tower,GameObject target,float interval)
    {
        if(!(tower is ArcherTower || tower is MegicTower))return 0;
        float duration=Mathf.Min(.36f,interval*.42f);
        var rig=tower.GetComponent<TowerModelShot>();
        if(rig){rig.Charge(target.GetComponent<Enemy>(),duration);return duration;}
        Show(tower,target,duration,true);
        return duration;
    }
    public static Vector3 Release(Tower tower,GameObject target)
    {
        var rig=tower.GetComponent<TowerModelShot>();
        if(rig)return rig.Fire(target.GetComponent<Enemy>());
        return Show(tower,target,1f/tower.currentAttackSpeed,false);
    }
    static Vector3 Show(Tower tower,GameObject target,float duration,bool drawing)
    {
        var enemy=target?target.GetComponent<Enemy>():null;
        if(!enemy)return tower.transform.position;
        var fx=tower.GetComponent<LegionWeapon>();
        if(!fx)fx=tower.gameObject.AddComponent<LegionWeapon>();
        fx.tower=tower;fx.target=enemy;fx.since=0;fx.period=duration;fx.drawing=drawing;
        if(!fx.weapon)fx.Build();
        fx.weapon.SetActive(true);fx.Draw();
        return fx.weapon.transform.position;
    }
    void Build()
    {
        float top=transform.position.y;
        foreach(var mesh in GetComponentsInChildren<MeshRenderer>())
        {
            if(mesh.GetComponentInParent<MortarProjectile>())continue;
            top=Mathf.Max(top,mesh.bounds.max.y);
            if(tower is MortarTower && mesh.name=="barrel")
            {
                barrel=mesh.transform;
                restBarrel=barrel.localPosition;
            }
        }
        localOrigin=transform.InverseTransformPoint(new Vector3(transform.position.x,top+.025f,transform.position.z));
        weapon=new GameObject("Weapon motion");
        weapon.transform.SetParent(transform,false);
        var s=transform.lossyScale;
        weapon.transform.localScale=new Vector3(1/s.x,1/s.y,1/s.z);
        if(tower is ArcherTower)
        {
            limb=LegionFx.Stroke(weapon.transform,new Color(.80f,.54f,.25f),.035f,5);
            stringLine=LegionFx.Stroke(weapon.transform,LegionFx.Gold,.012f,3);
        }
        else if(!barrel)limb=LegionFx.Stroke(weapon.transform,tower is MortarTower?new Color(.36f,.31f,.24f):new Color(.69f,.49f,1),tower is MortarTower?.16f:.045f,tower is MortarTower?2:17);
    }
    void Update()
    {
        if(!weapon)return;
        if(!weapon.activeSelf)return;
        since+=Time.deltaTime;
        if(!target || since>period+(drawing?.1f:0)){ResetWeapon();return;}
        Draw();
    }
    void OnDisable(){ResetWeapon();}
    void ResetWeapon()
    {
        if(weapon)weapon.SetActive(false);
        if(barrel)barrel.localPosition=restBarrel;
    }
    void Draw()
    {
        var mortar=tower as MortarTower;
        var origin=mortar&&mortar.muzzle?mortar.muzzle.position:transform.TransformPoint(localOrigin);
        weapon.transform.position=origin;
        var direction=target.AimPosition-origin;
        if(direction.sqrMagnitude>.0001f)weapon.transform.rotation=Quaternion.LookRotation(direction);
        float phase=drawing?Mathf.Clamp01(since/period):0;
        if(stringLine)
        {
            for(int i=0;i<5;i++){float x=(i-2)/2f;limb.SetPosition(i,new Vector3(x*.28f,.02f,.12f*(1-x*x)));}
            stringLine.SetPosition(0,limb.GetPosition(0));stringLine.SetPosition(2,limb.GetPosition(4));
            stringLine.SetPosition(1,Vector3.back*(.03f+.20f*phase));
        }
        else if(mortar)
        {
            float recoil=(1-Mathf.Clamp01(since/.16f))*.16f;
            if(barrel) barrel.localPosition=restBarrel+barrel.parent.InverseTransformVector(-weapon.transform.forward*recoil);
            else
            {
                limb.SetPosition(0,Vector3.back*(.38f+recoil));
                limb.SetPosition(1,Vector3.back*recoil);
            }
        }
        else
        {
            float radius=.04f+.14f*phase;
            for(int i=0;i<17;i++){float a=i*Mathf.PI/8;limb.SetPosition(i,new Vector3(Mathf.Cos(a),Mathf.Sin(a),0)*radius);}
        }
    }
}
