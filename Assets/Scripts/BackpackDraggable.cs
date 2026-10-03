using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Sürüklenen bir item görselinin davranışı. Hem "backpack içinde zaten yerleşik
/// bir item'ı taşıma" hem de "mağazadan yeni bir item'ı sürükleyip bırakma"
/// senaryosunu TEK bir yerde, aynı mantıkla yönetir - iki ayrı kod yolu (ve iki
/// ayrı yerde tekrar edecek CanPlace/Place çağrısı) yok.
/// </summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class BackpackDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ItemData data;
    private PlacedItem sourcePlacedItem; // null => mağazadan yeni satın alma, dolu => backpack içinde taşıma
    private Vector2Int originBeforeDrag;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas rootCanvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void InitializeAsPlaced(PlacedItem placedItem)
    {
        sourcePlacedItem = placedItem;
        data = placedItem.Data;
    }

    public void InitializeFromShop(ItemData itemData)
    {
        sourcePlacedItem = null;
        data = itemData;
    }

    public void OnBeginDrag(PointerEventData eventData) => BeginDrag();
    public void OnDrag(PointerEventData eventData) => UpdateDrag(eventData);
    public void OnEndDrag(PointerEventData eventData) => FinishDrag(eventData);

    /// <summary>
    /// ShopSlotDragStarter tarafından da manuel çağrılır (mağazadan sürüklerken asıl
    /// drag başlangıcı shop slot'unda oluyor, event'ler bu ghost objesine yönlendiriliyor).
    /// </summary>
    public void BeginDrag()
    {
        canvasGroup.blocksRaycasts = false;

        if (rootCanvas != null)
            transform.SetParent(rootCanvas.transform, true);

        if (sourcePlacedItem != null)
        {
            // Taşıma senaryosu: önce eski pozisyonu boşalt ki item kendi hücrelerine
            // çarparak yer değiştirmeyi engellemesin (eski sistemdeki bug tam burasıydı).
            originBeforeDrag = sourcePlacedItem.Origin;
            BackpackUIManager.Instance.ActiveTower.Backpack.Remove(sourcePlacedItem);
        }
    }

    public void UpdateDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;

        Vector2Int? cell = BackpackUIManager.Instance.ScreenPointToCell(eventData.position, eventData.pressEventCamera);
        if (cell == null)
        {
            BackpackUIManager.Instance.ClearPreview();
            return;
        }

        Vector2Int candidateOrigin = cell.Value - data.centerOffset;
        bool isValid = EvaluatePlacement(candidateOrigin);

        BackpackUIManager.Instance.ShowPlacementPreview(data, candidateOrigin, isValid);
    }

    /// <returns>Item başarıyla yerleştiyse true.</returns>
    public bool FinishDrag(PointerEventData eventData)
    {
        BackpackUIManager.Instance.ClearPreview();
        canvasGroup.blocksRaycasts = true;

        Backpack backpack = BackpackUIManager.Instance.ActiveTower.Backpack;
        Vector2Int? cell = BackpackUIManager.Instance.ScreenPointToCell(eventData.position, eventData.pressEventCamera);

        bool placed = false;

        if (cell.HasValue)
        {
            Vector2Int candidateOrigin = cell.Value - data.centerOffset;

            if (EvaluatePlacement(candidateOrigin))
            {
                // sourcePlacedItem varsa (taşıma) ücret yok; yoksa (mağaza) altın gerekiyor.
                bool canCommit = sourcePlacedItem != null || EconomyManager.TrySpendGold(data.cost);

                if (canCommit)
                {
                    backpack.Place(data, candidateOrigin);
                    placed = true;
                }
            }
        }

        if (!placed && sourcePlacedItem != null)
            backpack.Place(data, originBeforeDrag); // taşıma başarısız oldu, eski yerine geri koy

        BackpackUIManager.Instance.RefreshItemVisuals();

        // Yerleşti/taşındı/geri alındı fark etmez - backpack'in son hali her zaman
        // stat'a yansımalı. Küçük bir grid için maliyeti önemsiz, koşulsuz çağırmak
        // "item ekledim ama stat güncellenmedi" tarzı bir senkron hatasını baştan eler.
        BackpackUIManager.Instance.ActiveTower.CalculateStats();

        if (sourcePlacedItem == null)
            Destroy(gameObject); // mağaza ghost'u geçiciydi, işi bitti (başarılıysa gerçek görsel Refresh ile oluştu)

        return placed;
    }

    private bool EvaluatePlacement(Vector2Int candidateOrigin)
    {
        Backpack backpack = BackpackUIManager.Instance.ActiveTower.Backpack;
        bool fitsGrid = backpack.CanPlace(data, candidateOrigin);
        bool canAfford = sourcePlacedItem != null || EconomyManager.Gold >= data.cost;
        return fitsGrid && canAfford;
    }
}