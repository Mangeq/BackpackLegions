using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed partial class TowerPanelLayout
{
    [SerializeField] Button goldUpgrade;
    [SerializeField] TMP_Text goldUpgradeCaption;
    [SerializeField] RectTransform itemPlacementLock;
    void RefreshGoldUpgrade(Tower tower)
    {
        if (goldUpgrade == null)
        {
            // Covers the former experience bar, including the lettering baked into the artwork.
            var rect = NewRect("GoldUpgrade", root);
            ArtPlace(rect, 900, 34, 695, 83);
            var face = rect.gameObject.AddComponent<Image>();
            face.color = new Color(.13f, .19f, .12f, 1f);
            goldUpgrade = rect.gameObject.AddComponent<Button>();
            goldUpgrade.targetGraphic = face;
            goldUpgrade.onClick.AddListener(() => { if (bags.ActiveTower != null) bags.ActiveTower.TryUpgrade(); });
            var caption = MakeValue("Caption", rect, 16, 8, 660, 65, 29, cream);
            goldUpgradeCaption = caption.text;
            goldUpgradeCaption.alignment = TextAlignmentOptions.Center;
            var speedRect = NewRect("AttackSpeedCaption", root);
            ArtPlace(speedRect, 91, 139, 262, 40);
            var cover = speedRect.gameObject.AddComponent<Image>();
            cover.color = new Color(.16f, .12f, .085f); cover.raycastTarget = false;
            var speedLabel = MakeValue("Label", speedRect, 6, 0, 248, 40, 26, cream);
            speedLabel.Set("Saldırı hızı");
        }
        xp.text.gameObject.SetActive(false);
        if (itemPlacementLock == null)
        {
            itemPlacementLock = NewRect("ItemPlacementLock", root);
            ArtPlace(itemPlacementLock, 954, 311, 642, 598);
            var cover = itemPlacementLock.gameObject.AddComponent<Image>();
            cover.color = new Color(.075f, .095f, .075f, .97f);
            var label = MakeValue("Caption", itemPlacementLock, 25, 210, 590, 150, 30, cream);
            label.text.enableWordWrapping = true;
            label.text.alignment = TextAlignmentOptions.Center;
            label.Set("EŞYA İŞLEMLERİ KİLİTLİ\nHazırlıkta düzenleyebilirsin");
        }
        itemPlacementLock.gameObject.SetActive(!GameManager.CanEditItems);
        goldUpgrade.interactable = GameManager.CanLevelUpTowers && EconomyManager.Gold >= tower.UpgradeCost && !BackpackDraggable.IsDragging;
        goldUpgradeCaption.text = "SEVİYE " + (tower.Level + 1) + "  ·  YÜKSELT  ·  " + tower.UpgradeCost + " ALTIN";
    }
}
