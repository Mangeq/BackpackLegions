using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static int Gold = 1000;

    // Pozitif tam sayıda bölme küsuratı aşağı yuvarlar (151 -> 75).
    public static int GetSellValue(ItemData item) => item == null ? 0 : Mathf.Max(0, item.cost) / 2;

    public static void AddGold(int amount)
    {
        Gold += amount;
        UİManager.UpdateGoldText();
    }

    /// <summary>Yeterli altın varsa düşer ve true döner, yoksa hiçbir şey yapmadan false döner.</summary>
    public static bool TrySpendGold(int amount)
    {
        if (Gold < amount)
            return false;

        Gold -= amount;
        UİManager.UpdateGoldText();
        return true;
    }
}
