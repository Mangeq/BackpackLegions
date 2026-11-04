using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public GameObject myTower;
    public int health = 100;
    public float damage;
    public float critMultiplier = 1.5f; // Kritik vurusta hasar carpani
    public float engageRange = 0.8f; // Bu mesafedeki bos dusmani yakalar
    internal GameObject currentEnemy = null;
    public ElementType elementType;

    internal float critChance;           // Kuleden gelir (base + item bonuslari)
    internal float attackSpeed = 1f;  // Saniyedeki saldırı sayısı, kuleden gelir.
    private int baseHealth;              // Prefab'daki ham can, item bonusu bunun uzerine uygulanir
    private int maxHealth;

    private Coroutine combatRoutine;

    public bool IsIdle => currentEnemy == null && !marching;
    private bool marching;
    private Vector3 guardPosition;
    public float marchSpeed = 3f;
    private void Update()
    {
        if (!marching) return;
        transform.position = Vector3.MoveTowards(transform.position, guardPosition, marchSpeed * Time.deltaTime);
        if ((transform.position - guardPosition).sqrMagnitude < .0025f) marching = false;
    }
    public int CurrentHealth => Mathf.Max(0, health);
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        baseHealth = health;
        maxHealth = health;
    }

    // Kule, backpack'i her degistiginde ve birlik dogdugunda bunu cagirir.
    public void ApplyStats(float damage, float critChance, float attackSpeed, float healthMultiplier)
    {
        this.damage = damage;
        this.critChance = critChance;
        this.attackSpeed = Mathf.Max(.01f, attackSpeed);

        // Max can degisince mevcut can da ayni oranda olceklenir (yarim canli birlik yarim canli kalir).
        int newMaxHealth = Mathf.RoundToInt(baseHealth * healthMultiplier);
        health = Mathf.RoundToInt(health * (float)newMaxHealth / maxHealth);
        maxHealth = newMaxHealth;
    }

    // Kuleden doğan birlik, yol üzerindeki nöbet noktasına yürür.
    public void PlaceAt(Vector3 position)
    {
        guardPosition = position;
        marching = (transform.position - position).sqrMagnitude > .0025f;
    }

    // Kule bu birligi bos buldugunda yakinindaki bir dusmani tutmasi icin cagirir
    public void Engage(GameObject enemy)
    {
        if (!IsIdle || enemy == null) return;

        Enemy enemyComp = enemy.GetComponent<Enemy>();
        if (enemyComp == null || enemyComp.IsAir || enemyComp.isEngaged) return;

        currentEnemy = enemy;
        enemyComp.AttackTarget(gameObject);
        combatRoutine = StartCoroutine(Combat());
    }

    IEnumerator Combat()
    {
        while(currentEnemy != null)
        {
            yield return new WaitForSeconds(1f / attackSpeed);
            if (currentEnemy==null) break;
            GameAudio.Play("SFX_02", .25f, true, .1f);
            float finalDamage = Random.Range(0f, 1f) <= critChance ? damage * critMultiplier : damage;
            currentEnemy.GetComponent<Enemy>().TakeDamage(finalDamage, elementType, myTower != null ? myTower.GetComponent<Tower>() : null);
            if (currentEnemy==null) break;
        }
        combatRoutine = null;
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die(gameObject);
        }

    }
    public void Die(GameObject unit)
    {
        if(health <= 0)
        {
            if (currentEnemy != null)
            {
                Enemy enemyComp = currentEnemy.GetComponent<Enemy>();
                if (enemyComp != null)
                {
                    enemyComp.ResumeMoving();
                    enemyComp.isEngaged = false;
                }
            }
            if (combatRoutine != null)
            {
                StopCoroutine(combatRoutine);
            }
            myTower.GetComponent<UnitTower>().ReleaseUnit(unit);
            Destroy(unit);
        }
    }
}
