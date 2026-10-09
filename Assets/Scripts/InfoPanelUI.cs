using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Yalnız gösterim: bonus hesaplamaz, hedefin mevcut verisini okur.
public class InfoPanelUI : MonoBehaviour
{
    public RectTransform panel;
    public UnityEngine.UI.Text titleText;
    public UnityEngine.UI.Text subtitleText;
    public UnityEngine.UI.Text bodyText;
    public UnityEngine.UI.Image icon;
    public UnityEngine.UI.Image accent;

    public bool IsVisible => panel != null && panel.gameObject.activeSelf;

    public void Hide()
    {
        if (panel != null) panel.gameObject.SetActive(false);
    }

    public void Show(Object target, Vector2 screenPosition)
    {
        var body = new StringBuilder();
        Sprite sprite = null;
        Color color = new Color(0.45f, 0.72f, 1f);
        string title, subtitle;

        if (target is ItemData item)
        {
            title = item.Name;
            subtitle = RarityName(item.Rarity) + " · " + Owners(item.itemOwners);
            if (item.element != ElementType.Normal) subtitle += " · " + ElementName(item.element);
            var viewingTower = BackpackUIManager.Instance != null ? BackpackUIManager.Instance.ActiveTower : null;
            if (viewingTower != null && !viewingTower.IsItemActive(item))
                body.AppendLine("UYUMSUZ · Bu kuleye bonus vermez.");
            sprite = item.itemIcon;
            color = RarityColor(item.Rarity);
            Vector2Int size = ItemShapeUtility.GetBoundingSize(item.itemShape);
            body.AppendLine(item.cost > 0 ? "Fiyat: " + item.cost + " altın"
                : item.element != ElementType.Normal ? "Ana eşya · Element evrimi" : "Başlangıç eşyası · kuleye dahil");
            body.AppendLine("Boyut: " + size.x + " × " + size.y + " · " + item.itemShape.Count + " hücre");
            body.AppendLine();
            Bonus(body, "Hasar", item.damageBonus);
            Bonus(body, "Saldırı hızı", item.attackSpeedBonus);
            if (item.criticalChanceBonus != 0f)
                body.AppendLine("Kritik şansı: +" + Number(item.criticalChanceBonus * 100f) + " yüzde puanı");
            Bonus(body, "Asker canı", item.healthBonus);
            if (item.damageBonus == 0f && item.attackSpeedBonus == 0f && item.criticalChanceBonus == 0f && item.healthBonus == 0f)
                body.AppendLine("Ek stat bonusu yok.");
            if (item.Type == ItemType.Potion) body.AppendLine("Pasif etki · tüketilmez.");
            if(item.synergyRules!=null)foreach(var rule in item.synergyRules)
            {
                if(rule==null)continue;
                string filter=rule.filter==ItemSynergyRule.Filter.Any?"Herhangi bir eşya":rule.filter==ItemSynergyRule.Filter.Type?rule.acceptedType.ToString():rule.acceptedItem!=null?rule.acceptedItem.Name:"Tanımsız";
                body.AppendLine("Sinerji: "+filter+" · "+rule.description);
            }
            var owner = BackpackUIManager.Instance != null ? BackpackUIManager.Instance.ActiveTower : null;
            if(owner!=null && owner.evolutionRecipes!=null)foreach(var recipe in owner.evolutionRecipes)
            {
                if(recipe==null||recipe.result==null)continue;
                var partner=recipe.first==item?recipe.second:recipe.second==item?recipe.first:null;
                if(partner!=null)body.AppendLine("Evrim: "+partner.Name+" ile yan yana → "+recipe.result.Name+" (1 başarılı wave)");
            }
        }
        else if (target is Tower tower && tower.data != null)
        {
            title = TowerName(tower.data.type) + " Kulesi";
            subtitle = "Seviye " + tower.towerLevel;
            body.AppendLine("XP: " + Number(tower.Experience) + " / " + Number(tower.ExperienceRequired));
            body.AppendLine("Element: " + ElementName(tower.SelectedElement));
            if (tower.AwaitingElement) body.AppendLine("Hazırlıkta element seçimi bekleniyor.");
            sprite = tower.MainItem != null ? tower.MainItem.itemIcon : null;
            bool isBarracks = tower is UnitTower;
            Combat(body, tower.currentDamage, tower.currentAttackSpeed, tower.currentCriticalChance);
            if (isBarracks)
            {
                var barracks = (UnitTower)tower;
                body.AppendLine("Değerler asker başınadır.");
                body.AppendLine("Askerler: " + barracks.ActiveUnits.Count + " / " + barracks.maxUnitCount);
                body.AppendLine("Yeniden üretim: " + Number(barracks.spawnCooldown) + " sn");
                body.AppendLine("Asker can bonusu: %" + Number(tower.currentHealthBonus * 100f));
            }
            else body.AppendLine("Menzil: " + Number(tower.data.range));
        }
        else if (target is Unit unit && unit.CurrentHealth > 0)
        {
            title = "Lejyoner";
            subtitle = unit.IsIdle ? "Birlik · nöbette" : "Birlik · çatışmada";
            color = new Color(0.35f, 0.88f, 0.62f);
            var ownerTower = unit.myTower != null ? unit.myTower.GetComponent<Tower>() : null;
            if (ownerTower != null && ownerTower.MainItem != null) sprite = ownerTower.MainItem.itemIcon;
            body.AppendLine("Can: " + unit.CurrentHealth + " / " + unit.MaxHealth);
            Combat(body, unit.damage, unit.attackInterval, unit.critChance);
            body.AppendLine("Kritik çarpanı: ×" + Number(unit.critMultiplier));
            body.AppendLine("Element: " + ElementName(unit.elementType));
        }
        else if (target is Enemy enemy && enemy.enemyData != null && enemy.CurrentHealth > 0)
        {
            title = enemy.enemyData.enemyName == "Fast" ? "Hızlı Düşman" : enemy.enemyData.enemyName;
            subtitle = enemy.isEngaged ? "Düşman · çatışmada" : "Düşman · ilerliyor";
            subtitle += " · " + ElementName(enemy.enemyData.element);
            color = new Color(1f, 0.4f, 0.38f);
            body.AppendLine("Can: " + Number(enemy.CurrentHealth) + " / " + Number(enemy.MaxHealth));
            body.AppendLine("Hasar: " + enemy.damage);
            body.AppendLine("Saldırı aralığı: " + Number(enemy.enemyData.attackInterval) + " sn");
            body.AppendLine("Hareket hızı: " + Number(enemy.speed));
            if(enemy.IsAir) body.AppendLine("Hava · Yolu takip eder · Birlikler durduramaz");
            body.AppendLine("Ödül: " + enemy.enemyData.goldReward + " altın · " + Number(enemy.enemyData.experienceReward) + " XP");
            body.AppendLine("Zayıflık (×1,5): " + (enemy.enemyData.element == ElementType.Normal
                ? Elements(enemy.enemyData.weaknesses) : ElementName(enemy.enemyData.WeakTo)));
            body.AppendLine("Direnç (×0,5): " + (enemy.enemyData.element == ElementType.Normal
                ? Elements(enemy.enemyData.resistances) : ElementName(enemy.enemyData.ResistantTo)));
        }
        else { Hide(); return; }

        titleText.text = title;
        subtitleText.text = subtitle;
        bodyText.text = body.ToString().TrimEnd();
        icon.sprite = sprite;
        icon.enabled = sprite != null;
        accent.color = color;
        subtitleText.color = color;
        panel.gameObject.SetActive(true);
        MoveTo(screenPosition);
    }

