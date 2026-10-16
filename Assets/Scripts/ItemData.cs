using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Helmet,
    Armour,
    Potion,
    Parchment
}

public enum ItemRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "New Item", menuName = "Backpack/Item")]
public class ItemData : ScriptableObject
{
    [Tooltip("Item'in kapladığı hücreler, item'in kendi orijinine göre (0,0) sol-üst olacak şekilde tanımlanır. L-şekli örneği: (0,0),(0,1),(0,2),(1,2)")]
    public List<Vector2Int> itemShape;

    public string Name;
    public ItemType Type;
    public ItemRarity Rarity;
    public TowerType[] itemOwners;
    [Tooltip("Mağaza havuzu: Normal yalnız element seçilmeden önce sunulur. Önceden alınmış eşyalar korunur.")]
    public ElementType element = ElementType.Normal;
    public bool IsCompatibleWith(TowerType towerType, ElementType towerElement) =>
        itemOwners != null && System.Array.Exists(itemOwners, owner => owner == towerType)
        && (element == ElementType.Normal || element == towerElement);
    public string itemID;
    public int cost;
    public Sprite itemIcon;
    public List<string> synergyIDs;
    public List<ItemSynergyRecipe> synergyRecipes = new List<ItemSynergyRecipe>();
    [HideInInspector] public List<ItemSynergyRule> synergyRules = new List<ItemSynergyRule>();

    public float damageBonus;
    public float criticalChanceBonus;
    public float attackSpeedBonus;

    [Tooltip("Sadece Birlik kulesinde işe yarar: üretilen birliklerin canına % bonus (0.2 = %20).")]
    public float healthBonus;

    [Tooltip("Sürüklerken imlecin/dokunuşun item şeklinin hangi hücresine denk geleceği. Genelde şeklin görsel ortası.")]
    public Vector2Int centerOffset;

    // Not: width/height ve RotateItem() kaldırıldı.
    // Boyut artık ItemShapeUtility.GetBoundingSize(itemShape) ile itemShape'ten
    // TEK bir kaynaktan türetiliyor - ayrı bir width/height tutmak, tasarımcı
    // şekli güncelleyip boyutu güncellemeyi unuttuğunda veriyi tutarsız bırakırdı.
    // Dönüş, PlacedItem/BackpackDraggable runtime kaydında tutulur.
    // ItemShapeUtility bu özgün şekilden dönmüş hücreleri hesaplar;
    // paylaşılan ScriptableObject asset'i döndürülerek değiştirilmez.
}
