using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Backpack'in GÖRSEL temsilinden sorumlu tek sınıf: grid hücrelerini çizer,
/// yerleşik item görsellerini gerçek boyutlarına göre (width x height hücre)
/// boyutlandırıp konumlandırır, sürükleme sırasında yeşil/kırmızı highlight
/// gösterir, ekran koordinatını grid hücresine çevirir.
///
/// ÖNEMLİ: Hiçbir yerleştirme KARARI vermez (o iş Backpack sınıfının) ve hiçbir
/// altın kontrolü yapmaz (o iş EconomyManager'ın). Sadece "göster" katmanı.
/// </summary>
public class BackpackUIManager : MonoBehaviour
{
    public static BackpackUIManager Instance;
    public Font itemStatusFont;

    [Header("UI Referansları")]
    public GameObject backpackPanel;
    public GameObject cellPrefab;        // Image + BackpackCellView içermeli
    public GameObject itemVisualPrefab;  // Image + CanvasGroup + BackpackDraggable içermeli
    public RectTransform gridContainer;
    public RectTransform storageGridContainer;
    public GameObject combinedPanel;
    public RectTransform sellArea;
    public Text sellLabel;
    public RectTransform deliveryContainer;
    public Text deliveryLabel;
    public Text evolutionStatus;
    private int deliveryIndex;
    private GameObject deliveryVisual;
    public void NextDelivery()
    {
        if(BackpackDraggable.IsDragging)return;
        deliveryIndex++;RefreshDeliveries();
    }
    private void RefreshDeliveries()
    {
        if(deliveryVisual!=null)Destroy(deliveryVisual);
        if(deliveryContainer==null||ActiveTower==null||ActiveTower.Evolution==null)return;
        var entries=ActiveTower.Evolution.Deliveries;
        if(deliveryLabel!=null)deliveryLabel.text=entries.Count==0?"Teslim bekleyen yok":"Teslim "+(deliveryIndex%entries.Count+1)+"/"+entries.Count+" · sürükle →";
        if(entries.Count==0)return;
        deliveryIndex%=entries.Count;
        deliveryVisual=Instantiate(itemVisualPrefab,deliveryContainer);
        var rt=deliveryVisual.GetComponent<RectTransform>();rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f);rt.sizeDelta=new Vector2(36,36);rt.anchoredPosition=Vector2.zero;
        var image=deliveryVisual.GetComponent<Image>();image.sprite=entries[deliveryIndex].Item.itemIcon;image.preserveAspect=true;
        deliveryVisual.GetComponent<BackpackDraggable>().InitializeDelivery(ActiveTower,entries[deliveryIndex]);
    }
    private void RefreshEvolutionStatus()
    {
        if(evolutionStatus==null||!evolutionStatus.gameObject.activeInHierarchy||ActiveTower==null||ActiveTower.Evolution==null)return;
        var pairs=GameManager.CanEditItems?ActiveTower.Evolution.FindPairs():new List<TowerItemEvolution.Pair>(ActiveTower.Evolution.Pending);
        if(pairs.Count==0){evolutionStatus.text="EVRİM · Tarif eşyalarını yan yana yerleştir\nSinerji kareleri: eşyanın üzerine gel";return;}
        var text=new System.Text.StringBuilder("WAVE SONUNDA EVRİM · "+pairs.Count+" tarif\n");
        foreach(var pair in pairs)text.Append(pair.first.Data.Name+" + "+pair.second.Data.Name+" → "+pair.recipe.result.Name+"\n");
        evolutionStatus.text=text.ToString().TrimEnd();
    }

    public bool IsOverSellArea(Vector2 position, Camera camera)
    {
        return sellArea != null && sellArea.gameObject.activeInHierarchy &&
            RectTransformUtility.RectangleContainsScreenPoint(sellArea, position, camera);
    }

    public void ShowSellPreview(ItemData item, bool allowed, string blockedReason, bool hovered)
    {
        if (sellLabel != null)
            sellLabel.text = allowed ? "SAT  ·  " + item.Name + "  +" + EconomyManager.GetSellValue(item) + " altın  ·  Buraya bırak" : blockedReason;
        if (sellArea != null)
        {
            var image = sellArea.GetComponent<Image>();
            if (image != null) image.color = hovered
                ? (allowed ? new Color(.22f, .36f, .24f) : new Color(.38f, .18f, .18f))
                : new Color(.16f, .19f, .21f);
            if(GetComponent<TowerPanelLayout>()!=null&&image!=null)image.color=hovered?new Color(allowed?.2f:.8f,allowed?.8f:.2f,.2f,.22f):Color.clear;
        }
    }

    public void ResetSellPreview()
    {
        if (sellLabel != null) sellLabel.text = "SAT  ·  Eşyayı buraya bırak  ·  %50 geri ödeme";
        if (sellArea != null)
        {
            var image = sellArea.GetComponent<Image>();
            if (image != null) image.color = GetComponent<TowerPanelLayout>()!=null?Color.clear:new Color(.16f, .19f, .21f);
        }
    }

    [Header("Hücre Ayarları")]
    public float cellSize = 64f;

    [Header("Highlight Renkleri")]
    public Color validColor = new Color(0.30f, 0.85f, 0.40f, 0.55f);
    public Color invalidColor = new Color(0.90f, 0.25f, 0.25f, 0.55f);
    // Tamamen şeffaf: hücreler normalde hiçbir renk vermez, panelin kendi
    // arka plan görseli (grid çizgileri, doku vb.) hep olduğu gibi görünür.
    // Sadece sürüklerken ShowPlacementPreview ile geçici olarak boyanırlar.
    public Color defaultCellColor = new Color(1f, 1f, 1f, 0f);

    public Tower ActiveTower { get; private set; }

    private BackpackCellView[,] cellViews;
    private BackpackCellView[,] storageCellViews;
    private readonly Dictionary<PlacedItem, GameObject> itemVisuals = new Dictionary<PlacedItem, GameObject>();

    private void Awake()
    {
        Instance = this;

        // Aşağıdaki koordinat matematiği gridContainer'ın pivot'unun sol-üst (0,1)
        // olmasını varsayıyor. Editor'de unutulursa diye burada garanti altına alıyoruz.
        gridContainer.pivot = new Vector2(0f, 1f);
    }

    private void Start()
    {
        InitializeForScene();
    }

    private bool sceneInitialized;
    public void InitializeForScene()
    {
        if(sceneInitialized)return;
        sceneInitialized=true;
        Instance=this;
        gridContainer.pivot=new Vector2(0,1);
        TowerPanelLayout.Install(this);
        backpackPanel.SetActive(false);
        if (combinedPanel != null) combinedPanel.SetActive(false);
    }

    private void Update()
    {
        if(!BackpackDraggable.IsDragging)RefreshEvolutionStatus();
    }

    public void OpenBackpack(Tower tower)
    {
        UİManager.NotifyPanelOpened();
        var layout=GetComponent<TowerPanelLayout>();
        if(layout!=null)layout.PrepareToOpen(tower);
        if (ActiveTower == tower && backpackPanel.activeSelf)
        {
            if (ShopUIManager.ItemShopUI != null) ShopUIManager.ItemShopUI.SetActive(GameManager.CanEditItems);
            return;
        }

        BackpackDraggable.CancelActiveDrag();
        ActiveTower = tower;
        BuildGrid(tower.Backpack.Width, tower.Backpack.Height);
        if (storageGridContainer != null) BuildStorageGrid(tower.Storage);
        RefreshItemVisuals();
        backpackPanel.SetActive(true);
        if (combinedPanel != null) GameAudio.SetPanel(combinedPanel, true);
        if (ShopUIManager.ItemShopUI != null) ShopUIManager.ItemShopUI.SetActive(GameManager.CanEditItems);
    }

    public void CloseBackpack()
    {
        BackpackDraggable.CancelActiveDrag();
        backpackPanel.SetActive(false);
        if (combinedPanel != null) GameAudio.SetPanel(combinedPanel, false);
        if (ShopUIManager.ItemShopUI != null) ShopUIManager.ItemShopUI.SetActive(false);
    }

    private void BuildStorageGrid(Backpack bag)
    {
        foreach (Transform child in storageGridContainer) if(child.name!="StorageCells") Destroy(child.gameObject);
        if(bag.IsLooseStorage){storageCellViews=null;return;}
        storageCellViews = new BackpackCellView[bag.Width, bag.Height];
        for (int y = 0; y < bag.Height; y++) for (int x = 0; x < bag.Width; x++)
        {
            var go = Instantiate(cellPrefab, storageGridContainer);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0, 1);
            rt.sizeDelta = new Vector2(cellSize, cellSize);
            rt.anchoredPosition = new Vector2(x * cellSize, -y * cellSize);
            var view = go.GetComponent<BackpackCellView>();
            view.Setup(x, y);
            view.SetHighlight(CellHighlightState.None);
            storageCellViews[x, y] = view;
        }
    }

    private void BuildGrid(int width, int height)
    {
        foreach (Transform child in gridContainer)
            Destroy(child.gameObject);

        cellViews = new BackpackCellView[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject cellGO = Instantiate(cellPrefab, gridContainer);
                RectTransform rt = cellGO.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0f, 1f);
                rt.anchorMax = new Vector2(0f, 1f);
                rt.pivot = new Vector2(0f, 1f);
                rt.sizeDelta = new Vector2(cellSize, cellSize);
                rt.anchoredPosition = new Vector2(x * cellSize, -y * cellSize);

                BackpackCellView view = cellGO.GetComponent<BackpackCellView>();
                view.Setup(x, y);
                view.SetHighlight(CellHighlightState.None); // spawn anında hemen şeffaf yap - prefab'ın ham rengini bekletme
                cellViews[x, y] = view;
            }
        }
    }

    /// <summary>Tüm item görsellerini siler ve ActiveTower.Backpack'in güncel durumuna göre yeniden çizer.</summary>
    public void RefreshItemVisuals()
    {
        foreach (GameObject visual in itemVisuals.Values)
            Destroy(visual);

        itemVisuals.Clear();
        var pile=storageGridContainer!=null?storageGridContainer.GetComponent<StorageGridUI>():null;
        if(pile!=null)pile.Prepare(ActiveTower.Storage.Items.Count);

        foreach (PlacedItem placed in ActiveTower.Backpack.Items)
            SpawnItemVisual(placed, ActiveTower.Backpack, gridContainer);
        if (storageGridContainer != null)
            foreach (PlacedItem placed in ActiveTower.Storage.Items)
                SpawnItemVisual(placed, ActiveTower.Storage, storageGridContainer);
        RefreshDeliveries();
        RefreshEvolutionStatus();
        if(ActiveTower.Storage.IsLooseStorage&&storageGridContainer!=null)
        {
            Canvas.ForceUpdateCanvases();
            var scroll=storageGridContainer.GetComponentInParent<ScrollRect>(true);
            if(scroll!=null)scroll.verticalNormalizedPosition=1;
        }
    }

    public void MoveAllToStorage()
    {
        if(ActiveTower==null||!GameManager.CanEditItems)return;
        BackpackDraggable.CancelActiveDrag();
        foreach(var item in new List<PlacedItem>(ActiveTower.Backpack.Items))
        {
            ActiveTower.Backpack.Remove(item);item.HasStoragePosition=false;
            ActiveTower.Storage.PlaceExisting(item,Vector2Int.zero,item.Rotation);
        }
        ActiveTower.CalculateStats();RefreshItemVisuals();
    }
    public void RememberStorageDrop(PlacedItem item,Vector2 point,Camera cam)
    {
        Vector2 local;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(storageGridContainer,point,cam,out local))
        {item.StoragePosition=new Vector2(local.x,-local.y);item.HasStoragePosition=true;}
    }

    private void SpawnItemVisual(PlacedItem placed, Backpack bag, RectTransform container)
    {
        GameObject visualGO = Instantiate(itemVisualPrefab, container);
        RectTransform rt = visualGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);

        Vector2Int size = placed.BoundingSize;
        ConfigureItemVisual(rt, placed.Data, placed.Rotation, false);
        rt.anchoredPosition = new Vector2((placed.Origin.x + size.x * 0.5f) * cellSize,
            -(placed.Origin.y + size.y * 0.5f) * cellSize);

        visualGO.GetComponent<Image>().sprite = ReferencePanelAssets.IconFor(placed.Data);
        if(!placed.Data.isElementCatalyst&&placed.Data.element!=ElementType.Normal)
            visualGO.GetComponent<Image>().color=placed.Data.element==ElementType.Fire?new Color(1,.58f,.24f)
                :placed.Data.element==ElementType.Water?new Color(.45f,.75f,1)
                :placed.Data.element==ElementType.Earth?new Color(.8f,.63f,.35f):new Color(.65f,1,1);
        if (!ActiveTower.IsItemActive(placed.Data))
        {
            visualGO.GetComponent<Image>().color = new Color(.45f, .45f, .45f, .8f);
            var labelGO = new GameObject("IncompatibleLabel", typeof(RectTransform), typeof(UnityEngine.UI.Text));
            labelGO.transform.SetParent(visualGO.transform, false);
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = labelRect.anchorMax = labelRect.pivot = new Vector2(.5f, .5f);
            labelRect.sizeDelta = new Vector2(Mathf.Max(80f, size.x * cellSize - 8f), 36f);
            labelRect.localRotation = Quaternion.Euler(0, 0, 90f * placed.Rotation);
            var label = labelGO.GetComponent<UnityEngine.UI.Text>();
            label.font = itemStatusFont != null ? itemStatusFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 13;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = new Color(1f, .8f, .6f);
            label.text = "İŞLEVSİZ\nBonus yok";
            label.raycastTarget = false;
        }

        BackpackDraggable draggable = visualGO.GetComponent<BackpackDraggable>();
        draggable.InitializeAsPlaced(placed, bag);
        if(bag.IsLooseStorage)
        {
            var pile=container.GetComponent<StorageGridUI>();
            if(pile!=null)pile.Register(placed,rt);
        }

        itemVisuals[placed] = visualGO;
        int attachmentIndex=0;
        foreach(var part in placed.Attachments){
            var badge=Instantiate(itemVisualPrefab,visualGO.transform);badge.name="Attached_"+part.Data.Name;
            var badgeRect=badge.GetComponent<RectTransform>();badgeRect.anchorMin=badgeRect.anchorMax=new Vector2(1,1);badgeRect.pivot=new Vector2(1,1);badgeRect.sizeDelta=new Vector2(32,32);badgeRect.anchoredPosition=new Vector2(-2,-2-attachmentIndex*34);badgeRect.localRotation=Quaternion.Euler(0,0,90f*placed.Rotation);
            var image=badge.GetComponent<UnityEngine.UI.Image>();image.sprite=part.Data.itemIcon;image.preserveAspect=true;image.color=ActiveTower.IsItemActive(placed.Data)&&ActiveTower.IsItemActive(part.Data)?Color.white:new Color(.45f,.45f,.45f,.8f);
            if (!ActiveTower.IsItemActive(placed.Data) || !ActiveTower.IsItemActive(part.Data))
            {
                var status = new GameObject("IncompatibleLabel", typeof(RectTransform), typeof(UnityEngine.UI.Text));
                status.transform.SetParent(badge.transform, false);
                var rect = status.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
                rect.offsetMin = rect.offsetMax = Vector2.zero;
                var text = status.GetComponent<UnityEngine.UI.Text>();
                text.font = itemStatusFont != null ? itemStatusFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                text.fontSize = 7; text.alignment = TextAnchor.MiddleCenter;
                text.color = new Color(1f, .8f, .6f); text.text = "İŞLEVSİZ"; text.raycastTarget = false;
            }
            badge.GetComponent<BackpackDraggable>().InitializeAsPlaced(part,bag);itemVisuals[part]=badge;attachmentIndex++;
        }
    }

    /// <summary>Ekran koordinatını (mouse/dokunuş) grid hücre koordinatına çevirir. Grid dışındaysa null.</summary>
    public Vector2Int? ScreenPointToCell(Vector2 screenPoint, Camera cam)
    {
        if (ActiveTower == null)
            return null;
        return CellInGrid(gridContainer, ActiveTower.Backpack, screenPoint, cam);
    }

    public Vector2Int? FindDropCell(Vector2 point, Camera cam, out Backpack bag)
    {
        bag = null;
        if (ActiveTower == null) return null;
        var cell = CellInGrid(gridContainer, ActiveTower.Backpack, point, cam);
        if (cell.HasValue) { bag = ActiveTower.Backpack; return cell; }
        cell = CellInGrid(storageGridContainer, ActiveTower.Storage, point, cam);
        if (cell.HasValue) bag = ActiveTower.Storage;
        return cell;
    }

    private Vector2Int? CellInGrid(RectTransform container, Backpack bag, Vector2 screenPoint, Camera cam)
    {
        if (container == null || !container.gameObject.activeInHierarchy || bag == null) return null;
        var scroll=container.GetComponentInParent<UnityEngine.UI.ScrollRect>(true);
        if(scroll!=null&&scroll.viewport!=null&&
            !RectTransformUtility.RectangleContainsScreenPoint(scroll.viewport,screenPoint,cam))return null;
        if(bag.IsLooseStorage)return RectTransformUtility.RectangleContainsScreenPoint(container,screenPoint,cam)?Vector2Int.zero:(Vector2Int?)null;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(container, screenPoint, cam, out Vector2 local))
            return null;

        int x = Mathf.FloorToInt(local.x / cellSize);
        int y = Mathf.FloorToInt(-local.y / cellSize);

        if (x < 0 || x >= bag.Width || y < 0 || y >= bag.Height)
            return null;

        return new Vector2Int(x, y);
    }

    /// <summary>Verilen item'ın origin'den başlayarak kaplayacağı hücreleri geçerliyse yeşil, değilse kırmızı boyar.</summary>
    public void ShowPlacementPreview(ItemData data, Vector2Int origin, bool isValid, int rotation = 0, Backpack bag = null)
    {
        ClearPreview();

        var views = bag != null && bag == ActiveTower.Storage ? storageCellViews : cellViews;
        if (views == null)
            return;

        foreach (Vector2Int offset in ItemShapeUtility.GetRotatedShape(data, rotation))
        {
            Vector2Int cell = origin + offset;
            if (cell.x < 0 || cell.x >= views.GetLength(0) || cell.y < 0 || cell.y >= views.GetLength(1))
                continue;

            views[cell.x, cell.y].SetHighlight(isValid ? CellHighlightState.Valid : CellHighlightState.Invalid);
        }
        if(bag==ActiveTower.Backpack && isValid)ShowSynergyCells(data,origin,rotation,false);
    }

    public void ShowSynergyCells(ItemData data, Vector2Int origin, int rotation, bool clear=true)
    {
        if(clear)ClearPreview();
        if(cellViews==null||data.synergyRecipes==null||!ActiveTower.IsItemActive(data))return;
        foreach(var recipe in data.synergyRecipes){
            if(recipe==null||recipe.targetItem!=data)continue;
            foreach(var cell in ItemSynergyCalculator.Cells(data,recipe,origin,rotation)){
                if(cell.x<0||cell.y<0||cell.x>=cellViews.GetLength(0)||cell.y>=cellViews.GetLength(1))continue;
                var occupant=ActiveTower.Backpack.GetItemAt(cell);var image=cellViews[cell.x,cell.y].GetComponent<UnityEngine.UI.Image>();
                if(image!=null)image.color=occupant!=null&&ActiveTower.IsItemActive(occupant.Data)&&recipe.Accepts(occupant.Data)?new Color(.3f,.8f,.4f,.8f):new Color(.9f,.65f,.2f,.8f);
            }
        }
    }
    public void ShowAttachmentPreview(PlacedItem host,Backpack bag)
    {
        ClearPreview();var views=bag==ActiveTower.Storage?storageCellViews:cellViews;if(views==null)return;
        foreach(var cell in host.GetOccupiedCells())views[cell.x,cell.y].SetHighlight(CellHighlightState.Valid);
        if(evolutionStatus!=null)evolutionStatus.text="TAK · "+host.Data.Name+" üzerine bırak\nKüçük parça ikonunu sürükleyerek çıkarabilirsin.";
    }

    public void ConfigureItemVisual(RectTransform rect, ItemData data, int rotation, bool dragging)
    {
        Vector2Int size = ItemShapeUtility.GetBoundingSize(data.itemShape);
        rect.sizeDelta = new Vector2(size.x * cellSize, size.y * cellSize);
        rect.pivot = dragging
            ? new Vector2((data.centerOffset.x + 0.5f) / size.x, 1f - (data.centerOffset.y + 0.5f) / size.y)
            : new Vector2(0.5f, 0.5f);
        rect.localRotation = Quaternion.Euler(0f, 0f, -90f * ItemShapeUtility.NormalizeRotation(rotation));
    }

    public void ClearPreview()
    {
        if (storageCellViews != null)
            foreach (var view in storageCellViews) if (view != null) view.SetHighlight(CellHighlightState.None);
        if (cellViews == null)
            return;

        foreach (BackpackCellView view in cellViews)
            view.SetHighlight(CellHighlightState.None);
    }
}
