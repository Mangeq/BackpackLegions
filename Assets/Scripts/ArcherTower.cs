using UnityEngine;

public class ArcherTower : Tower
{
    public GameObject ArcherBulletP;
    public float arcDamage;
    private GameObject sequenceTarget;
    private int attacksOnTarget, hitStacks;
    private float skillCooldown;
    public int SkillSequenceVersion { get; private set; }
    public int SkillHitStacks => hitStacks;
    protected override void OnDisable() { base.OnDisable();ResetSequence(); }
    protected override void RefreshSkillStats() { ResetSequence();base.RefreshSkillStats(); }
    public override void CalculateStats()
    {
        base.CalculateStats();skillCooldown=currentAttackSpeed;ApplyStackSpeed();
    }
    private void ApplyStackSpeed()
    {
        float bonus=0f;
        foreach(var effect in skillEffects)if(effect.type==SkillData.EffectType.SameTargetHitSpeed)bonus+=Mathf.Min(hitStacks,effect.count)*effect.value/100f;
        currentAttackSpeed=Mathf.Max(.1f,skillCooldown/(1f+bonus));
    }
    private void ResetSequence()
    {
        sequenceTarget=null;attacksOnTarget=0;hitStacks=0;SkillSequenceVersion++;
        if(skillCooldown>0f)ApplyStackSpeed();
    }
    protected override void OnTargetSelected(GameObject target)
    {
        if(target==sequenceTarget && target!=null)return;
        if(sequenceTarget!=null||attacksOnTarget>0||hitStacks>0)ResetSequence();
        sequenceTarget=target;
    }
    // The projectile reports actual hits, not launches. Old in-flight shots cannot add stacks after a target switch/reset.
    public void RegisterSkillHit(Enemy enemy,int sequenceVersion)
    {
        if(enemy==null||enemy.CurrentHealth<=0||sequenceVersion!=SkillSequenceVersion
            ||enemy.gameObject!=sequenceTarget)return;
        int cap=0;foreach(var effect in skillEffects)if(effect.type==SkillData.EffectType.SameTargetHitSpeed)cap=Mathf.Max(cap,effect.count);
        hitStacks=Mathf.Min(cap,hitStacks+1);ApplyStackSpeed();
    }
    public override void Attack(GameObject target)
    {
        if(target==null||ArcherBulletP==null)return;
        var enemy=target.GetComponent<Enemy>();if(enemy==null||enemy.CurrentHealth<=0)return;
        OnTargetSelected(target);attacksOnTarget++;
        float bonus=0f;bool guaranteed=false;
        foreach(var effect in skillEffects)
        {
            if(effect.type==SkillData.EffectType.OuterRangeDamage&&Vector3.Distance(transform.position,target.transform.position)>CurrentRange*effect.threshold/100f)bonus+=effect.value/100f;
            if(effect.type==SkillData.EffectType.LowHealthDamage&&enemy.CurrentHealth<enemy.MaxHealth*effect.threshold/100f)bonus+=effect.value/100f;
            if(effect.type==SkillData.EffectType.EveryNthAttackCritical&&attacksOnTarget%effect.count==0)guaranteed=true;
        }
        GameObject bullet=Instantiate(ArcherBulletP,transform.position,Quaternion.identity);
        bullet.GetComponent<Bullet>().Initialize(target.transform,currentDamage*(1f+bonus),guaranteed?1f:currentCriticalChance,this);
    }
}