    public void MoveTo(Vector2 screenPosition)
    {
        var root = panel.parent as RectTransform;
        var canvas = panel.GetComponentInParent<Canvas>();
        Camera camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(root, screenPosition, camera, out Vector2 point)) return;
        // Panel pivot'u sol üst: sağ/alt taşmasını Canvas koordinatlarında sınırla.
        float width = panel.rect.width, height = panel.rect.height;
        float x = point.x + 20f, y = point.y - 20f;
        if (x + width > root.rect.xMax - 8f) x = point.x - width - 20f;
        x = Mathf.Clamp(x, root.rect.xMin + 8f, Mathf.Max(root.rect.xMin + 8f, root.rect.xMax - width - 8f));
        y = Mathf.Clamp(y, Mathf.Min(root.rect.yMax - 8f, root.rect.yMin + height + 8f), root.rect.yMax - 8f);
        panel.localPosition = new Vector3(x, y, 0f);
    }

    private static void Combat(StringBuilder body, float damage, float interval, float critical)
    {
        body.AppendLine("Hasar: " + Number(damage));
        body.AppendLine("Saldırı aralığı: " + Number(interval) + " sn");
        body.AppendLine("Kritik şansı: %" + Number(critical * 100f));
    }
    private static void Bonus(StringBuilder body, string label, float value)
    {
        if (value != 0f) body.AppendLine(label + ": +%" + Number(value * 100f));
    }
    private static string Number(float value) => value.ToString("0.##");
    public static string TowerName(TowerType type)
    {
        switch (type) { case TowerType.ArrowTower: return "Okçu"; case TowerType.MagicTower: return "Büyücü"; case TowerType.UnitTower: return "Birlik"; default: return "Topçu"; }
    }
    private static string Owners(TowerType[] owners)
    {
        if (owners == null || owners.Length == 0) return "";
        var names = new List<string>(); foreach (var owner in owners) names.Add(TowerName(owner));
        return string.Join(", ", names);
    }
    private static string RarityName(ItemRarity rarity)
    {
        switch (rarity) { case ItemRarity.Common: return "Yaygın"; case ItemRarity.Rare: return "Nadir"; case ItemRarity.Epic: return "Epik"; default: return "Efsanevi"; }
    }
    private static Color RarityColor(ItemRarity rarity)
    {
        switch (rarity) { case ItemRarity.Common: return new Color(0.7f, 0.78f, 0.84f); case ItemRarity.Rare: return new Color(0.4f, 0.7f, 1f); case ItemRarity.Epic: return new Color(0.75f, 0.55f, 1f); default: return new Color(1f, 0.78f, 0.35f); }
    }
    private static string Elements(List<ElementType> elements)
    {
        if (elements == null || elements.Count == 0) return "Yok";
        var names = new List<string>(); foreach (var element in elements) names.Add(ElementName(element));
        return string.Join(", ", names);
    }
    private static string ElementName(ElementType element)
    {
        switch (element) { case ElementType.Fire: return "Ateş"; case ElementType.Water: return "Su"; case ElementType.Earth: return "Toprak"; case ElementType.Air: return "Hava"; default: return "Normal"; }
    }
}
