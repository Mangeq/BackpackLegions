using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopUIManager : MonoBehaviour
{
    public ShopSlotUI[] shopSlots;
    public int rerollCost = 10;
    internal static GameObject ItemShopUI;
    private void Awake()
    {
        ItemShopUI = GameObject.Find("İtemShopUİ");

    }

    private void Start()
    {
        ItemShopUI.SetActive(false);
    }
    public void RefreshShop()
    {
        if (EconomyManager.Gold >= rerollCost && UİManager.activeTower != null)
        {
            EconomyManager.AddGold(-rerollCost);
            UİManager.UpdateGoldText();
            UİManager.activeTower.GenerateShopItems();
            UpdateShopUI();

        }
        else
        {
            return;
        }
    }

    public void UpdateShopUI()
    {
        for (int i = 0; i < shopSlots.Length; i++)
        {
            shopSlots[i].image.enabled = false;
            shopSlots[i].itemCost.text = "0";
            shopSlots[i].currentItem = null;
            // Eksikti: isPurchased hiç sıfırlanmıyordu, bir önceki kulede satılan
            // slot yeni kulede de kalıcı olarak "satıldı" (sürüklenemez) kalıyordu.
            // Her shop yenilenmesi (kule değişimi ya da reroll) fiilen yeni bir
            // dükkan demek, önceki "satıldı" durumu buraya taşınmamalı.
            shopSlots[i].isPurchased = false;

            if (UİManager.activeTower.myShopItems[i] != null)
            {
                shopSlots[i].image.enabled = true;
                shopSlots[i].image.sprite = UİManager.activeTower.myShopItems[i].itemIcon;
                shopSlots[i].itemCost.text = UİManager.activeTower.myShopItems[i].cost.ToString();
                shopSlots[i].currentItem = UİManager.activeTower.myShopItems[i];
            }
        }
    }
}