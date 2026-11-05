using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Mağaza slot'una eklenen ince bir "adapter" component. Mevcut ShopSlotUI'yi hiç
/// değiştirmiyor, sadece sürükleme başladığında geçici bir BackpackDraggable ghost'u
/// oluşturup pointer event'lerini ona yönlendiriyor. Tüm yerleştirme/altın mantığı
/// BackpackDraggable'da yaşıyor - burada mantık tekrarı yok.
///
/// Kurulum: Shop slot prefab'ındaki ShopSlotUI'nin olduğu GameObject'e bu component'i
/// de ekle.
/// </summary>
[RequireComponent(typeof(ShopSlotUI))]
public class ShopSlotDragStarter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ShopSlotUI shopSlot;
    private BackpackDraggable activeGhost;
    private Tower purchaseOwner;
    private ItemData purchaseItem;
    private int purchaseIndex;

    private void Awake()
    {
        shopSlot = GetComponent<ShopSlotUI>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(eventData.button!=PointerEventData.InputButton.Left)return;
        if (!GameManager.CanEditItems) return;
        if (shopSlot.currentItem == null || shopSlot.isPurchased)
            return;

        if (BackpackUIManager.Instance.ActiveTower == null)
            return;
        purchaseOwner=BackpackUIManager.Instance.ActiveTower;
        purchaseItem=shopSlot.currentItem;
        if(EconomyManager.Gold<purchaseItem.cost){
            var label=shopSlot.transform.Find("ReferenceCost");
            LegionUiMotion.Pulse(label?label:shopSlot.itemCost?shopSlot.itemCost.transform:shopSlot.transform,true);
            return;
        }
        purchaseIndex=System.Array.IndexOf(FindObjectOfType<ShopUIManager>().shopSlots,shopSlot);

        GameObject ghost = Instantiate(BackpackUIManager.Instance.itemVisualPrefab, BackpackUIManager.Instance.transform.root);

        RectTransform rt = ghost.GetComponent<RectTransform>();
        Vector2Int size = ItemShapeUtility.GetBoundingSize(shopSlot.currentItem.itemShape);
        rt.sizeDelta = new Vector2(size.x * BackpackUIManager.Instance.cellSize, size.y * BackpackUIManager.Instance.cellSize);
        ghost.GetComponent<Image>().sprite = ReferencePanelAssets.IconFor(shopSlot.currentItem);

        activeGhost = ghost.GetComponent<BackpackDraggable>();
        activeGhost.InitializeFromShop(shopSlot.currentItem);
        activeGhost.BeginDrag();
        activeGhost.UpdateDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        activeGhost?.UpdateDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bool success = activeGhost != null && activeGhost.FinishDrag(eventData);
        activeGhost = null;

        if (success)
        {
            purchaseOwner.CompleteShopPurchase(purchaseIndex,purchaseItem);
            FindObjectOfType<ShopUIManager>().UpdateShopUI();
        }
    }
}
