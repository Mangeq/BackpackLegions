using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemBonus
{
    public float damage, attackSpeed, criticalChance, health;
    public static ItemBonus operator +(ItemBonus a, ItemBonus b) => new ItemBonus {
        damage=a.damage+b.damage, attackSpeed=a.attackSpeed+b.attackSpeed,
        criticalChance=a.criticalChance+b.criticalChance, health=a.health+b.health };
}

[Serializable]
public class ItemSynergyRule
{
    public enum Filter { Any, Type, SpecificItem }
    public string description;
    public List<Vector2Int> cells = new List<Vector2Int>();
    public Filter filter;
    public ItemType acceptedType;
    public ItemData acceptedItem;
    public ItemBonus bonus;
    public bool Accepts(ItemData item) => item != null && (filter == Filter.Any ||
        (filter == Filter.Type && item.Type == acceptedType) ||
        (filter == Filter.SpecificItem && item == acceptedItem));
}

// Saf hesap: asset veya envanter değiştirmez. Her kural en fazla bir kez katkı verir.
public static class ItemSynergyCalculator
{
    public static IEnumerable<Vector2Int> Cells(ItemData data, ItemSynergyRule rule, Vector2Int origin, int rotation)
    {
        if (rule == null || rule.cells == null) yield break;
        foreach (var cell in rule.cells)
        {
            bool adjacent = false;
            foreach (var occupied in data.itemShape)
                if (Mathf.Abs(cell.x-occupied.x)+Mathf.Abs(cell.y-occupied.y)==1) adjacent=true;
            if (data.itemShape.Contains(cell) || !adjacent) continue;
            yield return origin + ItemShapeUtility.RotateCell(cell, ItemShapeUtility.GetBoundingSize(data.itemShape), rotation);
        }
    }

    public static bool IsActive(Backpack bag, ItemData data, ItemSynergyRule rule, Vector2Int origin, int rotation, Func<ItemData, bool> compatible = null)
    {
        if (compatible != null && !compatible(data)) return false;
        foreach (var cell in Cells(data, rule, origin, rotation))
        {
            var neighbor = bag.GetItemAt(cell);
            if (neighbor != null && (compatible == null || compatible(neighbor.Data)) && !(neighbor.Data == data && neighbor.Origin == origin && neighbor.Rotation == rotation) && rule.Accepts(neighbor.Data)) return true;
        }
        return false;
    }

    public static ItemBonus Additional(Backpack bag, PlacedItem item, Func<ItemData, bool> compatible = null)
    {
        var result = new ItemBonus();
        if (item.Data.synergyRules != null)
            foreach (var rule in item.Data.synergyRules)
                if (rule != null && IsActive(bag,item.Data,rule,item.Origin,item.Rotation,compatible)) result += rule.bonus;
        return result;
    }
}
