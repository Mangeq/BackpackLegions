using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int damage;
    public float speed;
    public EnemyData enemyData;
    private Transform target; 
    public GameObject currentUnit;
    private float currenthealth;
    private NavMeshAgent agent;
    private float attackTimer;
    private bool isDead;
    private readonly Dictionary<Tower, float> damageContributions = new Dictionary<Tower, float>();
    private float totalDamageReceived;

    // Kale teması aynı karedeki mermilerden önce ölüm/XP yolunu kapatır.
    public bool TryReachBase()
    {
        if (isDead) return false;
        isDead = true;
        Destroy(gameObject);
        return true;
    }
    [Tooltip("Uçan prefabın görsel kökü. Yol takibi ve kale teması ana objede kalır.")]
    public Transform visualRoot;
    public bool IsAir => enemyData != null && enemyData.movementType == EnemyData.MovementType.Air;
    public Vector3 AimPosition => IsAir && visualRoot != null ? visualRoot.position : transform.position;
    // UI canı okuyabilir; savaş verisini değiştiremez.
    public float CurrentHealth => Mathf.Max(0f, currenthealth);
    public float MaxHealth => enemyData != null ? enemyData.health : 0f;
    internal bool isEngaged = false; // Bir birlik tarafindan hedeflenip savasiliyor mu (baska birliklerin ayni dusmani secmesini onler)
    void Start()
    {
       

        agent = GetComponent<NavMeshAgent>();   
        damage = enemyData.damage;  
        speed = enemyData.speed;
        currenthealth = enemyData.health;
        if (IsAir)
        {
            if (visualRoot != null) visualRoot.position += Vector3.up * enemyData.flightHeight;
            // Yol üzerinde ilerler, fakat yerdeki asker/düşmanlardan kaçınmak için beklemez.
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }
        agent.SetDestination(GameObject.FindWithTag("Base").transform.position);
        agent.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead || !isEngaged) return;
        if (currentUnit == null)
        {
            ResumeMoving();
            return;
        }

        // Saldırı ritmi askerin hızından bağımsızdır; pause ile birlikte durur.
        attackTimer -= Time.deltaTime;
        if (attackTimer > 0f) return;
        attackTimer = Mathf.Max(0.1f, enemyData.attackInterval);
        Unit unit = currentUnit.GetComponent<Unit>();
        if (unit != null && unit.health > 0) unit.TakeDamage(damage);
        else ResumeMoving();
    }

    public void StopMoving()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
    }

    public void ResumeMoving()
    {
        currentUnit = null;
        isEngaged = false;
        attackTimer = 0f;
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }

    public void AttackTarget(GameObject unit)
    {
        if (!IsAir && !isDead && unit != null && unit.CompareTag("Unit") && currentUnit != unit)
        {
            currentUnit = unit;
            isEngaged = true;
            StopMoving();
            // İlk vuruş da düşmanın kendi saldırı süresini bekler.
            attackTimer = Mathf.Max(0.1f, enemyData.attackInterval);
        }
    }
    public void TakeDamage(float damageAmount, ElementType elementType, Tower owner = null)
    {
        if (isDead || damageAmount <= 0f || float.IsNaN(damageAmount) || float.IsInfinity(damageAmount)) return;
        float multiplier = enemyData.GetDamageMultiplier(elementType);
        float actualDamage = Mathf.Min(CurrentHealth, damageAmount * multiplier);
        totalDamageReceived += actualDamage;
        if (owner != null)
        {
            damageContributions.TryGetValue(owner, out float previous);
            damageContributions[owner] = previous + actualDamage;
        }
        currenthealth -= actualDamage;
        if (currenthealth <= 0f) Die();
    }

    private void Die()
    {
        isDead = true;
        if (currentUnit != null)
        {
            currentUnit.GetComponent<Unit>().currentEnemy = null;
        }
        EconomyManager.AddGold(enemyData.goldReward);
        if (totalDamageReceived > 0f)
            foreach (var contribution in damageContributions)
                if (contribution.Key != null)
                    contribution.Key.AddExperience(enemyData.experienceReward * (contribution.Value / totalDamageReceived));
        Destroy(gameObject);
    }
}
