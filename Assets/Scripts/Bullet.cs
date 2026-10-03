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
            currentBulletTarget = target.position;
            targetLastPosition = target.position;
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
                target.GetComponent<Enemy>().TakeDamage(damage, bulletData.bulletType);
            }
            Destroy(gameObject); return;
        }
    }

    public void Initialize(Transform target, float calculateDamage, float calculateCriticalChance)
    {
        this.target = target;
        this.finalDamage = calculateDamage;
        this.finalCriticalChance = calculateCriticalChance;
    }
}