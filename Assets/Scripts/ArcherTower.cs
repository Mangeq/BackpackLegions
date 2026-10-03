using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTower : Tower
{
    public GameObject ArcherBulletP;
    public float arcDamage;

    public override void Attack(GameObject target)
    {
        
        GameObject bullet = Instantiate(ArcherBulletP, transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Initialize(target.transform, currentDamage, currentCriticalChance);
    }
}
