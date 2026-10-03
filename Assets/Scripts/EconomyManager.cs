using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static int Gold = 1000;

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