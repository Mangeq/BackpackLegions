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
    private ElementType damageElement;

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
                // bulletData.Damage kaldırıldı - finalDamage zaten Tower'dan gelen
                // nihai (base + item bonusları) hasar. Kritikte sadece bu değer
                // critMultiplier ile çarpılıyor, ikinci bir çarpan yok.
                float damage = Random.Range(0f, 1f) <= finalCriticalChance
                    ? finalDamage * bulletData.critMultiplier
                    : finalDamage;
                target.GetComponent<Enemy>().TakeDamage(damage, damageElement, owner);
            }
            Destroy(gameObject); return;
        }
    }

    public void Initialize(Transform target, float calculateDamage, float calculateCriticalChance, Tower owner = null)
    {
        this.owner = owner;
        damageElement = owner != null && owner.SelectedElement != ElementType.Normal
            ? owner.SelectedElement : bulletData.bulletType;
        if (owner != null && owner.SelectedElement != ElementType.Normal)
        {
            // Ortak materyali değiştirme: bu merminin rengi yalnız bu atışa aittir.
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
        this.target = target;
        targetEnemy = target != null ? target.GetComponent<Enemy>() : null;
        targetLastPosition = targetEnemy != null ? targetEnemy.AimPosition : target != null ? target.position : transform.position;
        this.finalDamage = calculateDamage;
        this.finalCriticalChance = calculateCriticalChance;
    }
}
