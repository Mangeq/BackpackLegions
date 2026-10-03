using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }
    internal List<RarityData> itemRarities;
    public List<ItemData> allItems;
    void Start()
    {
        Instance = this;

        itemRarities = new List<RarityData>
        {
            new RarityData { level = 1, commonChance = 75, rareChance = 20, epicChance = 5, legendaryChance = 0 },
            new RarityData { level = 2, commonChance = 60, rareChance = 28, epicChance = 10, legendaryChance = 2 },
            new RarityData { level = 3, commonChance = 45, rareChance = 35, epicChance = 16, legendaryChance = 4 },
            new RarityData { level = 4, commonChance = 35, rareChance = 38, epicChance = 21, legendaryChance = 6 },
            new RarityData { level = 5, commonChance = 25, rareChance = 40, epicChance = 27, legendaryChance = 8 }

        };
    }

    public ItemRarity GetRandomRarity(int towerLvl)
    {
        // Tablo dışındaki seviyelerde ilk/son satırı kullan.
        towerLvl = Mathf.Clamp(towerLvl, 1, itemRarities.Count);
        float zar = Random.Range(0f, 100f);
        if(zar < itemRarities[towerLvl-1].commonChance)
        {
            return ItemRarity.Common;
        }
        else if (zar < itemRarities[towerLvl-1].commonChance + itemRarities[towerLvl - 1].rareChance)
        {
            return ItemRarity.Rare;
        }
        else if (zar < itemRarities[towerLvl-1].commonChance + itemRarities[towerLvl - 1].rareChance + itemRarities[towerLvl - 1].epicChance)
        {
            return ItemRarity.Epic;
        }
        else
        {
            return ItemRarity.Legendary;
        }
        
    }

    public ItemData GetRandomItem(TowerType towerType, ItemRarity targetRarity)
    {
        if (allItems == null) return null;
        List<ItemData> filteredItems = allItems.FindAll(item => item != null && item.itemOwners != null && item.Rarity == targetRarity && System.Array.Exists(item.itemOwners, owner => owner == towerType));
        // Henüz içeriği olmayan kule/tier için dükkan boş slot gösterebilir.
        if (filteredItems.Count == 0) return null;
        return filteredItems[Random.Range(0, filteredItems.Count)];
    }
}

public class RarityData
{
    public int level;
    public float commonChance;
    public float rareChance;
    public float epicChance;
    public float legendaryChance;
}
