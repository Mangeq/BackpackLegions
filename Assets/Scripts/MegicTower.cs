using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegicTower : Tower
{
    public GameObject MegicBulletP;

    public override void Attack(GameObject target)
    {
        GameObject bullet = Instantiate(MegicBulletP, transform.position, Quaternion.identity);
        // Initialize() eksikti - finalDamage/finalCriticalChance hiç set edilmiyordu,
        // yani bu kule şu ana kadar her zaman 0 hasar veriyordu.
        bullet.GetComponent<Bullet>().Initialize(target.transform, currentDamage, currentCriticalChance, this);
    }
}