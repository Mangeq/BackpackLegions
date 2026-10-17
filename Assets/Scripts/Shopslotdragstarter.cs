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
            // Dükkan artık kule başına kalıcı - satın alınan item kulenin listesinden de
            // silinmeli, yoksa panel tekrar açılınca slot yeniden dolu görünür.
            ShopSlotUI[] slots = FindObjectOfType<ShopUIManager>().shopSlots;
            int slotIndex = System.Array.IndexOf(slots, shopSlot);
            if (slotIndex >= 0 && UİManager.activeTower != null)
            {
                UİManager.activeTower.myShopItems[slotIndex] = null;
                UİManager.activeTower.ClearOfferReservation(slotIndex);
            }

            shopSlot.isPurchased = true;
            shopSlot.image.enabled = false;
            shopSlot.itemCost.text = "";
            shopSlot.currentItem = null;
            shopSlot.RefreshCard();
            shopSlot.ShowReservation(false);
        }
    }
}
