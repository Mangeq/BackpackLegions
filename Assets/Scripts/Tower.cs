using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    public TowerData data;
    private bool initialized;
    private bool retired;
    private bool startAtLevelOne;
    public bool IsItemActive(ItemData item) => item != null && data != null &&
        (item == MainItem || item.IsCompatibleWith(data.type, SelectedElement));
    public int ElementChangeCost => Mathf.Max(0, data.elementChangeCost);
    public int TypeChangeRefund => Mathf.FloorToInt(Mathf.Max(0, data.cost) * Mathf.Clamp01(data.typeChangeRefundRate));
    public string TransitionError { get; private set; }


    private List<GameObject> enemies = new List<GameObject>();
    // Bir düşmanda birden fazla collider bulunursa giriş sırası yine bir kez tutulur.
    private readonly Dictionary<Collider, GameObject> enemyColliders = new Dictionary<Collider, GameObject>();
    private readonly List<Collider> staleColliders = new List<Collider>();
    public enum TargetType { Both = 0, Ground = 1, Air = 2 }
    public enum TargetOrder { FirstEntered = 0, LastEntered = 1 }
    public TargetType SelectedTargetType { get; private set; } = TargetType.Both;
    public TargetOrder SelectedTargetOrder { get; private set; } = TargetOrder.FirstEntered;
    public bool SupportsManualTargeting => this is ArcherTower || this is MegicTower;

    public void SetTargetType(int value)
    {
        if (SupportsManualTargeting && value >= 0 && value <= 2)
            SelectedTargetType = (TargetType)value;
    }

    public void SetTargetOrder(int value)
    {
        if (SupportsManualTargeting && value >= 0 && value <= 1)
            SelectedTargetOrder = (TargetOrder)value;
    }
    private bool isShooting = false;
    private SphereCollider rangeCollider;

    public ItemData[] myShopItems = new ItemData[4];
    public bool IsShopFrozen { get; private set; }

    public void ToggleShopFreeze()
    {
        if (GameManager.CanEditItems) IsShopFrozen = !IsShopFrozen;
    }

    // TowerData'dan başlangıçta kopyalanır; yükseltmeler yalnız bu kuleyi etkiler.
    internal int towerLevel;
    public int Level => towerLevel;
    public float Experience { get; private set; }
    public ElementType SelectedElement { get; private set; } = ElementType.Normal;
    public ItemData MainItem => data != null ? data.GetMainItem(SelectedElement) : null;
    public string ElementSelectionError { get; private set; }
    public bool IsMainItem(ItemData item) => data != null && data.IsMainItem(item);
    public bool AwaitingElement => towerLevel >= 5 && SelectedElement == ElementType.Normal;
    public float ExperienceRequired => Mathf.Max(1f, data.firstLevelExperience
        + Mathf.Max(0, towerLevel - 1) * data.experienceIncreasePerLevel);

    // XP savaş sırasında gelir. Hazırlık kısıtı yalnız element seçimine aittir.
    public void AddExperience(float amount)
    {
        if (retired || amount <= 0f || float.IsNaN(amount) || float.IsInfinity(amount)) return;
        Experience += amount;
        ProcessExperience();
    }

    private void ProcessExperience()
    {
        while (!AwaitingElement && Experience >= ExperienceRequired)
        {
            Experience -= ExperienceRequired;
            towerLevel++;
            baseDamage += 5f;
            baseCriticalChance = Mathf.Clamp01(baseCriticalChance + 0.05f);
            baseAttackSpeed = Mathf.Max(0.1f, baseAttackSpeed - 0.1f);
            CalculateStats();
        }
    }

    public bool TrySelectElement(ElementType element) => ApplyElement(element, false);

    public string GetElementChangeError(ElementType element)
    {
        if (retired || !GameManager.CanUpgradeTowers) return "Dönüşüm yalnız hazırlıkta yapılabilir.";
        if (SelectedElement == ElementType.Normal) return "Önce 5. seviyede ilk elementini seç.";
        if (element < ElementType.Fire || element > ElementType.Air || element == SelectedElement)
            return "Farklı bir element seç.";
        var main = data.GetMainItem(element);
        if (main == null || main.element != element || main.itemShape == null || main.itemShape.Count == 0)
            return "Yeni elementin ana eşya verisi eksik.";
        if (EconomyManager.Gold < ElementChangeCost) return "Yetersiz altın.";
        return null;
    }

    public bool TryChangeElement(ElementType element)
    {
        TransitionError = GetElementChangeError(element);
        if (TransitionError != null) return false;
        if (ApplyElement(element, true)) return true;
        TransitionError = ElementSelectionError;
        return false;
    }

    private bool ApplyElement(ElementType element, bool paidChange)
    {
        ElementSelectionError = null;
        if (retired || !GameManager.CanUpgradeTowers || (!paidChange && !AwaitingElement)
            || element < ElementType.Fire || element > ElementType.Air) return false;
        if (paidChange && GetElementChangeError(element) != null) return false;
        var evolved = data.GetMainItem(element);
        if (evolved == null || evolved.element != element || evolved.itemShape == null || evolved.itemShape.Count == 0)
        {
            ElementSelectionError = "Bu elementin ana eşya verisi eksik.";
            return false;
        }
        // Sürüklemede ana eşya geçici olarak grid dışında olabilir; önce güvenle geri koy.
        BackpackDraggable.CancelActiveDrag();
        PlacedItem original = null;
        if (Backpack != null)
            foreach (var placed in Backpack.Items)
                if (placed.Data == MainItem) { original = placed; break; }
        if (original == null)
        {
            ElementSelectionError = "Ana eşya kule çantasında bulunamadı.";
            return false;
        }
        if (!Backpack.TryReplace(original, evolved))
        {
            ElementSelectionError = "Evrilmiş ana eşya bu konuma sığmıyor.";
            return false;
        }
        // Doğrulama ve değiştirme aynı ana-thread çağrısında; başarısız dönüşüm ücret kesmez.
        if (paidChange) EconomyManager.TrySpendGold(ElementChangeCost);
        SelectedElement = element;
        ProcessExperience();
        CalculateStats(); // Yaşayan askerler de seçilen elementi hemen alır.
        // İlk uzmanlaşma eski teklifleri korur. Ücretli değişim yeni mağazayı hemen açar.
        if (paidChange)
        {
            IsShopFrozen = false;
            GenerateShopItems();
        }
        var bags = BackpackUIManager.Instance;
        if (bags != null && bags.ActiveTower == this) bags.RefreshItemVisuals();
        if (UİManager.activeTower == this && shopUIManager != null) shopUIManager.UpdateShopUI();
        return true;
    }
    public int GetTypeChangeCost(GameObject prefab)
    {
        var target = prefab != null ? prefab.GetComponent<Tower>() : null;
        return target != null && target.data != null ? Mathf.Max(0, target.data.cost - TypeChangeRefund) : 0;
    }

    public string GetTypeChangeError(GameObject prefab)
    {
        if (retired || !GameManager.CanUpgradeTowers) return "Dönüşüm yalnız hazırlıkta yapılabilir.";
        if (Backpack == null || BackpackDraggable.IsDragging) return "Önce eşya sürüklemeyi bitir.";
        bool mainFound = false;
        foreach (var item in Backpack.Items)
        {
            if (item.Data != MainItem) return "Ana eşya dışındaki çanta eşyalarını sat veya depoya taşı.";
            mainFound = true;
        }
        if (!mainFound) return "Ana eşya bulunamadı.";
        var target = prefab != null ? prefab.GetComponent<Tower>() : null;
        if (target == null || target.data == null || !prefab.activeSelf) return "Bu kule türü kullanılamıyor.";
        if (target.data.type == data.type) return "Farklı bir kule türü seç.";
        var itemData = target.data.defaultWeapon;
        if (itemData == null || itemData.itemShape == null || itemData.itemShape.Count == 0
            || target.data.gridWidth < 1 || target.data.gridHeight < 1) return "Yeni kulenin çanta verisi eksik.";
        var grid = new Backpack(target.data.gridWidth, target.data.gridHeight);
        if (!grid.CanPlace(itemData, grid.GetCenterOrigin(itemData))) return "Yeni ana eşya çantaya sığmıyor.";
        if (EconomyManager.Gold < GetTypeChangeCost(prefab)) return "Yetersiz altın.";
        return null;
    }

    public bool TryChangeTowerType(GameObject prefab, out Tower replacement)
    {
        replacement = null;
        // Sürüklenerek grid dışına alınan eşya boş-çanta kontrolünü aşamaz.
        BackpackDraggable.CancelActiveDrag();
        TransitionError = GetTypeChangeError(prefab);
        if (TransitionError != null) return false;
        GameObject created = null;
        try
        {
            created = Instantiate(prefab, transform.position, transform.rotation);
            replacement = created.GetComponent<Tower>();
            replacement.startAtLevelOne = true;
            replacement.InitializeTower();
        }
        catch (System.Exception error)
        {
            if (created != null) { created.SetActive(false); Destroy(created); }
            replacement = null;
            TransitionError = "Yeni kule hazırlanamadı; altın ve eşyalar korundu.";
            Debug.LogException(error);
            return false;
        }
        if (!EconomyManager.TrySpendGold(GetTypeChangeCost(prefab)))
        {
            created.SetActive(false);
            Destroy(created);
            replacement = null;
            TransitionError = "Yetersiz altın.";
            return false;
        }
        replacement.Storage = Storage;
        if (Evolution != null) Evolution.TransferDeliveriesTo(replacement.Evolution);
        if (this is UnitTower oldBarracks) oldBarracks.RemoveUnitsForTypeChange();
        retired = true; // Aynı karede ikinci onay/ödül eski kuleyi kullanamaz.
        gameObject.SetActive(false);
        Destroy(gameObject);
        return true;
    }

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
    // Her kule kendi deposuna sahiptir; CalculateStats yalnız Backpack'i okur.
    public Backpack Storage { get; private set; }
    public List<ItemEvolutionRecipe> evolutionRecipes = new List<ItemEvolutionRecipe>();
    public TowerItemEvolution Evolution { get; private set; }

    private void Awake()
    {
        rangeCollider = GetComponentInChildren<SphereCollider>(true);
    }

    protected virtual void Start() => InitializeTower();

    private void InitializeTower()
    {
        if (initialized) return;
        InitializeBaseStats();
        shopUIManager = GameObject.FindObjectOfType<ShopUIManager>();

        if (rangeCollider != null)
        {
            // Birlikler kendi engageRange alanını kullanır; kule küresine ihtiyaç yok.
            rangeCollider.enabled = SupportsManualTargeting;
            rangeCollider.isTrigger = true;
            rangeCollider.center = Vector3.zero;
            Vector3 scale = rangeCollider.transform.lossyScale;
            float largestScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            // SphereCollider yarıçapı yereldir; TowerData.range dünya birimidir.
            rangeCollider.radius = Mathf.Max(0f, data.range) / Mathf.Max(0.0001f, largestScale);
        }

        Backpack = new Backpack(data.gridWidth, data.gridHeight);
        Storage = new Backpack(3, 5);
        Evolution = new TowerItemEvolution(this);

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

        // İlk teklifler; daha sonra ücretli reroll veya dalga arası ücretsiz yenileme.
        GenerateShopItems();
        initialized = true;
    }

    private void InitializeBaseStats()
    {
        // Eski asset'lerde seviye 0 olabilir; dükkan seviyeleri 1'den başlar.
        towerLevel = startAtLevelOne ? 1 : Mathf.Max(1, data.towerLevel);
        baseDamage = data.damage;
        baseCriticalChance = data.criticalChance;
        baseAttackSpeed = data.attackSpeed;
    }

    protected virtual void Update()
    {
        if (enemies.Count > 0 && !isShooting && Time.timeScale > 0f)
            StartCoroutine(Shoot());
    }

    public void GenerateShopItems()
    {
        for (int i = 0; i < myShopItems.Length; i++)
        {
            ItemRarity rarity = ShopManager.Instance.GetRandomRarity(towerLevel);
            myShopItems[i] = ShopManager.Instance.GetRandomItem(data.type, rarity, SelectedElement);
        }
    }

    // Eski Inspector bağlantıları için korunur; ücretsiz seviye atlatmaz.
    public void UpgradeTower()
    {
        if (GameManager.CanUpgradeTowers) ProcessExperience();
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
                if (!IsItemActive(placed.Data)) continue;
                var synergy = ItemSynergyCalculator.Additional(Backpack, placed, IsItemActive);
                damageBonus += synergy.damage;
                criticalChanceBonus += synergy.criticalChance;
                attackSpeedBonus += synergy.attackSpeed;
                healthBonus += synergy.health;
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
        if (TowerSlot.IsPointerOverUI(Input.mousePosition)) return;
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
            GameObject targetEnemy = SelectTarget();
            if (targetEnemy == null || Time.timeScale <= 0f)
            {
                // Uygun tür yoksa veya oyun duruyorsa bekle; başka türe geçme.
                yield return null;
                continue;
            }
            Attack(targetEnemy);

            yield return new WaitForSeconds(currentAttackSpeed);
        }
        isShooting = false;
    }

    private GameObject SelectTarget()
    {
        staleColliders.Clear();
        foreach (var pair in enemyColliders)
            if (pair.Key == null || !pair.Key.enabled || !pair.Key.gameObject.activeInHierarchy || pair.Value == null)
                staleColliders.Add(pair.Key);
        foreach (var collider in staleColliders) enemyColliders.Remove(collider);
        enemies.RemoveAll(enemy => enemy == null || !enemy.activeInHierarchy || !enemyColliders.ContainsValue(enemy));

        for (int n = 0; n < enemies.Count; n++)
        {
            int index = SelectedTargetOrder == TargetOrder.FirstEntered ? n : enemies.Count - 1 - n;
            var enemy = enemies[index].GetComponent<Enemy>();
            if (enemy == null || enemy.CurrentHealth <= 0f || enemy.enemyData == null) continue;
            bool matches = SelectedTargetType == TargetType.Both
                || (SelectedTargetType == TargetType.Ground && enemy.enemyData.movementType == EnemyData.MovementType.Ground)
                || (SelectedTargetType == TargetType.Air && enemy.enemyData.movementType == EnemyData.MovementType.Air);
            if (matches) return enemy.gameObject;
        }
        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!SupportsManualTargeting || enemyColliders.ContainsKey(other)) return;
        var enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null || !enemy.CompareTag("Enemy")) return;
        enemyColliders.Add(other, enemy.gameObject);
        if (!enemies.Contains(enemy.gameObject)) enemies.Add(enemy.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!enemyColliders.TryGetValue(other, out GameObject enemy)) return;
        enemyColliders.Remove(other);
        if (!enemyColliders.ContainsValue(enemy)) enemies.Remove(enemy);
    }

    private void OnDisable()
    {
        if (!SupportsManualTargeting) return;
        StopAllCoroutines();
        isShooting = false;
        enemies.Clear();
        enemyColliders.Clear();
    }
}
