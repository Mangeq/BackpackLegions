using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UİManager : MonoBehaviour
{
    //public Transform gridVisualsParent; // Arkadaki 25 siyah karenin klasörü
    //private UnityEngine.UI.Image[,] cellVisuals;

    //public Color defaultColor = Color.white;
    //public Color validColor = new Color(0, 1, 0, 0.4f);
    //public Color invalidColor = new Color(1, 0, 0, 0.4f);

    

    public static GameObject TowerShopUI;
    public static Text WaveText;
    public static Transform selectedTowerPlace;
    public static TowerSlot selectedTowerSlot;
    public static string selectedTowerType;
    public static Tower activeTower;
    [SerializeField] private GameObject towerShopPanel;
    [SerializeField] private BackpackUIManager backpackUI;
    [SerializeField] private ShopUIManager itemShopUI;
    [SerializeField] private Button TowerShopBuyBTN;
    [SerializeField] private Text UnitTowerGold;
    [SerializeField] private Text MegicTowerGold;
    [SerializeField] private Text ArcherTowerGold;
    private static Text GoldText;
    // Kule fiyatlarının tek kaynağı her prefab'ın kendi TowerData.cost'u.
    private int UnitTowerCost => UnitTowerPrefab.GetComponent<Tower>().data.cost;
    private int MegicTowerCost => MegicTowerPrefab.GetComponent<Tower>().data.cost;
    private int ArcherTowerCost => ArcherTowerPrefab.GetComponent<Tower>().data.cost;

    public GameObject UnitTowerPrefab;
    public GameObject MegicTowerPrefab;
    public GameObject ArcherTowerPrefab;
    public GameObject MortarTowerPrefab;

    [Header("Kule seçimi ve hazırlık göstergeleri")]
    public GameObject unitSelectionHighlight;
    public GameObject archerSelectionHighlight;
    public GameObject magicSelectionHighlight;
    public Text towerPurchaseFeedback;
    public Text preparationCountdownText;
    public RectTransform enemyEntryMarker;
    public RectTransform enemyEntryArrow;
    public Button startGameButton;
    [Header("Oyun hızı — sırasıyla 1×, 2×, 3×")]
    public UnityEngine.UI.Button[] speedButtons = new UnityEngine.UI.Button[3];
    public UnityEngine.UI.Image[] speedHighlights = new UnityEngine.UI.Image[3];
    [Header("Seçili okçu / büyücü hedeflemesi")]
    public GameObject targetingPanel;
    public UnityEngine.UI.Button[] targetTypeButtons = new UnityEngine.UI.Button[3];
    public UnityEngine.UI.Button[] targetOrderButtons = new UnityEngine.UI.Button[2];
    private GameManager gameManager;
    [Header("Birleşik kule paneli")]
    public UnityEngine.UI.Text towerPanelTitle;
    public UnityEngine.UI.Text towerPanelStats;
    public UnityEngine.UI.Text towerPanelGold;
    public Camera towerPreviewCamera;
    [Header("Kule XP ve element seçimi")]
    public UnityEngine.UI.Text towerExperienceText;
    public UnityEngine.UI.Text towerElementStatus;
    public UnityEngine.UI.Image towerExperienceFill;
    public UnityEngine.UI.Button[] elementButtons = new UnityEngine.UI.Button[4];

    public void SelectTowerElement(int element)
    {
        if (activeTower != null) activeTower.TrySelectElement((ElementType)element);
        RefreshTowerPanel();
    }

    public static string ElementLabel(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire: return "Ateş";
            case ElementType.Water: return "Su";
            case ElementType.Earth: return "Toprak";
            case ElementType.Air: return "Hava";
            default: return "Normal";
        }
    }
    [Header("Kule dönüşüm onayı")]
    public GameObject transitionPanel;
    public UnityEngine.UI.Text transitionTitle, transitionDetails;
    public UnityEngine.UI.Button transitionConfirm, openElementChangeButton, openTypeChangeButton;
    public UnityEngine.UI.Button[] transitionElements = new UnityEngine.UI.Button[4];
    public UnityEngine.UI.Button[] transitionTypes = new UnityEngine.UI.Button[3];
    private Tower transitionOwner;
    private bool changingTowerType;
    private ElementType transitionElement;
    private GameObject transitionPrefab;
    private string transitionFailure;
    private static int panelOpenedFrame = -1, panelDismissedFrame = -1;
    public static void NotifyPanelOpened() => panelOpenedFrame = Time.frameCount;

    public bool CloseTopPanel()
    {
        if (transitionPanel != null && transitionPanel.activeInHierarchy)
        { BackpackDraggable.CancelActiveDrag(); CloseTransition(); return true; }
        var bags = BackpackUIManager.Instance;
        if (bags != null && bags.combinedPanel != null && bags.combinedPanel.activeInHierarchy)
        { CloseTowerPanel(); return true; }
        if (TowerShopUI != null && TowerShopUI.activeInHierarchy)
        { ClosePurchasePanel(); return true; }
        return false;
    }

    private static bool ContainsPoint(GameObject panel, Vector2 point)
    {
        if (panel == null || !panel.activeInHierarchy) return false;
        var rect = panel.transform as RectTransform;
        var canvas = panel.GetComponentInParent<Canvas>();
        return rect != null && RectTransformUtility.RectangleContainsScreenPoint(rect, point,
            canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera);
    }

    public bool HandleOutsideClick(Vector2 point)
    {
        if (SettingsPanelUI.BlocksWorldInput || SkillTreePanelUI.BlocksWorldInput || SceneLoadingUI.IsLoading) return true;
        if (gameManager == null || gameManager.CurrentState == GameManager.GameState.Victory
            || gameManager.CurrentState == GameManager.GameState.Defeat) return false;
        if (gameManager.IsPaused)
        {
            if (ContainsPoint(gameManager.pausePanel, point)) return false;
            gameManager.ContinueGame(); return true;
        }
        if (transitionPanel != null && transitionPanel.activeInHierarchy)
        {
            var card = transitionPanel.transform.Find("Card");
            if (card != null && ContainsPoint(card.gameObject, point)) return false;
        }
        else if (BackpackUIManager.Instance != null && ContainsPoint(BackpackUIManager.Instance.combinedPanel, point)) return false;
        else if (ContainsPoint(TowerShopUI, point)) return false;
        return CloseTopPanel();
    }

    public static bool ConsumeWorldPanelClick()
    {
        if (panelDismissedFrame == Time.frameCount) return true;
        var ui = FindObjectOfType<UİManager>();
        if (ui == null || !ui.HandleOutsideClick(Input.mousePosition)) return false;
        panelDismissedFrame = Time.frameCount;
        return true;
    }

    private void LateUpdate()
    {
        // Açan tıklama aynı karede paneli kapatmasın; sürükleme bırakması satışla çakışmasın.
        if (Input.GetMouseButtonDown(0) && panelOpenedFrame != Time.frameCount
            && panelDismissedFrame != Time.frameCount && !BackpackDraggable.IsDragging
            && HandleOutsideClick(Input.mousePosition)) panelDismissedFrame = Time.frameCount;
    }

    public void OpenElementChange()
    {
        if(activeTower==null)return;
        BackpackDraggable.CancelActiveDrag();
        if(activeTower.OpenElementShop())
        {
            CloseTransition();
            if(itemShopUI!=null)itemShopUI.UpdateShopUI();
            NotifyPanelOpened();
        }
    }
    public void OpenTowerTypeChange() => OpenTransition(true);
    private void OpenTransition(bool typeChange)
    {
        if (!GameManager.CanUpgradeTowers || activeTower == null || transitionPanel == null) return;
        if (!typeChange && activeTower.SelectedElement == ElementType.Normal) return;
        BackpackDraggable.CancelActiveDrag();
        changingTowerType = typeChange;
        transitionFailure = null;
        transitionOwner = activeTower;
        transitionElement = ElementType.Normal;
        transitionPrefab = null;
        GameAudio.SetPanel(transitionPanel, true);
        transitionPanel.transform.SetAsLastSibling();
        NotifyPanelOpened();
        RefreshTransition();
    }
    public void CloseTransition()
    {
        if (transitionPanel != null) GameAudio.SetPanel(transitionPanel, false);
        transitionOwner = null;
        transitionPrefab = null;
        transitionElement = ElementType.Normal;
    }
    public void ChooseTransitionElement(int element)
    {
        if (transitionOwner == null || changingTowerType) return;
        transitionElement = (ElementType)element;
        transitionFailure = null;
        RefreshTransition();
    }
    public void ChooseTransitionType(int type)
    {
        if (transitionOwner == null || !changingTowerType) return;
        transitionFailure = null;
        transitionPrefab = type == (int)TowerType.MagicTower ? MegicTowerPrefab
            : type == (int)TowerType.ArrowTower ? ArcherTowerPrefab
            : type == (int)TowerType.UnitTower ? UnitTowerPrefab
            : type == (int)TowerType.CannonTower ? MortarTowerPrefab : null;
        RefreshTransition();
    }
    private static string TowerTypeLabel(TowerType type) => type == TowerType.MagicTower ? "Büyü Kulesi"
        : type == TowerType.ArrowTower ? "Okçu Kulesi" : type == TowerType.CannonTower ? "Havan Kulesi" : "Birlik Kulesi";

    private void RefreshTransition()
    {
        if (openElementChangeButton != null) openElementChangeButton.interactable = GameManager.CanUpgradeTowers
            && activeTower != null && activeTower.Level>=5 && !activeTower.IsElementShop;
        if (openTypeChangeButton != null)
        {
            openTypeChangeButton.interactable = GameManager.CanUpgradeTowers && activeTower != null && !BackpackDraggable.IsDragging;
            var label = openTypeChangeButton.GetComponentInChildren<UnityEngine.UI.Text>();
            if (label != null) label.text = "Kule tipi değiştir";
        }
        if (transitionPanel == null || !transitionPanel.activeSelf) return;
        if (transitionOwner == null || transitionOwner != activeTower || !GameManager.CanUpgradeTowers
            || BackpackUIManager.Instance == null || !BackpackUIManager.Instance.combinedPanel.activeInHierarchy)
        { CloseTransition(); return; }
        transitionTitle.text = changingTowerType ? "Kule tipi değiştir" : "Element değiştir";
        foreach (var button in transitionElements) if (button != null) button.gameObject.SetActive(!changingTowerType);
        foreach (var button in transitionTypes) if (button != null) button.gameObject.SetActive(changingTowerType);
        for (int i = 0; i < transitionElements.Length; i++) if (transitionElements[i] != null)
            transitionElements[i].interactable = (ElementType)(i + 1) != transitionOwner.SelectedElement;
        string error;
        if (changingTowerType)
        {
            error = transitionPrefab == null ? "Yeni kule türünü seç." : transitionOwner.GetTypeChangeError(transitionPrefab);
            error = error ?? transitionFailure;
            var target = transitionPrefab != null ? transitionPrefab.GetComponent<Tower>() : null;
            transitionDetails.text = (target != null ? "Yeni kule: " + TowerTypeLabel(target.data.type)
                + "\nYeni fiyat: " + target.data.cost + " altın · Eski kule iadesi: " + transitionOwner.TypeChangeRefund
                + "\nDönüşüm ücreti: " + transitionOwner.GetTypeChangeCost(transitionPrefab) + " altın"
                + "\nÇantadaki " + transitionOwner.TypeChangeSaleCount + " eşya SATILACAK: +" + transitionOwner.TypeChangeSaleValue + " altın"
                + "\nİşlem sonrası altın: " + (EconomyManager.Gold + transitionOwner.TypeChangeSaleValue - transitionOwner.GetTypeChangeCost(transitionPrefab))
                : "Okçu, büyü, birlik veya havan kulesine geçebilirsin.")
                + "\n\nSeviye 1 · XP 0 · Normal element\nEski ana eşya yeni başlangıç eşyasıyla değiştirilir."
                + "\nDepo ve teslim bekleyen eşyalar korunur.\nMağaza yenilenir; dondurma kaldırılır."
                + (transitionOwner is UnitTower ? "\nMevcut askerler kaldırılır." : "")
                + "\n\n" + (error ?? "Bu değişikliği onaylıyor musun?");
        }
        else
        {
            error = transitionElement == ElementType.Normal ? "Yeni elementi seç." : transitionOwner.GetElementChangeError(transitionElement);
            error = error ?? transitionFailure;
            transitionDetails.text = "Yeni element: " + (transitionElement == ElementType.Normal ? "—" : ElementLabel(transitionElement))
                + "\nÜcret: " + transitionOwner.ElementChangeCost + " altın"
                + "\n\nSeviye ve XP korunur; ana silah dönüşür."
                + "\nNormal eşyalar çalışmaya devam eder.\nEski elemente özel eşyalar kalır fakat bonus vermez."
                + "\nMağaza hemen yenilenir; dondurma kaldırılır."
                + "\nDepodaki eşyalar hiçbir bonus vermez."
                + "\n\n" + (error ?? "Bu değişikliği onaylıyor musun?");
        }
        transitionConfirm.interactable = error == null;
    }

    public void ConfirmTransition()
    {
        if (transitionPanel == null || !transitionPanel.activeInHierarchy || transitionOwner == null
            || activeTower != transitionOwner || !GameManager.CanUpgradeTowers) return;
        var owner = transitionOwner;
        if (changingTowerType)
        {
            if (transitionPrefab == null || !owner.TryChangeTowerType(transitionPrefab, out Tower replacement))
            { transitionFailure = owner.TransitionError ?? "Yeni kule türünü seç."; RefreshTransition(); return; }
            CloseTransition();
            activeTower = replacement;
            BackpackUIManager.Instance.OpenBackpack(replacement);
            FindObjectOfType<ShopUIManager>().UpdateShopUI();
        }
        else
        {
            if (!owner.TryChangeElement(transitionElement)) { transitionFailure = owner.TransitionError; RefreshTransition(); return; }
            CloseTransition();
        }
        RefreshTowerPanel();
    }

    [Header("Kule satın alma kartı")]
    public GameObject purchaseDetails;
    public Text purchaseTitle;
    public Text purchaseDescription;
    public Text purchaseStats;
    public Text purchaseStatValues;
    public Text purchasePrice;
    public Image purchasePortrait;
    public Sprite magicPurchasePortrait, archerPurchasePortrait, unitPurchasePortrait;
    public Sprite mortarPurchasePortrait;
    public GameObject cannonSelectionHighlight;
    private WaveManager waveManager;
    private Camera worldCamera;
    private Vector3 entryDirection = Vector3.forward;

    internal GameObject Heart1;
    internal GameObject Heart2;
    internal GameObject Heart3;

    // Start is called before the first frame update
    void Awake()
    {
        TowerShopUI = towerShopPanel != null ? towerShopPanel : GameObject.Find("TowerShopUI");
        if (TowerShopBuyBTN == null) TowerShopBuyBTN = GameObject.Find("TowerShopBuyBTN").GetComponent<Button>();
        if (UnitTowerGold == null) UnitTowerGold = GameObject.Find("UnitTowerGold").GetComponent<Text>();
        if (MegicTowerGold == null) MegicTowerGold = GameObject.Find("MegicTowerGold").GetComponent<Text>();
        if (ArcherTowerGold == null) ArcherTowerGold = GameObject.Find("ArcherTowerGold").GetComponent<Text>();
        GoldText = GameObject.Find("GoldText").GetComponent<Text>();
        Heart1 = GameObject.Find("HealBar/Heart1");
        Heart2 = GameObject.Find("HealBar/Heart2");
        Heart3 = GameObject.Find("HealBar/Heart3");
        WaveText= GameObject.Find("WaveBar/WaveText").GetComponent<Text>();
        gameManager = FindObjectOfType<GameManager>();
        waveManager = FindObjectOfType<WaveManager>();
        worldCamera = Camera.main;
        selectedTowerType = null;
        selectedTowerSlot = null;
        selectedTowerPlace = null;
        // The saved UI starts closed, so panel-owned controllers may not have
        // received Awake yet. Initialize them from this always-active controller.
        if(itemShopUI!=null)itemShopUI.InitializeForScene();
        if(backpackUI!=null)backpackUI.InitializeForScene();

        //cellVisuals = new UnityEngine.UI.Image[5, 5]; // Çanta 5x5
        //if (gridVisualsParent != null)
        //{
        //    int childIndex = 0;
        //    for (int y = 0; y < 5; y++)
        //    {
        //        for (int x = 0; x < 5; x++)
        //        {
        //            if (childIndex < gridVisualsParent.childCount)
        //            {
        //                cellVisuals[x, y] = gridVisualsParent.GetChild(childIndex).GetComponent<UnityEngine.UI.Image>();
        //            }
        //            childIndex++;
        //        }
        //    }
        //}
    }
    void Start()
    {
        TowerPurchaseSkin.Install(this);
        TowerShopBuyBTN.enabled = false;
        UnitTowerGold.text = UnitTowerCost.ToString();
        MegicTowerGold.text = MegicTowerCost.ToString();
        ArcherTowerGold.text = ArcherTowerCost.ToString();
        UpdateGoldText();
        GameAudio.SetPanel(TowerShopUI, false);
        CacheEntryDirection();
        RefreshPurchaseFeedback();
    }

    private void Update()
    {
        // Panel başka bir yoldan gizlense de geçici slot kilidi kalmasın.
        if (selectedTowerSlot != null && TowerShopUI != null && !TowerShopUI.activeInHierarchy)
            CancelTowerPurchase();
        RefreshPurchaseFeedback();
        if(TowerShopUI!=null && TowerShopUI.activeSelf)
        {
            var parent=TowerShopUI.transform.parent as RectTransform;
            if(parent!=null&&GetComponent<TowerPurchaseSkin>()==null)TowerShopUI.transform.localScale=Vector3.one*Mathf.Min(1f,Mathf.Min(parent.rect.width/700f,parent.rect.height/760f));
            if(gameManager!=null && gameManager.CurrentState!=GameManager.GameState.Preparation)ClosePurchasePanel();
        }
        if (gameManager == null) return;
        RefreshTowerPanel();
        RefreshTransition();
        RefreshSpeedControls();
        RefreshTargetingControls();
        bool preparing = gameManager.CurrentState == GameManager.GameState.Preparation;
        if (preparationCountdownText != null)
        {
            preparationCountdownText.transform.parent.gameObject.SetActive(preparing);
            preparationCountdownText.text = gameManager.IsPaused
                ? "Hazırlık duraklatıldı · " + Mathf.CeilToInt(gameManager.PreparationSecondsRemaining) + " sn"
                : "Savaş " + Mathf.CeilToInt(gameManager.PreparationSecondsRemaining) + " sn sonra başlıyor";
        }
        if (startGameButton != null) startGameButton.interactable = preparing && !gameManager.IsPaused;
        UpdateEntryMarker();
    }

    public void SelectGameSpeed(int speed)
    {
        if (gameManager == null) return;
        gameManager.SetGameSpeed(speed);
        RefreshSpeedControls();
    }

    private void RefreshTowerPanel()
    {
        var bags = BackpackUIManager.Instance;
        if (bags != null && bags.combinedPanel != null && bags.GetComponent<TowerPanelLayout>() == null)
        {
            var parent = bags.combinedPanel.transform.parent as RectTransform;
            if (parent != null)
                bags.combinedPanel.transform.localScale = Vector3.one * Mathf.Min(parent.rect.width / 1660f, parent.rect.height / 970f);
        }
        bool visible = bags != null && bags.combinedPanel != null && bags.combinedPanel.activeInHierarchy && activeTower != null;
        if (towerPreviewCamera != null) towerPreviewCamera.enabled = visible && bags.GetComponent<TowerPanelLayout>() == null;
        if (!visible) return;
        var t = activeTower;
        string name = TowerTypeLabel(t.data.type);
        if (t.SelectedElement != ElementType.Normal)
            name = ElementLabel(t.SelectedElement) + (t is UnitTower ? " Birlik Kulesi" : t is ArcherTower ? " Okçusu" : t is MortarTower ? " Havanı" : " Büyücüsü");
        if (towerPanelTitle != null) towerPanelTitle.text = name + "  ·  Seviye " + t.towerLevel;
        if (towerPanelGold != null) towerPanelGold.text = EconomyManager.Gold + " altın";
        if (towerExperienceText != null)
            towerExperienceText.text = "XP  " + t.Experience.ToString("0.#") + " / " + t.ExperienceRequired.ToString("0.#")
                + "    ·    " + ElementLabel(t.SelectedElement);
        if (towerExperienceFill != null)
            towerExperienceFill.rectTransform.anchorMax = new Vector2(Mathf.Clamp01(t.Experience / t.ExperienceRequired), 1f);
        if (towerElementStatus != null)
            towerElementStatus.text = !string.IsNullOrEmpty(t.ElementSelectionError) ? t.ElementSelectionError
                : t.Level<5 ? "5. seviyede element eşyaları mağazaya gelir" : "Element eşyasını ana silahın yanına koy";
        for (int i = 0; i < elementButtons.Length; i++)
            if (elementButtons[i] != null)
            {
                elementButtons[i].gameObject.SetActive(t.AwaitingElement);
                elementButtons[i].interactable = t.AwaitingElement && GameManager.CanUpgradeTowers;
            }
        if (towerPanelStats != null)
        {
            towerPanelStats.text = "HASAR  " + t.currentDamage.ToString("0.##") + "     (Temel " + t.baseDamage.ToString("0.##") + " · Eşya " + (t.currentDamage-t.baseDamage).ToString("+0.##;-0.##;0") + ")\n"
                + "ATIŞ ARALIĞI  " + t.currentAttackSpeed.ToString("0.##") + " sn     (Temel " + t.baseAttackSpeed.ToString("0.##") + " · Eşya " + (t.currentAttackSpeed-t.baseAttackSpeed).ToString("+0.##;-0.##;0") + " sn)\n"
                + "KRİTİK  %" + (100*t.currentCriticalChance).ToString("0.#") + "     (Temel %" + (100*t.baseCriticalChance).ToString("0.#") + " · Eşya " + (100*(t.currentCriticalChance-t.baseCriticalChance)).ToString("+0.#;-0.#;0") + " puan)\n"
                + (t is UnitTower ? "ASKER CAN BONUSU  %" + (100*t.currentHealthBonus).ToString("0.#") + " · Değerler asker başına" : "MENZİL  " + t.CurrentRange.ToString("0.#") + " birim");
            if (t is MortarTower) towerPanelStats.text += " · PATLAMA  " + t.data.explosionRadius.ToString("0.#") + " · Yalnız kara";
        }
        if (towerPreviewCamera != null)
        {
            towerPreviewCamera.transform.position = t.transform.position + new Vector3(0, 7, -6);
            towerPreviewCamera.transform.LookAt(t.transform.position + Vector3.up * 0.4f);
        }
    }

    public void CloseTowerPanel()
    {
        CloseTransition();
        if (BackpackUIManager.Instance != null) BackpackUIManager.Instance.CloseBackpack();
        activeTower = null;
    }

    private bool CanEditTargeting => gameManager != null && !gameManager.IsPaused
        && (gameManager.CurrentState == GameManager.GameState.Preparation || gameManager.CurrentState == GameManager.GameState.Combat)
        && activeTower != null && activeTower.SupportsManualTargeting;

    public void SelectTargetType(int value)
    {
        if (!CanEditTargeting) return;
        activeTower.SetTargetType(value);
        RefreshTargetingControls();
    }

    public void SelectTargetOrder(int value)
    {
        if (!CanEditTargeting) return;
        activeTower.SetTargetOrder(value);
        RefreshTargetingControls();
    }

    private void RefreshTargetingControls()
    {
        if (targetingPanel == null) return;
        bool visible = activeTower != null && activeTower.SupportsManualTargeting
            && gameManager.CurrentState != GameManager.GameState.Victory && gameManager.CurrentState != GameManager.GameState.Defeat;
        targetingPanel.SetActive(visible);
        if (!visible) return;
        UpdateTargetButtons(targetTypeButtons, (int)activeTower.SelectedTargetType);
        if (!activeTower.SupportsAirTargets)
            foreach (var button in targetTypeButtons) if (button != null) button.interactable = false;
        UpdateTargetButtons(targetOrderButtons, (int)activeTower.SelectedTargetOrder);
    }

    private void UpdateTargetButtons(UnityEngine.UI.Button[] buttons, int selected)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;
            buttons[i].interactable = CanEditTargeting;
            bool referenceSkin=BackpackUIManager.Instance!=null&&BackpackUIManager.Instance.GetComponent<TowerPanelLayout>()!=null;
            buttons[i].targetGraphic.color = referenceSkin ? Color.white : i == selected ? new Color(0.55f, 0.35f, 0.08f) : new Color(0.10f, 0.13f, 0.18f);
        }
    }

    private void RefreshSpeedControls()
    {
        for (int i = 0; i < speedButtons.Length; i++)
        {
            if (speedButtons[i] != null) speedButtons[i].interactable = gameManager.CanChangeSpeed;
            if (i < speedHighlights.Length && speedHighlights[i] != null)
                speedHighlights[i].enabled = gameManager.SelectedSpeed == i + 1;
        }
    }

    public void RefreshPurchaseFeedback()
    {
        bool canBuy = GameManager.CanBuyTowers;
        var prefab=selectedTowerType=="UnitTower"?UnitTowerPrefab:selectedTowerType=="ArcherTower"?ArcherTowerPrefab:selectedTowerType=="MegicTower"?MegicTowerPrefab:selectedTowerType=="CannonTower"?MortarTowerPrefab:null;
        var selected=prefab!=null?prefab.GetComponent<Tower>():null;
        if (TowerShopBuyBTN != null) TowerShopBuyBTN.interactable = canBuy && selected != null && EconomyManager.Gold>=selected.data.cost;
        RefreshPurchaseCard(selected);
        if (unitSelectionHighlight != null) unitSelectionHighlight.SetActive(selectedTowerType == "UnitTower");
        if (archerSelectionHighlight != null) archerSelectionHighlight.SetActive(selectedTowerType == "ArcherTower");
        if (magicSelectionHighlight != null) magicSelectionHighlight.SetActive(selectedTowerType == "MegicTower");
        if (cannonSelectionHighlight != null) cannonSelectionHighlight.SetActive(selectedTowerType == "CannonTower");
        if (towerPurchaseFeedback == null) return;
        if (!canBuy)
        {
            towerPurchaseFeedback.text = "Kule alımı kilitli · Hazırlığı bekle";
            towerPurchaseFeedback.color = new Color(1f, 0.35f, 0.3f);
            return;
        }
        int cost = selected != null ? selected.data.cost : 0;
        bool insufficient = cost > EconomyManager.Gold;
        towerPurchaseFeedback.text = selectedTowerType == null ? "Bir kule seç" : insufficient
            ? "Yetersiz para! " + (cost - EconomyManager.Gold) + " altın eksik."
            : "Seçildi · " + cost + " altın";
        towerPurchaseFeedback.color = insufficient ? new Color(1f, 0.35f, 0.3f) : new Color(1f, 0.85f, 0.4f);
    }

    private void RefreshPurchaseCard(Tower tower)
    {
        if(purchaseDetails==null)return;
        purchaseDetails.SetActive(selectedTowerType!=null);
        if(selectedTowerType==null)return;
        if(tower==null)
        {
            purchaseTitle.text="TOPÇU KULESİ";purchaseDescription.text="Yakında\nTopçu kulesi geliştirme aşamasında.";
            purchaseStats.text="Bu kule henüz satın alınamaz.";if(purchaseStatValues!=null)purchaseStatValues.text="";purchasePrice.text="YAKINDA";purchasePortrait.enabled=false;return;
        }
        var data=tower.data;bool unit=tower is UnitTower;bool magic=tower is MegicTower;
        purchaseTitle.text=tower is MortarTower?"HAVAN KULESİ":unit?"BİRLİK KULESİ":magic?"BÜYÜ KULESİ":"OKÇU KULESİ";
        purchaseDescription.text=unit?"Birlik üretir. Düşmanları tutarak ilerlemelerini yavaşlatır ve hasar verir.":magic?"Menzilindeki düşmanlara büyü saldırıları yapar. Kara ve hava hedefleri seçilebilir.":"Menzilindeki düşmanlara ok atar. Kara ve hava hedefleri seçilebilir.";
        purchasePortrait.sprite=unit?unitPurchasePortrait:magic?magicPurchasePortrait:archerPurchasePortrait;purchasePortrait.enabled=purchasePortrait.sprite!=null;
        if (tower is MortarTower)
        {
            purchaseDescription.text = "Kara düşmanlarının bulunduğu noktayı bombalar. Alan hasarı verir; hareketli hedefler kaçabilir.";
            purchasePortrait.sprite = mortarPurchasePortrait;
            purchasePortrait.enabled = mortarPurchasePortrait != null;
        }
        // Satın alınınca verilen ana itemin katkısı da fiyat kartındaki başlangıç değerlerine dahildir.
        var item=data.defaultWeapon;float damage=data.damage*(1+(item!=null?item.damageBonus:0));float interval=Mathf.Max(.1f,data.attackSpeed/(1+(item!=null?item.attackSpeedBonus:0)));float crit=Mathf.Clamp01(data.criticalChance+(item!=null?item.criticalChanceBonus:0));
        purchaseStats.text=(unit?"ASKER BAŞINA HASAR":"HASAR")+"\nATIŞ ARALIĞI\nKRİTİK ŞANSI\n";
        string values=damage.ToString("0.##")+"\n"+interval.ToString("0.##")+" sn\n%"+(crit*100).ToString("0.#")+"\n";
        if(unit){var barracks=(UnitTower)tower;purchaseStats.text+="BİRLİK SAYISI\nYENİDEN ÜRETİM\n";values+=barracks.maxUnitCount+"\n"+barracks.spawnCooldown.ToString("0.#")+" sn\n";}
        else {purchaseStats.text+="MENZİL\n";values+=data.range.ToString("0.#")+"\n";}
        if (tower is MortarTower) { purchaseStats.text += "PATLAMA YARIÇAPI\n"; values += data.explosionRadius.ToString("0.#") + "\n"; }
        purchaseStats.text+="ÇANTA";values+=data.gridWidth+" × "+data.gridHeight;
        if(purchaseStatValues!=null)purchaseStatValues.text=values;
        purchasePrice.text="SATIN AL  ·  "+data.cost+" altın";
    }

    public void PreparePurchasePanel()
    {
        NotifyPanelOpened();
        CloseTowerPanel();
        RefreshPurchaseFeedback();
    }
    public void ClosePurchasePanel()
    {
        GameAudio.SetPanel(TowerShopUI, false);CancelTowerPurchase();
    }
    public void CannonTowerBTN()
    {
        selectedTowerType="CannonTower";TowerShopBuyBTN.enabled=true;RefreshPurchaseFeedback();
    }

    private void CacheEntryDirection()
    {
        if (waveManager == null || waveManager.spawnPoint == null) return;
        var destination = GameObject.FindGameObjectWithTag("Base");
        if (destination == null) return;
        var path = new UnityEngine.AI.NavMeshPath();
        if (UnityEngine.AI.NavMesh.CalculatePath(waveManager.spawnPoint.position, destination.transform.position, UnityEngine.AI.NavMesh.AllAreas, path))
            foreach (var corner in path.corners)
                if ((corner - waveManager.spawnPoint.position).sqrMagnitude > 0.1f)
                { entryDirection = (corner - waveManager.spawnPoint.position).normalized; return; }
        entryDirection = (destination.transform.position - waveManager.spawnPoint.position).normalized;
    }

    private void UpdateEntryMarker()
    {
        if (enemyEntryMarker == null || waveManager == null || waveManager.spawnPoint == null || worldCamera == null) return;
        bool ended = gameManager.CurrentState == GameManager.GameState.Victory || gameManager.CurrentState == GameManager.GameState.Defeat;
        enemyEntryMarker.gameObject.SetActive(!ended);
        if (ended) return;
        Vector3 screen = worldCamera.WorldToScreenPoint(waveManager.spawnPoint.position);
        Vector3 next = worldCamera.WorldToScreenPoint(waveManager.spawnPoint.position + entryDirection);
        var root = enemyEntryMarker.parent as RectTransform;
        var canvas = root.GetComponentInParent<Canvas>();
        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(root, screen, uiCamera, out Vector2 point);
        float halfWidth = enemyEntryMarker.rect.width * 0.5f, halfHeight = enemyEntryMarker.rect.height * 0.5f;
        point.x = Mathf.Clamp(point.x, root.rect.xMin + halfWidth + 12f, root.rect.xMax - halfWidth - 12f);
        point.y = Mathf.Clamp(point.y, root.rect.yMin + halfHeight + 12f, root.rect.yMax - halfHeight - 12f);
        enemyEntryMarker.localPosition = new Vector3(point.x, point.y, 0f);
        if (enemyEntryArrow != null)
        {
            Vector2 direction = next - screen;
            enemyEntryArrow.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
    }
    public void TogglePanel(GameObject panel)
    {
        bool willBeActive = !panel.activeSelf;
        GameAudio.SetPanel(panel, willBeActive);

        if (panel == TowerShopUI && !willBeActive)
            CancelTowerPurchase();
    }

    private void CancelTowerPurchase()
    {
        // Satın alınmamış slotu yeniden tıklanabilir yap; seçimi temizle.
        if (selectedTowerSlot != null)
        {
            selectedTowerSlot.ReleaseSelection();
            selectedTowerSlot = null;
        }
        selectedTowerPlace = null;
        selectedTowerType = null;
        TowerShopBuyBTN.enabled = false;
        RefreshPurchaseFeedback();
    }
   


    public static void UpdateWaveText(int currentWave, int totalWaves)
    {
        WaveText.text = "WAVE: " + currentWave + "/" + totalWaves;
    }

    public void UnitTowerBTN()
    {
        selectedTowerType = "UnitTower";
        TowerShopBuyBTN.enabled = true;
        RefreshPurchaseFeedback();
    }
    public void MegicTowerBTN()
    {
        selectedTowerType = "MegicTower";
        TowerShopBuyBTN.enabled = true;
        RefreshPurchaseFeedback();
    }
    public void ArcherTowerBTN()
    {
        selectedTowerType = "ArcherTower";
        TowerShopBuyBTN.enabled = true;
        RefreshPurchaseFeedback();
    }
    public static void UpdateGoldText()
    {
        GoldText.text = EconomyManager.Gold.ToString();
    }
    public void BuyTower() //Bu fonksiyonun içindeki ifler kalkacak towermanager adında bir sınıf oluşturulacak ve o sınıfta kule satın alma işlemi yapılacak. Bu sınıfın içinde kule türüne göre fiyat kontrolü yapılacak ve kule satın alınacak.
    {
        if (!GameManager.CanBuyTowers) { RefreshPurchaseFeedback(); return; }
        if(selectedTowerType == null || selectedTowerPlace == null || selectedTowerSlot == null
            || selectedTowerSlot.isOccupied || !TowerShopUI.activeInHierarchy)
        {
            Debug.Log("Lütfen bir kule türü ve yer seçin!");
            return;
        }
        if(selectedTowerType == "UnitTower" && EconomyManager.Gold >= UnitTowerCost)
        {
            EconomyManager.Gold -= UnitTowerCost;
            GameObject tower = Instantiate(UnitTowerPrefab, selectedTowerPlace.position, Quaternion.identity);
            tower.GetComponent<UnitTower>().directionUp = selectedTowerPlace.GetComponent<TowerSlot>().directionUp;
            UpdateGoldText();
        }
        else if(selectedTowerType == "MegicTower" && EconomyManager.Gold >= MegicTowerCost)
        {
            EconomyManager.Gold -= MegicTowerCost;
            Instantiate(MegicTowerPrefab, selectedTowerPlace.position, Quaternion.identity);
            UpdateGoldText();
        }
        else if(selectedTowerType == "ArcherTower" && EconomyManager.Gold >= ArcherTowerCost)
        {
            EconomyManager.Gold -= ArcherTowerCost;
            Instantiate(ArcherTowerPrefab, selectedTowerPlace.position, Quaternion.identity);
            UpdateGoldText();
        }
        else if(selectedTowerType == "CannonTower" && MortarTowerPrefab != null
            && EconomyManager.TrySpendGold(MortarTowerPrefab.GetComponent<Tower>().data.cost))
        {
            Instantiate(MortarTowerPrefab, selectedTowerPlace.position, Quaternion.identity);
            UpdateGoldText();
        }
        else
        {
            Debug.Log("Yeterli altın yok!");
            RefreshPurchaseFeedback();
            return;
        }
       
        Debug.Log("Kule satın alındı!");
        selectedTowerSlot.MarkOccupied();
        selectedTowerSlot = null;
        selectedTowerPlace = null;
        selectedTowerType = null;
        TowerShopBuyBTN.enabled = false;
        RefreshPurchaseFeedback();
        GameAudio.SetPanel(TowerShopUI, false);
    }

    //public void ResetAllHighlights()
    //{
    //    if (cellVisuals == null) return;
    //    for (int x = 0; x < 5; x++)
    //    {
    //        for (int y = 0; y < 5; y++)
    //        {
    //            if (cellVisuals[x, y] != null)
    //                cellVisuals[x, y].color = defaultColor;
    //        }
    //    }
    //}

    //public void HighlightCells(ItemData item, int startX, int startY)
    //{
    //    ResetAllHighlights();
    //    if (activeBackpack == null || cellVisuals == null) return;

    //    // Eşya sığıyor mu diye kuledeki çantaya (backend) soruyoruz
    //    bool canPlace = activeBackpack.CanPlaceItem(item, startX, startY);

    //    for (int x = 0; x < item.width; x++)
    //    {
    //        for (int y = 0; y < item.height; y++)
    //        {
    //            int targetX = startX + x;
    //            int targetY = startY + y;

    //            if (targetX >= 0 && targetX < 5 && targetY >= 0 && targetY < 5)
    //            {
    //                if (cellVisuals[targetX, targetY] != null)
    //                {
    //                    cellVisuals[targetX, targetY].color = canPlace ? validColor : invalidColor;
    //                }
    //            }
    //        }
    //    }
    //}
}
