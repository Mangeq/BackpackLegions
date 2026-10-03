using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    public TowerData data;

    private List<GameObject> enemies = new List<GameObject>();
    private GameObject targetEnemy;
    private bool isEnemyInRange = false;
    private bool isShooting = false;
    private SphereCollider rangeCollider;

    public ItemData[] myShopItems = new ItemData[4];

    // TowerData'dan başlangıçta kopyalanır; yükseltmeler yalnız bu kuleyi etkiler.
    internal int towerLevel;
    internal float baseDamage;
    internal float baseCriticalChance;
    internal float baseAttackSpeed;

    internal float currentDamage;
    internal float currentCriticalChance;
    internal float currentAttackSpeed;
    internal float currentHealthBonus; // Sadece birlik üreten kuleler kullanır (birlik canına % bonus)

    private ShopUIManager shopUIManager;

    /// <summary>Bu kulenin backpack'i. Her kule kendi instance'�n� Start()'ta bir kez olu�turur.</summary>
    public Backpack Backpack { get; private set; }

    private void Awake()
    {
        rangeCollider = GetComponent<SphereCollider>();
    }

    protected virtual void Start()
    {
        InitializeBaseStats();
        shopUIManager = GameObject.FindObjectOfType<ShopUIManager>();

        if (rangeCollider != null)
            rangeCollider.radius = data.range;

        Backpack = new Backpack(data.gridWidth, data.gridHeight);

        // Varsay�lan item, olu�turuldu�u andan itibaren backpack'in merkezine yerle�ir.
        // Bu TEK bir yerde, TEK bir kez olur - art�k ba�ka hi�bir script (�rn. backpack
        // paneli a��l�nca) bunu tekrar tetiklemiyor, bu y�zden eski "ayn� silah iki kez
        // eklendi" bug'� mimari olarak imkans�z hale geldi.
        if (data.defaultWeapon != null)
        {
            Vector2Int center = Backpack.GetCenterOrigin(data.defaultWeapon);
            Backpack.Place(data.defaultWeapon, center);
        }

        CalculateStats();

        // Dükkan kule kurulduğunda bir kez üretilir; sonrasında sadece reroll ile yenilenir.
        GenerateShopItems();
    }

    private void InitializeBaseStats()
    {
        // Eski asset'lerde seviye 0 olabilir; dükkan seviyeleri 1'den başlar.
        towerLevel = Mathf.Max(1, data.towerLevel);
        baseDamage = data.damage;
        baseCriticalChance = data.criticalChance;
        baseAttackSpeed = data.attackSpeed;
    }

    protected virtual void Update()
    {
        if (isEnemyInRange && !isShooting)
            StartCoroutine(Shoot());
    }

    public void GenerateShopItems()
    {
        for (int i = 0; i < myShopItems.Length; i++)
        {
            ItemRarity rarity = ShopManager.Instance.GetRandomRarity(towerLevel);
            myShopItems[i] = ShopManager.Instance.GetRandomItem(data.type, rarity);
        }
    }

    public void UpgradeTower()
    {
        towerLevel++;
        baseDamage += 5f;
        baseCriticalChance += 0.05f;
        baseAttackSpeed -= 0.1f;
        // Not: data.damage / data.attackSpeed / data.towerLevel'a yazma sat�rlar�
        // kald�r�ld� - data payla��lan bir ScriptableObject asset'i, oraya yazmak
        // ayn� TowerData'y� kullanan T�M kuleleri etkilerdi. Instance'a �zel stat
        // zaten yukar�daki alanlarda (baseDamage, towerLevel vb.) tutuluyor.
        CalculateStats();
    }

    public abstract void Attack(GameObject target);

    public virtual void CalculateStats()
    {
        // Item bonusları kesir (0.05 = %5) ve kendi aralarında toplanır.
        float damageBonus = 0f;
        float criticalChanceBonus = 0f;
        float attackSpeedBonus = 0f;
        float healthBonus = 0f;

        if (Backpack != null)
        {
            foreach (PlacedItem placed in Backpack.Items)
            {
                damageBonus += placed.Data.damageBonus;
                criticalChanceBonus += placed.Data.criticalChanceBonus;
                attackSpeedBonus += placed.Data.attackSpeedBonus;
                healthBonus += placed.Data.healthBonus;
            }
        }

        currentHealthBonus = healthBonus;
        currentDamage = baseDamage * (1f + damageBonus);
        currentCriticalChance = Mathf.Clamp01(baseCriticalChance + criticalChanceBonus);
        // currentAttackSpeed bir "atışlar arası bekleme süresi" (cooldown). %X hızlanma
        // = süreyi (1 + X)'e bölmek; böylece bonuslar ne kadar birikirse biriksin süre sıfıra inmez.
        currentAttackSpeed = baseAttackSpeed / (1f + attackSpeedBonus);

        // �ok fazla attackSpeedBonus �st �ste binerse s�re s�f�ra/negatife d���p
        // Shoot() coroutine'inde WaitForSeconds'� bozmas�n diye alt s�n�r.
        currentAttackSpeed = Mathf.Max(0.1f, currentAttackSpeed);
    }

    private void OnMouseDown()
    {
        UİManager.activeTower = this;
        ShopUIManager.ItemShopUI.SetActive(true);
        shopUIManager.UpdateShopUI();
        BackpackUIManager.Instance.OpenBackpack(this);
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        while (enemies.Count > 0)
        {
            targetEnemy = enemies[0];
            if (targetEnemy == null)
            {
                enemies.RemoveAt(0);
                continue;
            }

            Enemy enemyScript = targetEnemy.GetComponent<Enemy>();
            if (enemyScript != null)
                Attack(targetEnemy);

            yield return new WaitForSeconds(currentAttackSpeed);
        }
        isShooting = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            isEnemyInRange = true;
            enemies.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemies.Remove(other.gameObject);
            if (enemies.Count == 0)
                isEnemyInRange = false;
        }
    }
}
