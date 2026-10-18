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
    public bool SupportsManualTargeting => this is ArcherTower || this is MegicTower || this is MortarTower;
    public bool SupportsAirTargets => !(this is MortarTower);

    public void SetTargetType(int value)
    {
        if (SupportsManualTargeting && value >= 0 && value <= 2 && (SupportsAirTargets || value == (int)TargetType.Ground))
            SelectedTargetType = (TargetType)value;
    }

    public void SetTargetOrder(int value)
    {
        if (SupportsManualTargeting && value >= 0 && value <= 1)
            SelectedTargetOrder = (TargetOrder)value;
    }
    private bool isShooting = false;
    private SphereCollider rangeCollider;
    protected List<SkillData.Effect> skillEffects = new List<SkillData.Effect>();
    protected float SkillTotal(SkillData.EffectType type)
    { float value=0f;foreach(var effect in skillEffects)if(effect.type==type)value+=effect.value;return value/100f; }
    public virtual float CurrentRange => data != null ? data.range*(1f+SkillTotal(SkillData.EffectType.RangePercent)) : 0f;
    protected virtual void OnEnable() { SkillTreeProgress.Changed+=RefreshSkillStats; }
    protected virtual void RefreshSkillStats() { if(data!=null)CalculateStats(); }
    protected void RefreshRangeCollider()
    {
        if(rangeCollider==null)return;
        Vector3 scale=rangeCollider.transform.lossyScale;
        float largest=Mathf.Max(Mathf.Abs(scale.x),Mathf.Abs(scale.y),Mathf.Abs(scale.z));
        rangeCollider.radius=Mathf.Max(0f,CurrentRange)/Mathf.Max(.0001f,largest);
    }
    protected virtual void OnTargetSelected(GameObject target) { }


    public ItemData[] myShopItems = new ItemData[4];
    public bool IsShopFrozen => false;
    public bool IsElementShop { get; private set; }
    private bool elementShopUnlocked;
    private ItemData[] savedItemOffers;
    private readonly HashSet<int> savedReservations = new HashSet<int>();

    public bool OpenElementShop()
    {
        if(Level<5||!GameManager.CanEditItems||IsElementShop)return false;
        elementShopUnlocked=true;
        savedItemOffers=(ItemData[])myShopItems.Clone();
        savedReservations.Clear();
        foreach(var index in reservedOffers)savedReservations.Add(index);
        reservedOffers.Clear();IsElementShop=true;GenerateShopItems();return true;
    }
    public void CompleteShopPurchase(int index, ItemData purchased)
    {
        if(index<0||index>=myShopItems.Length||myShopItems[index]!=purchased)return;
        myShopItems[index]=null;reservedOffers.Remove(index);
        if(!IsElementShop||purchased==null||!purchased.isElementCatalyst)return;
        IsElementShop=false;
        myShopItems=savedItemOffers??new ItemData[4];savedItemOffers=null;
        reservedOffers.Clear();foreach(var slot in savedReservations)reservedOffers.Add(slot);
        savedReservations.Clear();
        if(System.Array.TrueForAll(myShopItems,x=>x==null))GenerateShopItems();
    }
    private readonly HashSet<int> reservedOffers=new HashSet<int>();
    public bool IsOfferReserved(int index)=>reservedOffers.Contains(index);
    public void ToggleOfferReservation(int index)
    {
        if(!GameManager.CanEditItems||index<0||index>=myShopItems.Length||myShopItems[index]==null)return;
        if(!reservedOffers.Remove(index))reservedOffers.Add(index);
    }
    public void ClearOfferReservation(int index)=>reservedOffers.Remove(index);

    public void ToggleShopFreeze()
    {
        // Legacy inspector callback: reservations now belong to individual offers.
    }

    // TowerData'dan başlangıçta kopyalanır; yükseltmeler yalnız bu kuleyi etkiler.
    internal int towerLevel;
    public int Level => towerLevel;
    public float Experience { get; private set; }
    public ElementType SelectedElement { get; private set; } = ElementType.Normal;
    public ItemData MainItem => data != null ? data.GetMainItem(SelectedElement) : null;
    public string ElementSelectionError { get; private set; }
    public bool IsMainItem(ItemData item) => data != null && data.IsMainItem(item);
    public bool AwaitingElement => false;
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
        while (Experience >= ExperienceRequired)
        {
            Experience -= ExperienceRequired;
            towerLevel++;
            if(towerLevel==5&&ShopManager.Instance!=null)GenerateShopItems();
            RebuildLevelStats();
            CalculateStats();
        }
    }

    public bool TrySelectElement(ElementType element) => false;

    public string GetElementChangeError(ElementType element)
    {
        return "Element eşyasını ana silahın yanına yerleştir.";
    }

    public bool TryChangeElement(ElementType element)
    {
        TransitionError = GetElementChangeError(element);return false;
    }

    public bool TryFuseElement()
    {
        if(retired||Level<5||!GameManager.CanEditItems||Backpack==null)return false;
        ElementSelectionError = null;
        PlacedItem original = null;
        foreach(var placed in Backpack.Items)if(IsMainItem(placed.Data)){original=placed;break;}
        if(original==null)return false;
        foreach(var ingredient in new List<PlacedItem>(Backpack.Items))
        {
            if(!ingredient.Data.isElementCatalyst||ingredient.Data.element==original.Data.element||
                !TowerItemEvolution.Adjacent(original,ingredient))continue;
            var evolved=data.GetMainItem(ingredient.Data.element);
            if(evolved==null||evolved.element!=ingredient.Data.element)continue;
            if(!Backpack.TryReplace(original,evolved))
            {ElementSelectionError="Dönüşen silah için çantada yer yok.";return false;}
            Evolution.ReleaseAttachments(original);
            Backpack.Remove(ingredient);
            SelectedElement=evolved.element;
            return true;
        }
        return false;
    }
    public int GetTypeChangeCost(GameObject prefab)
    {
        var target = prefab != null ? prefab.GetComponent<Tower>() : null;
        return target != null && target.data != null ? Mathf.Max(0, target.data.cost - TypeChangeRefund) : 0;
    }

    public int TypeChangeSaleValue
    {
        get
        {
            int total = 0;
            if (Backpack != null) foreach (var item in Backpack.Items)
                if (!IsMainItem(item.Data)) total += EconomyManager.GetSellValue(item.Data);
            return total;
        }
    }

    public int TypeChangeSaleCount
    {
        get
        {
            int count = 0;
            if (Backpack != null) foreach (var item in Backpack.Items)
                if (!IsMainItem(item.Data)) count++;
            return count;
        }
    }

    public string GetTypeChangeError(GameObject prefab)
    {
        if (retired || !GameManager.CanUpgradeTowers) return "Dönüşüm yalnız hazırlıkta yapılabilir.";
        if (Backpack == null || BackpackDraggable.IsDragging) return "Önce eşya sürüklemeyi bitir.";
        bool mainFound = false;
        foreach (var item in Backpack.Items)
        {
            if (item.Data == MainItem) mainFound = true;
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
        if (EconomyManager.Gold + TypeChangeSaleValue < GetTypeChangeCost(prefab)) return "Eşya satışı dahil yetersiz altın.";
        return null;
    }

    public bool TryChangeTowerType(GameObject prefab, out Tower replacement)
    {
        replacement = null;
        // Sürüklenen eşya önce geri konur; satış hesabından kaçamaz.
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
        int finalGold = EconomyManager.Gold + TypeChangeSaleValue - GetTypeChangeCost(prefab);
        if (finalGold < 0)
        {
            created.SetActive(false);
            Destroy(created);
            replacement = null;
            TransitionError = "Yetersiz altın.";
            return false;
        }
        // Yeni kule başarıyla hazırlandı: satış ve ücret tek bakiye değişikliğidir.
        EconomyManager.Gold = finalGold;
        UİManager.UpdateGoldText();
        foreach (var item in new List<PlacedItem>(Backpack.Items))
        { Evolution.ReleaseAttachments(item);if (!IsMainItem(item.Data)) Backpack.Remove(item); }
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
        if (!SupportsAirTargets) SelectedTargetType = TargetType.Ground;
        shopUIManager = GameObject.FindObjectOfType<ShopUIManager>(true);

        if (rangeCollider != null)
        {
            // Birlikler kendi engageRange alanını kullanır; kule küresine ihtiyaç yok.
            rangeCollider.enabled = SupportsManualTargeting;
            rangeCollider.isTrigger = true;
            rangeCollider.center = Vector3.zero;
            Vector3 scale = rangeCollider.transform.lossyScale;
            float largestScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            // SphereCollider yarıçapı yereldir; TowerData.range dünya birimidir.
            rangeCollider.radius = Mathf.Max(0f, CurrentRange) / Mathf.Max(0.0001f, largestScale);
        }

        Backpack = new Backpack(data.gridWidth, data.gridHeight);
        Storage = new Backpack(2, 6, true);
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
        RebuildLevelStats();
    }

    private void RebuildLevelStats()
    {
        int gainedLevels = Mathf.Max(0, towerLevel - 1);
        baseDamage = data.damage * (1f + gainedLevels * data.levelDamageGrowth);
        baseCriticalChance = Mathf.Min(data.maximumCriticalChance, data.criticalChance + gainedLevels * data.levelCriticalGrowth);
        baseAttackSpeed = Mathf.Max(0.1f, data.attackSpeed / (1f + gainedLevels * data.levelAttackSpeedGrowth));
    }

    protected virtual void Update()
    {
        if (enemies.Count > 0 && !isShooting && Time.timeScale > 0f)
            StartCoroutine(Shoot());
    }

    public void GenerateShopItems()
    {
        if(Level>=5&&!elementShopUnlocked)
        {
            elementShopUnlocked=true;savedItemOffers=(ItemData[])myShopItems.Clone();
            savedReservations.Clear();foreach(var index in reservedOffers)savedReservations.Add(index);
            reservedOffers.Clear();IsElementShop=true;
        }
        var catalysts=IsElementShop?Resources.LoadAll<ItemData>("Data/Items/Elements"):null;
        for (int i = 0; i < myShopItems.Length; i++)
        {
            if(IsOfferReserved(i)&&myShopItems[i]!=null)continue;
            reservedOffers.Remove(i);
            if(IsElementShop)
            {
                myShopItems[i]=catalysts.Length>0?catalysts[Random.Range(0,catalysts.Length)]:null;
                continue;
            }
            ItemRarity rarity = ShopManager.Instance.GetRandomRarity(towerLevel);
            myShopItems[i] = ShopManager.Instance.GetRandomItem(data.type, rarity, SelectedElement);
            if(myShopItems[i]==null&&SelectedElement!=ElementType.Normal)
                myShopItems[i]=ShopManager.Instance.GetRandomItem(data.type,rarity,ElementType.Normal);
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
        TryFuseElement();
        SelectedElement=ElementType.Normal;
        if(Backpack!=null)foreach(var equipped in Backpack.Items)
            if(IsMainItem(equipped.Data)){SelectedElement=equipped.Data.element;break;}
        skillEffects=new SkillTreeProgress().ActiveEffects(data.type);
        // Item bonusları kesir (0.05 = %5) ve kendi aralarında toplanır.
        float damageBonus = 0f;
        float flatDamageBonus = 0f;
        float criticalChanceBonus = 0f;
        float attackSpeedBonus = 0f;
        float healthBonus = 0f;

        if (Backpack != null)
        {
            foreach (PlacedItem placed in Backpack.Items)
            {
                if (!IsItemActive(placed.Data)) continue;
                var synergy = ItemSynergyCalculator.Additional(Backpack, placed, IsItemActive);
                flatDamageBonus += synergy.flatDamage;
                damageBonus += synergy.damage;
                criticalChanceBonus += synergy.criticalChance;
                attackSpeedBonus += synergy.attackSpeed;
                healthBonus += synergy.health;
                damageBonus += placed.Data.damageBonus;
                criticalChanceBonus += placed.Data.criticalChanceBonus;
                attackSpeedBonus += placed.Data.attackSpeedBonus;
                healthBonus += placed.Data.healthBonus;
                foreach(var part in placed.Attachments)if(IsItemActive(part.Data)){
                    damageBonus+=part.Data.damageBonus;criticalChanceBonus+=part.Data.criticalChanceBonus;
                    attackSpeedBonus+=part.Data.attackSpeedBonus;healthBonus+=part.Data.healthBonus;
                }
            }
        }

        currentHealthBonus = healthBonus;
        currentDamage = baseDamage * (1f + damageBonus) + flatDamageBonus;
        currentCriticalChance = Mathf.Clamp(baseCriticalChance + criticalChanceBonus, 0f, data.maximumCriticalChance);
        // currentAttackSpeed bir "atışlar arası bekleme süresi" (cooldown). %X hızlanma
        // = süreyi (1 + X)'e bölmek; böylece bonuslar ne kadar birikirse biriksin süre sıfıra inmez.
        currentAttackSpeed = baseAttackSpeed / (1f + attackSpeedBonus);

        // �ok fazla attackSpeedBonus �st �ste binerse s�re s�f�ra/negatife d���p
        // Shoot() coroutine'inde WaitForSeconds'� bozmas�n diye alt s�n�r.
        currentDamage*=1f+SkillTotal(SkillData.EffectType.DamagePercent);
        currentCriticalChance=Mathf.Min(data.maximumCriticalChance,currentCriticalChance+SkillTotal(SkillData.EffectType.CriticalChancePoints));
        currentAttackSpeed/=1f+SkillTotal(SkillData.EffectType.AttackSpeedPercent);
        currentHealthBonus=(1f+currentHealthBonus)*(1f+SkillTotal(SkillData.EffectType.UnitHealthPercent))-1f;
        currentAttackSpeed = Mathf.Max(0.1f, currentAttackSpeed);
        RefreshRangeCollider();
    }

    private void OnMouseDown()
    {
        if (TowerSlot.IsPointerOverUI(Input.mousePosition)) return;
        if (UİManager.ConsumeWorldPanelClick()) return;
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
            OnTargetSelected(targetEnemy);
            if (targetEnemy == null || Time.timeScale <= 0f)
            {
                // Uygun tür yoksa veya oyun duruyorsa bekle; başka türe geçme.
                yield return null;
                continue;
            }
            Attack(targetEnemy);

            yield return new WaitForSeconds(currentAttackSpeed);
        }
        OnTargetSelected(null);
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
            if (Vector3.Distance(transform.position, enemy.transform.position) > CurrentRange) continue;
            if (!SupportsAirTargets && enemy.IsAir) continue;
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

    protected virtual void OnDisable()
    {
        SkillTreeProgress.Changed-=RefreshSkillStats;
        OnTargetSelected(null);
        if (!SupportsManualTargeting) return;
        StopAllCoroutines();
        isShooting = false;
        enemies.Clear();
        enemyColliders.Clear();
    }
}
