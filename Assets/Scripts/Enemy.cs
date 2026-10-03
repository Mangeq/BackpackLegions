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
    internal bool isEngaged = false; // Bir birlik tarafindan hedeflenip savasiliyor mu (baska birliklerin ayni dusmani secmesini onler)
    void Start()
    {
       

        agent = GetComponent<NavMeshAgent>();   
        damage = enemyData.damage;  
        speed = enemyData.speed;
        currenthealth = enemyData.health;
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
        if (!isDead && unit != null && unit.CompareTag("Unit") && currentUnit != unit)
        {
            currentUnit = unit;
            isEngaged = true;
            StopMoving();
            // İlk vuruş da düşmanın kendi saldırı süresini bekler.
            attackTimer = Mathf.Max(0.1f, enemyData.attackInterval);
        }
    }
    public void TakeDamage(float damageAmount, ElementType elementType)
    {
        if (isDead) return;
        float damageMultiplier = 1f;
        if (enemyData.weaknesses.Contains(elementType))
        {
            damageMultiplier = 1.5f; // Zay�fl�k durumunda hasar art�r�l�r
        }
        else if (enemyData.resistances.Contains(elementType))
        {
            damageMultiplier = 0.5f; // Diren� durumunda hasar azalt�l�r
        }
        currenthealth -= damageAmount * damageMultiplier;
        if (currenthealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        if (currentUnit != null)
        {
            currentUnit.GetComponent<Unit>().currentEnemy = null;
        }
        EconomyManager.AddGold(enemyData.goldReward);
        Destroy(gameObject);
    }
}
