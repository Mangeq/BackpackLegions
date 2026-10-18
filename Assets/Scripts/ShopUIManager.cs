using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopUIManager : MonoBehaviour
{
    public ShopSlotUI[] shopSlots;
    public int rerollCost = 10;
    public UnityEngine.UI.Button rerollButton;
    public UnityEngine.UI.Text interactionStatus;
    public UnityEngine.UI.Button freezeButton;
    public UnityEngine.UI.Text freezeButtonText;
    public UnityEngine.UI.Text offerCountText;
    public UnityEngine.UI.ScrollRect offersScroll;
    internal static GameObject ItemShopUI;
    [SerializeField] private GameObject itemShopPanel;
    private bool sceneInitialized;
    private Tower renderedTower;
    private bool renderedElementShop;
    private void Awake()
    {
        InitializeForScene();
    }

    private void Start()
    {
        InitializeForScene();
    }
    public void InitializeForScene()
    {
        if(sceneInitialized)return;
        sceneInitialized=true;
        ItemShopUI = itemShopPanel != null ? itemShopPanel : GameObject.Find("İtemShopUİ");
        ItemShopUI.SetActive(false);
    }
    public void RefreshShop()
    {
        if (!GameManager.CanEditItems || UİManager.activeTower == null || UİManager.activeTower.IsShopFrozen) return;
        if (EconomyManager.Gold >= rerollCost && UİManager.activeTower != null)
        {
            BackpackDraggable.CancelActiveDrag();
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

    private void Update()
    {
        bool canEdit = GameManager.CanEditItems;
        if (offersScroll != null) offersScroll.enabled = !BackpackDraggable.IsDragging;
        var tower = UİManager.activeTower;
        if(tower!=null&&(renderedTower!=tower||renderedElementShop!=tower.IsElementShop))UpdateShopUI();
        bool frozen = false;
        if (rerollButton != null) rerollButton.interactable = canEdit && tower != null && !frozen && EconomyManager.Gold >= rerollCost;
        if (freezeButton != null) freezeButton.gameObject.SetActive(false);
        if (freezeButtonText != null) freezeButtonText.text = frozen ? "Dondurmayı kaldır" : "Dükkanı dondur";
        if (interactionStatus != null)
            interactionStatus.text = !canEdit ? "Eşya işlemleri kilitli · Hazırlığı bekle" : frozen ? "Bu kulenin teklifleri korunuyor" : "Hazırlık · Eşyaları düzenleyebilirsin";
    }

    public void ToggleFreeze()
    {
        if (!GameManager.CanEditItems || UİManager.activeTower == null) return;
        UİManager.activeTower.ToggleShopFreeze();
        Update();
    }

    public void RefreshShopsForPreparation()
    {
        foreach (var tower in FindObjectsOfType<Tower>())
            if (tower.Backpack != null && !tower.IsShopFrozen && !tower.IsElementShop) tower.GenerateShopItems();
        if (UİManager.activeTower != null) UpdateShopUI();
    }

    public void UpdateShopUI()
    {
        if (UİManager.activeTower == null) return;
        renderedTower=UİManager.activeTower;renderedElementShop=renderedTower.IsElementShop;
        int count = UİManager.activeTower.myShopItems.Length;
        if (offerCountText != null)
        {
            var tower = UİManager.activeTower;
            offerCountText.text=tower.IsElementShop?"4 teklif · Elementi çantana sürükle":count+" teklif · Sağ tık: rezerve et";
        }
        if (count > shopSlots.Length && shopSlots.Length > 0)
        {
            var slots = new List<ShopSlotUI>(shopSlots);
            while (slots.Count < count) slots.Add(Instantiate(shopSlots[0], shopSlots[0].transform.parent));
            shopSlots = slots.ToArray();
        }
        for (int i = 0; i < shopSlots.Length; i++)
        {
            shopSlots[i].gameObject.SetActive(i < count);
            if (i >= count) continue;
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
                shopSlots[i].image.sprite = ReferencePanelAssets.IconFor(UİManager.activeTower.myShopItems[i]);
                shopSlots[i].itemCost.text = UİManager.activeTower.myShopItems[i].cost.ToString();
                shopSlots[i].currentItem = UİManager.activeTower.myShopItems[i];
            }
            shopSlots[i].RefreshCard();
            shopSlots[i].ShowReservation(UİManager.activeTower.IsOfferReserved(i)&&shopSlots[i].currentItem!=null);
        }
        if (offersScroll != null) offersScroll.verticalNormalizedPosition = 1f;
    }
}
