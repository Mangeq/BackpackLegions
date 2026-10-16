using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Backpack/Synergy Recipe",fileName="SynergyRecipe")]
public class ItemSynergyRecipe : ScriptableObject
{
    public enum Placement { Cells, Attached, CellsOrAttached }
    public ItemData targetItem;
    public ItemData supportItem;
    [Tooltip("Eski genel kuralları da destekler. Belirli iki eşya için SpecificItem seç.")]
    public ItemSynergyRule.Filter filter = ItemSynergyRule.Filter.SpecificItem;
    public ItemType supportType;
    public Placement placement;
    [Tooltip("Bonus alan eşyanın döndürülmemiş yerel koordinatları. Şeklinin dışındaki kareler.")]
    public List<Vector2Int> cells = new List<Vector2Int>();
    [Min(1)] public int maxActivations = 1;
    [TextArea] public string description;
    [Tooltip("flatDamage sabit hasar; diğer bonuslar kesir: 0.05 = %5.")]
    public ItemBonus bonus;
    public bool AllowsAttachment => placement != Placement.Cells;
    public bool Accepts(ItemData item) => item!=null && (filter==ItemSynergyRule.Filter.Any ||
        (filter==ItemSynergyRule.Filter.Type && item.Type==supportType) ||
        (filter==ItemSynergyRule.Filter.SpecificItem && item==supportItem));
}
