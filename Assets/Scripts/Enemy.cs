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
    private LegionFigure figure;
    // Kale temasý ayný karedeki mermilerden önce ölüm yolunu kapatýr.
    public bool TryReachBase()
    {
        if (isDead) return false;
        isDead = true;
        Destroy(gameObject);
        return true;
    }
    [Tooltip("Uçan prefabýn görsel kökü. Yol takibi ve kale temasý ana objede kalýr.")]
    public Transform visualRoot;
    public bool IsAir => enemyData != null && enemyData.movementType == EnemyData.MovementType.Air;
    public Vector3 AimPosition => figure ? figure.Center : IsAir && visualRoot != null ? visualRoot.position : transform.position;
    // UI caný okuyabilir; savaþ verisini deðiþtiremez.
    public float CurrentHealth => Mathf.Max(0f, currenthealth);
    public float MaxHealth => enemyData != null ? enemyData.health : 0f;
    internal bool isEngaged = false; // Bir birlik tarafindan hedeflenip savasiliyor mu (baska birliklerin ayni dusmani secmesini onler)
    void Start()
    {
       

        agent = GetComponent<NavMeshAgent>();   
        damage = enemyData.damage;  
        speed = enemyData.speed;
        currenthealth = enemyData.health;
        // Per-instance tint: shared materials and other enemies remain untouched.
        float size = enemyData.isBoss ? .93f : IsAir ? .62f : Mathf.Clamp(transform.lossyScale.x*2.02f,.34f,1.27f);
        figure=LegionFigure.Attach(transform,IsAir?visualRoot:null,enemyData.characterModel,size,IsAir);
        if (figure && enemyData.element!=ElementType.Normal) figure.Accent(LegionFx.Hue(enemyData.element));
        if (!figure && enemyData.element != ElementType.Normal)
        {
            Color tint = enemyData.element == ElementType.Fire ? new Color(1f,.35f,.16f)
                : enemyData.element == ElementType.Water ? new Color(.2f,.65f,1f)
                : enemyData.element == ElementType.Earth ? new Color(.65f,.48f,.23f)
                : new Color(.55f,.90f,.77f);
            var block = new MaterialPropertyBlock();
            foreach(var renderer in GetComponentsInChildren<Renderer>())
            {
                var material=renderer.sharedMaterial;
                if(material==null)continue;
                renderer.GetPropertyBlock(block);
                foreach(var property in new[]{"_BaseColor","_Color"})
                    if(material.HasProperty(property)) block.SetColor(property,Color.Lerp(material.GetColor(property),tint,.38f));
                renderer.SetPropertyBlock(block);block.Clear();
            }
        }
        if (IsAir)
        {
            if (visualRoot != null) visualRoot.position += Vector3.up * enemyData.flightHeight;
            // Yol üzerinde ilerler, fakat yerdeki asker/düþmanlardan kaçýnmak için beklemez.
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }
        agent.SetDestination(GameObject.FindWithTag("Base").transform.position);
        agent.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        if (figure) figure.Tick(!isEngaged, currentUnit ? currentUnit.transform.position-transform.position : agent.velocity,
            speed,isEngaged ? attackTimer : -1,enemyData.attackInterval);
        if (!isEngaged) return;
        if (currentUnit == null)
        {
            ResumeMoving();
            return;
        }

        // Saldýrý ritmi askerin hýzýndan baðýmsýzdýr; pause ile birlikte durur.
        attackTimer -= Time.deltaTime;
        if (attackTimer > 0f) return;
        attackTimer = Mathf.Max(0.1f, enemyData.attackInterval);
        Unit unit = currentUnit.GetComponent<Unit>();
        if (unit != null && unit.health > 0) { if(figure)figure.Contact(); unit.TakeDamage(damage); }
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
            // Ýlk vuruþ da düþmanýn kendi saldýrý süresini bekler.
            attackTimer = Mathf.Max(0.1f, enemyData.attackInterval);
        }
    }
    public void TakeDamage(float damageAmount, ElementType elementType, Tower owner = null, bool critical = false)
    {
        if (isDead || damageAmount <= 0f || float.IsNaN(damageAmount) || float.IsInfinity(damageAmount)) return;
        float multiplier = enemyData.GetDamageMultiplier(elementType);
        float actualDamage = Mathf.Min(CurrentHealth, damageAmount * multiplier);
        if(figure) figure.Hit();
        LegionFx.Burst(AimPosition,elementType,critical);
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
        if(figure) figure.Fall();
        EconomyManager.AddGold(enemyData.goldReward);
        Destroy(gameObject);
    }
}
