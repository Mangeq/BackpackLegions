using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletData bulletData;
    internal Transform target;

    private float finalDamage;
    private float finalCriticalChance;
    private Vector3 targetLastPosition;
    private Enemy targetEnemy;
    private Tower owner;
    private int archerSequenceVersion;
    private ElementType damageElement;
    private Transform projectileArt;

    // Update is called once per frame
    void Update()
    {

        Vector3 currentBulletTarget;
        if (target == null)
        {
            currentBulletTarget = targetLastPosition;
        }
        else
        {
            currentBulletTarget = targetEnemy != null ? targetEnemy.AimPosition : target.position;
            targetLastPosition = currentBulletTarget;
        }

        var direction=currentBulletTarget-transform.position;
        if(projectileArt && direction.sqrMagnitude>.00001f)
            projectileArt.rotation=Quaternion.LookRotation(direction);
        transform.position = Vector3.MoveTowards(transform.position, currentBulletTarget, bulletData.Speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, currentBulletTarget) < 0.1f)
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                // bulletData.Damage kaldýrýldý - finalDamage zaten Tower'dan gelen
                // nihai (base + item bonuslarý) hasar. Kritikte sadece bu deðer
                // critMultiplier ile çarpýlýyor, ikinci bir çarpan yok.
                bool critical=Random.Range(0f,1f)<=finalCriticalChance;
                float damage = critical
                    ? finalDamage * bulletData.critMultiplier
                    : finalDamage;
                if(targetEnemy!=null && targetEnemy.CurrentHealth>0f)
                {
                    var archer=owner as ArcherTower;
                    if(archer!=null)archer.RegisterSkillHit(targetEnemy,archerSequenceVersion);
                    targetEnemy.TakeDamage(damage, damageElement, owner,critical);
                    if(owner is MegicTower) LegionFx.Ring(targetEnemy.AimPosition,.28f,LegionFx.Hue(damageElement));
                }
            }
            Destroy(gameObject); return;
        }
    }

    public void Initialize(Transform target, float calculateDamage, float calculateCriticalChance, Tower owner = null)
    {
        this.owner = owner;
        archerSequenceVersion=owner is ArcherTower ? ((ArcherTower)owner).SkillSequenceVersion : -1;
        damageElement = owner != null
            ? owner.SelectedElement : bulletData.bulletType;
        if(owner is ArcherTower || owner is MegicTower)
        {
            var asset=Resources.Load<GameObject>("BlackpineLegion/Effects/"+(owner is ArcherTower?"Arrow":"Spell"));
            if(asset)
            {
                foreach(var renderer in GetComponentsInChildren<MeshRenderer>()) renderer.enabled=false;
                projectileArt=Instantiate(asset,transform,false).transform;
                projectileArt.localScale*=owner is ArcherTower ? .92f : 1.14f;
            }
        }
        if (owner != null && owner.SelectedElement != ElementType.Normal)
        {
            // Ortak materyali deðiþtirme: bu merminin rengi yalnýz bu atýþa aittir.
            Color tint = damageElement == ElementType.Water ? new Color(0.2f, 0.65f, 1f)
                : damageElement == ElementType.Fire ? new Color(1f, 0.3f, 0.08f)
                : damageElement == ElementType.Earth ? new Color(0.65f, 0.43f, 0.2f)
                : new Color(0.65f, 0.95f, 0.9f);
            foreach (var renderer in GetComponentsInChildren<Renderer>())
            {
                var properties = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(properties);
                properties.SetColor("_Color", tint);
                properties.SetColor("_BaseColor", tint);
                renderer.SetPropertyBlock(properties);
            }
        }
        LegionFx.Trail(gameObject,owner is MegicTower && damageElement==ElementType.Normal ? new Color(.65f,.45f,1) : LegionFx.Hue(damageElement),owner is MegicTower?.052f:.022f);
        this.target = target;
        targetEnemy = target != null ? target.GetComponent<Enemy>() : null;
        targetLastPosition = targetEnemy != null ? targetEnemy.AimPosition : target != null ? target.position : transform.position;
        this.finalDamage = calculateDamage;
        this.finalCriticalChance = calculateCriticalChance;
    }
}
