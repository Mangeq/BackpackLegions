using UnityEngine;
using System.Collections.Generic;

// Hedef sırası, menzil, çanta ve yükseltme ortak Tower sınıfında kalır.
public class MortarTower : Tower
{
    public MortarProjectile projectilePrefab;
    public Transform muzzle;
    private readonly List<MortarProjectile> projectiles = new List<MortarProjectile>(8);

    protected override void Start()
    {
        base.Start();
        // Hazırlıkta ısıtılır; normal savaş akışında Instantiate/Destroy yapılmaz.
        if (projectilePrefab != null)
            while (projectiles.Count < 8) CreateProjectile();
    }

    private MortarProjectile CreateProjectile()
    {
        var projectile = Instantiate(projectilePrefab, transform);
        projectile.PrepareForReuse();
        projectiles.Add(projectile);
        return projectile;
    }

    public override void Attack(GameObject target)
    {
        var enemy = target != null ? target.GetComponent<Enemy>() : null;
        if (enemy == null || enemy.IsAir || enemy.CurrentHealth <= 0f || projectilePrefab == null) return;
        Vector3 origin = muzzle != null ? muzzle.position : transform.position + Vector3.up;
        MortarProjectile projectile = null;
        foreach (var candidate in projectiles)
            if (candidate != null && !candidate.gameObject.activeSelf) { projectile = candidate; break; }
        // Beklenmedik eşzamanlı yükte atış kaybetme; havuz yalnız gerektiğinde büyür.
        if (projectile == null) projectile = CreateProjectile();
        projectile.transform.SetPositionAndRotation(origin, Quaternion.identity);
        projectile.gameObject.SetActive(true);
        // Atış anının konumu ve statları sabitlenir; mermi sonradan hedefi takip etmez.
        projectile.Initialize(enemy.transform.position, currentDamage, currentCriticalChance, this);
        GameAudio.Play("cannon_tower_attack", .5f, true, .12f);
    }
}
