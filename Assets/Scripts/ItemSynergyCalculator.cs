using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemBonus
{
    public float damage, attackSpeed, criticalChance, health, flatDamage;
    public static ItemBonus operator +(ItemBonus a, ItemBonus b) => new ItemBonus {
        flatDamage=a.flatDamage+b.flatDamage, damage=a.damage+b.damage, attackSpeed=a.attackSpeed+b.attackSpeed,
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

// Saf hesap: asset veya envanter değiştirmez. Aynı tarif referansı bir kez okunur;
// farklı sağlayıcıların katkısı tarifin maxActivations değeriyle sınırlıdır.
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

    public static IEnumerable<Vector2Int> Cells(ItemData data,ItemSynergyRecipe recipe,Vector2Int origin,int rotation)
    {
        if(recipe==null||recipe.placement==ItemSynergyRecipe.Placement.Attached||recipe.cells==null)yield break;
        foreach(var cell in recipe.cells){if(data.itemShape.Contains(cell))continue;
            yield return origin+ItemShapeUtility.RotateCell(cell,ItemShapeUtility.GetBoundingSize(data.itemShape),rotation);
        }
    }
    public static int Activations(Backpack bag,PlacedItem item,ItemSynergyRecipe recipe,Func<ItemData,bool> compatible=null)
    {
        if(bag==null||item==null||item.Parent!=null||!bag.Contains(item)||recipe==null||recipe.targetItem!=item.Data||
           (compatible!=null&&!compatible(item.Data)))return 0;
        var sources=new HashSet<PlacedItem>();
        foreach(var cell in Cells(item.Data,recipe,item.Origin,item.Rotation)){
            var source=bag.GetItemAt(cell);if(source!=null&&source!=item&&recipe.Accepts(source.Data)&&(compatible==null||compatible(source.Data)))sources.Add(source);
        }
        if(recipe.AllowsAttachment)foreach(var part in item.Attachments)
            if(recipe.Accepts(part.Data)&&(compatible==null||compatible(part.Data)))sources.Add(part);
        return Mathf.Min(Mathf.Max(1,recipe.maxActivations),sources.Count);
    }
    public static ItemBonus Additional(Backpack bag,PlacedItem item,Func<ItemData,bool> compatible=null)
    {
        var result=new ItemBonus();
        if(item==null||item.Data.synergyRecipes==null)return result;
        var used=new HashSet<ItemSynergyRecipe>();
        foreach(var recipe in item.Data.synergyRecipes)if(recipe!=null&&used.Add(recipe)){
            int count=Activations(bag,item,recipe,compatible);for(int i=0;i<count;i++)result+=recipe.bonus;
        }
        return result;
    }
}
