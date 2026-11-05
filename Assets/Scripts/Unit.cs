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
    internal float attackSpeed = 1f;  // Saniyedeki saldýrý sayýsý, kuleden gelir.
    private int baseHealth;              // Prefab'daki ham can, item bonusu bunun uzerine uygulanir
    private int maxHealth;

    private Coroutine combatRoutine;
    private LegionFigure figure;
    private float contactTime, interval;

    public bool IsIdle => currentEnemy == null && !marching;
    private bool marching;
    private Vector3 guardPosition;
    public float marchSpeed = 3f;
    private void Update()
    {
        if (figure)
            figure.Tick(marching, currentEnemy ? currentEnemy.transform.position-transform.position : guardPosition-transform.position,
                marchSpeed,currentEnemy ? contactTime-Time.time : -1,interval);
        if (!marching) return;
        transform.position = Vector3.MoveTowards(transform.position, guardPosition, marchSpeed * Time.deltaTime);
        if ((transform.position - guardPosition).sqrMagnitude < .0025f) marching = false;
    }
    public int CurrentHealth => Mathf.Max(0, health);
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        figure=LegionFigure.Attach(transform,null,Resources.Load<GameObject>("BlackpineLegion/Sentinel"),.78f,false);
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

    // Kuleden doðan birlik, yol üzerindeki nöbet noktasýna yürür.
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
            interval=1f/attackSpeed;
            contactTime=Time.time+interval;
            yield return new WaitForSeconds(interval);
            if (currentEnemy==null) break;
            if(figure)figure.Contact();
            GameAudio.Play("SFX_02", .25f, true, .1f);
            bool critical=Random.Range(0f,1f)<=critChance;
            if(critical)LegionFx.Slash(transform.position,currentEnemy.transform.position);
            float finalDamage = critical ? damage * critMultiplier : damage;
            currentEnemy.GetComponent<Enemy>().TakeDamage(finalDamage, elementType, myTower != null ? myTower.GetComponent<Tower>() : null, critical);
            if (currentEnemy==null) break;
        }
        combatRoutine = null;
    }
    public void TakeDamage(int damage)
    {
        if(figure) figure.Hit();
        LegionFx.Burst(figure?figure.Center:transform.position,ElementType.Normal);
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
            if(figure) figure.Fall();
            myTower.GetComponent<UnitTower>().ReleaseUnit(unit);
            Destroy(unit);
        }
    }
}
