using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    [Header("UI Referansları")]
    public GameObject backpackPanel;
    public GameObject cellPrefab;        // Image + BackpackCellView içermeli
    public GameObject itemVisualPrefab;  // Image + CanvasGroup + BackpackDraggable içermeli
    public RectTransform gridContainer;

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
        backpackPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleClickOutside();
    }

    /// <summary>
    /// Hiçbir UI açık değilse yapacak bir şey yok. UI üzerine tıklandıysa dokunma
    /// (etkileşimi bozma). Bir kuleye tıklandıysa da dokunma - Tower.OnMouseDown
    /// zaten o kulenin UI'larını kendi açıyor/güncelliyor. Geriye kalan tek durum
    /// "boşluğa tıklandı" - o zaman backpack ve shop UI'larını birlikte kapat.
    /// </summary>
    private void HandleClickOutside()
    {
        if (ActiveTower == null)
            return;

        // Sadece backpack ve item dükkanı panellerinin içi paneli açık tutar (dükkandan
        // sürükleme bunu gerektiriyor). HUD, Start/Ayarlar gibi diğer UI'lar dahil
        // her yer "dışarı" sayılır.
        if (IsPointerOverPanel(backpackPanel) || IsPointerOverPanel(ShopUIManager.ItemShopUI))
            return;

        if (Camera.main != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.GetComponent<Tower>() != null)
                return;
        }

        CloseBackpack();

        if (ShopUIManager.ItemShopUI != null)
            ShopUIManager.ItemShopUI.SetActive(false);

        UİManager.activeTower = null;
        ActiveTower = null;
    }

    private bool IsPointerOverPanel(GameObject panel)
    {
        if (panel == null || !panel.activeInHierarchy)
            return false;

        Canvas canvas = panel.GetComponentInParent<Canvas>();
        Camera cam = (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;
        return RectTransformUtility.RectangleContainsScreenPoint(panel.GetComponent<RectTransform>(), Input.mousePosition, cam);
    }

    public void OpenBackpack(Tower tower)
    {
        if (ActiveTower == tower && backpackPanel.activeSelf)
            return;

        ActiveTower = tower;
        BuildGrid(tower.Backpack.Width, tower.Backpack.Height);
        RefreshItemVisuals();
        backpackPanel.SetActive(true);
    }

    public void CloseBackpack()
    {
        backpackPanel.SetActive(false);
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

        foreach (PlacedItem placed in ActiveTower.Backpack.Items)
            SpawnItemVisual(placed);
    }

    private void SpawnItemVisual(PlacedItem placed)
    {
        GameObject visualGO = Instantiate(itemVisualPrefab, gridContainer);
        RectTransform rt = visualGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);

        Vector2Int size = placed.BoundingSize;
        rt.sizeDelta = new Vector2(size.x * cellSize, size.y * cellSize);
        rt.anchoredPosition = new Vector2(placed.Origin.x * cellSize, -placed.Origin.y * cellSize);

        visualGO.GetComponent<Image>().sprite = placed.Data.itemIcon;

        BackpackDraggable draggable = visualGO.GetComponent<BackpackDraggable>();
        draggable.InitializeAsPlaced(placed);

        itemVisuals[placed] = visualGO;
    }

    /// <summary>Ekran koordinatını (mouse/dokunuş) grid hücre koordinatına çevirir. Grid dışındaysa null.</summary>
    public Vector2Int? ScreenPointToCell(Vector2 screenPoint, Camera cam)
    {
        if (ActiveTower == null)
            return null;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(gridContainer, screenPoint, cam, out Vector2 local))
            return null;

        int x = Mathf.FloorToInt(local.x / cellSize);
        int y = Mathf.FloorToInt(-local.y / cellSize);

        if (x < 0 || x >= ActiveTower.Backpack.Width || y < 0 || y >= ActiveTower.Backpack.Height)
            return null;

        return new Vector2Int(x, y);
    }

    /// <summary>Verilen item'ın origin'den başlayarak kaplayacağı hücreleri geçerliyse yeşil, değilse kırmızı boyar.</summary>
    public void ShowPlacementPreview(ItemData data, Vector2Int origin, bool isValid)
    {
        ClearPreview();

        if (cellViews == null)
            return;

        foreach (Vector2Int offset in data.itemShape)
        {
            Vector2Int cell = origin + offset;
            if (cell.x < 0 || cell.x >= ActiveTower.Backpack.Width || cell.y < 0 || cell.y >= ActiveTower.Backpack.Height)
                continue;

            cellViews[cell.x, cell.y].SetHighlight(isValid ? CellHighlightState.Valid : CellHighlightState.Invalid);
        }
    }

    public void ClearPreview()
    {
        if (cellViews == null)
            return;

        foreach (BackpackCellView view in cellViews)
            view.SetHighlight(CellHighlightState.None);
    }
}