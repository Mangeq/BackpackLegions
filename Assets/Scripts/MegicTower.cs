using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegicTower : Tower
{
    public GameObject MegicBulletP;

    public override void Attack(GameObject target)
    {
        if (target == null || MegicBulletP == null) return;
        Vector3 origin=LegionWeapon.Release(this,target);
        GameObject bullet = Instantiate(MegicBulletP, origin, Quaternion.identity);
        // Initialize() eksikti - finalDamage/finalCriticalChance hiç set edilmiyordu,
        // yani bu kule şu ana kadar her zaman 0 hasar veriyordu.
        bullet.GetComponent<Bullet>().Initialize(target.transform, currentDamage, currentCriticalChance, this);
        GameAudio.Play("magic_tower_attack", .35f, true, .09f);
    }
}