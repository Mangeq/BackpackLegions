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
public class BackpackDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private ItemData data;
    public ItemData Item => data;
    private PlacedItem sourcePlacedItem; // null => mağazadan yeni satın alma, dolu => backpack içinde taşıma
    private Vector2Int originBeforeDrag;
    private int rotationBeforeDrag;
    public int Rotation { get; private set; }
    private Vector2 pointerPosition;
    private Camera pointerCamera;
    private Vector2Int DragAnchor => ItemShapeUtility.RotateCell(data.centerOffset,
        ItemShapeUtility.GetBoundingSize(data.itemShape), Rotation);
    private Tower dragOwner;
    private Backpack sourceBag;
    private TowerItemEvolution.Delivery delivery;
    private Tower deliveryOwner;
    public void InitializeDelivery(Tower owner, TowerItemEvolution.Delivery entry)
    { deliveryOwner=owner;delivery=entry;data=entry.Item;Rotation=0; }
    public void OnPointerEnter(PointerEventData e)
    {
        if (!IsDragging && sourcePlacedItem != null && sourceBag == BackpackUIManager.Instance.ActiveTower.Backpack)
            BackpackUIManager.Instance.ShowSynergyCells(data,sourcePlacedItem.Origin,Rotation);
    }
    public void OnPointerExit(PointerEventData e) { if(!IsDragging) BackpackUIManager.Instance.ClearPreview(); }
    private bool isDragging;
    private static BackpackDraggable activeDrag;
    public static bool IsDragging => activeDrag != null;

    public static void CancelActiveDrag()
    {
        if (activeDrag != null) activeDrag.CancelDrag();
    }

    private void CancelDrag()
    {
        if (!isDragging) return;
        isDragging = false;
        if (activeDrag == this) activeDrag = null;
        canvasGroup.blocksRaycasts = true;
        if (sourcePlacedItem != null && dragOwner != null)
            sourceBag.Place(data, originBeforeDrag, rotationBeforeDrag);
        var ui = BackpackUIManager.Instance;
        if (ui != null)
        {
            ui.ClearPreview();
            ui.ResetSellPreview();
            if (ui.ActiveTower == dragOwner && dragOwner != null) ui.RefreshItemVisuals();
        }
        if (dragOwner != null) dragOwner.CalculateStats();
        if (sourcePlacedItem == null) Destroy(gameObject);
    }

    private void Update()
    {
        if (!isDragging) return;
        if (!GameManager.CanEditItems || BackpackUIManager.Instance.ActiveTower != dragOwner) { CancelDrag(); return; }
        int steps = Input.GetKeyDown(KeyCode.R) ? 1 : 0;
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0f) steps += scroll > 0f ? 1 : -1;
        if (steps != 0) Rotate(steps);
    }

    public void Rotate(int steps)
    {
        if (!isDragging) return;
        if (!GameManager.CanEditItems || BackpackUIManager.Instance.ActiveTower != dragOwner) { CancelDrag(); return; }
        Rotation = ItemShapeUtility.NormalizeRotation(Rotation + steps);
        BackpackUIManager.Instance.ConfigureItemVisual(rectTransform, data, Rotation, true);
        RefreshDragPreview();
    }

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas rootCanvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void InitializeAsPlaced(PlacedItem placedItem, Backpack ownerBag = null)
    {
        sourcePlacedItem = placedItem;
        sourceBag = ownerBag;
        data = placedItem.Data;
        Rotation = placedItem.Rotation;
    }

    public void InitializeFromShop(ItemData itemData)
    {
        sourcePlacedItem = null;
        data = itemData;
        Rotation = 0;
    }

    public void OnBeginDrag(PointerEventData eventData) { BeginDrag(); UpdateDrag(eventData); }
    public void OnDrag(PointerEventData eventData) => UpdateDrag(eventData);
    public void OnEndDrag(PointerEventData eventData) => FinishDrag(eventData);

    /// <summary>
    /// ShopSlotDragStarter tarafından da manuel çağrılır (mağazadan sürüklerken asıl
    /// drag başlangıcı shop slot'unda oluyor, event'ler bu ghost objesine yönlendiriliyor).
    /// </summary>
    public void BeginDrag()
    {
        if (isDragging || !GameManager.CanEditItems || BackpackUIManager.Instance.ActiveTower == null) return;
        CancelActiveDrag();
        dragOwner = BackpackUIManager.Instance.ActiveTower;
        if(delivery!=null && (deliveryOwner!=dragOwner || !dragOwner.Evolution.Contains(delivery)))return;
        if (sourcePlacedItem != null)
        {
            if (sourceBag == null) sourceBag = dragOwner.Backpack;
            if (sourceBag != dragOwner.Backpack && sourceBag != dragOwner.Storage) return;
            bool found = false;
            foreach (var item in sourceBag.Items) if (item == sourcePlacedItem) found = true;
            if (!found) return;
        }
        isDragging = true;
        activeDrag = this;
        canvasGroup.blocksRaycasts = false;
        pointerPosition = Input.mousePosition;
        pointerCamera = rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay ? rootCanvas.worldCamera : null;

        if (rootCanvas != null)
            transform.SetParent(rootCanvas.transform, true);

        if (sourcePlacedItem != null)
        {
            // Taşıma senaryosu: önce eski pozisyonu boşalt ki item kendi hücrelerine
            // çarparak yer değiştirmeyi engellemesin (eski sistemdeki bug tam burasıydı).
            originBeforeDrag = sourcePlacedItem.Origin;
            rotationBeforeDrag = sourcePlacedItem.Rotation;
            sourceBag.Remove(sourcePlacedItem);
        }
        BackpackUIManager.Instance.ConfigureItemVisual(rectTransform, data, Rotation, true);
        UpdateSellPreview();
    }

    public void UpdateDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        if (!GameManager.CanEditItems || BackpackUIManager.Instance.ActiveTower != dragOwner) { CancelDrag(); return; }
        pointerPosition = eventData.position;
        pointerCamera = eventData.pressEventCamera;
        RefreshDragPreview();
    }

    private void RefreshDragPreview()
    {
        UpdateSellPreview();
        RectTransform parent = rectTransform.parent as RectTransform;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(parent, pointerPosition, pointerCamera, out Vector3 world))
            rectTransform.position = world;

        Backpack targetBag;
        Vector2Int? cell = BackpackUIManager.Instance.FindDropCell(pointerPosition, pointerCamera, out targetBag);
        if (cell == null)
        {
            BackpackUIManager.Instance.ClearPreview();
            return;
        }

        Vector2Int candidateOrigin = cell.Value - DragAnchor;
        bool isValid = EvaluatePlacement(targetBag, candidateOrigin);

        BackpackUIManager.Instance.ShowPlacementPreview(data, candidateOrigin, isValid, Rotation, targetBag);
    }

    private bool CanSell => sourcePlacedItem != null && dragOwner != null &&
        data != null && !dragOwner.IsMainItem(data) &&
        (sourceBag == dragOwner.Backpack || sourceBag == dragOwner.Storage);

    private void UpdateSellPreview()
    {
        var ui = BackpackUIManager.Instance;
        string reason = delivery != null ? "TESLİM · Önce çantaya veya depoya yerleştir" : sourcePlacedItem == null ? "SATILAMAZ · Önce eşyayı satın almalısın" : "ANA İTEM · Satılamaz ve depoya konulamaz";
        ui.ShowSellPreview(data, CanSell, reason, ui.IsOverSellArea(pointerPosition, pointerCamera));
    }

    /// <returns>Item başarıyla yerleştiyse veya satıldıysa true.</returns>
    public bool FinishDrag(PointerEventData eventData)
    {
        if (!isDragging) return false;
        if (!GameManager.CanEditItems || BackpackUIManager.Instance.ActiveTower != dragOwner) { CancelDrag(); return false; }
        isDragging = false;
        if (activeDrag == this) activeDrag = null;
        BackpackUIManager.Instance.ClearPreview();
        BackpackUIManager.Instance.ResetSellPreview();
        canvasGroup.blocksRaycasts = true;

        Backpack backpack;
        Vector2Int? cell = BackpackUIManager.Instance.FindDropCell(eventData.position, eventData.pressEventCamera, out backpack);

        bool placed = false;

        if (BackpackUIManager.Instance.IsOverSellArea(eventData.position, eventData.pressEventCamera))
        {
            if (CanSell)
            {
                // BeginDrag kaynağı zaten boşalttı. Satışta geri koymadan yalnız bir kez ödeme yap.
                EconomyManager.AddGold(EconomyManager.GetSellValue(data));
                placed = true;
            }
        }
        else if (cell.HasValue)
        {
            Vector2Int candidateOrigin = cell.Value - DragAnchor;

            if (EvaluatePlacement(backpack, candidateOrigin))
            {
                // sourcePlacedItem varsa (taşıma) ücret yok; yoksa (mağaza) altın gerekiyor.
                bool canCommit = delivery != null ? dragOwner.Evolution.Claim(delivery) : sourcePlacedItem != null || EconomyManager.TrySpendGold(data.cost);

                if (canCommit)
                {
                    backpack.Place(data, candidateOrigin, Rotation);
                    placed = true;
                }
            }
        }

        if (!placed && sourcePlacedItem != null)
            sourceBag.Place(data, originBeforeDrag, rotationBeforeDrag); // Kaynak alan, konum ve yönü birlikte geri al.

        BackpackUIManager.Instance.RefreshItemVisuals();

        // Yerleşti/taşındı/geri alındı fark etmez - backpack'in son hali her zaman
        // stat'a yansımalı. Küçük bir grid için maliyeti önemsiz, koşulsuz çağırmak
        // "item ekledim ama stat güncellenmedi" tarzı bir senkron hatasını baştan eler.
        dragOwner.CalculateStats();

        if (sourcePlacedItem == null)
            Destroy(gameObject); // mağaza ghost'u geçiciydi, işi bitti (başarılıysa gerçek görsel Refresh ile oluştu)

        return placed;
    }

    private bool EvaluatePlacement(Backpack backpack, Vector2Int candidateOrigin)
    {
        if (backpack == null || (backpack != dragOwner.Backpack && backpack != dragOwner.Storage)) return false;
        // Kule türünü belirleyen ana item çantada kalır. Önizleme ve bırakma aynı kuralı kullanır.
        if (backpack == dragOwner.Storage && dragOwner.IsMainItem(data)) return false;
        bool fitsGrid = backpack.CanPlace(data, candidateOrigin, Rotation);
        bool canAfford = sourcePlacedItem != null || delivery != null || EconomyManager.Gold >= data.cost;
        return fitsGrid && canAfford;
    }
}
