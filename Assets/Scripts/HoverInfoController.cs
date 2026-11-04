using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverInfoController : MonoBehaviour
{
    public static HoverInfoController Instance { get; private set; }
    public InfoPanelUI infoPanel;
    public RectTransform healthBarLayer;
    [Min(0f)] public float hoverDelay = 0.15f;

    private readonly List<RaycastResult> uiHits = new List<RaycastResult>();
    private PointerEventData pointer;
    private EventSystem pointerEventSystem;
    private Camera worldCamera;
    private UnityEngine.Object hovered;
    private float showAt, refreshAt;
    private GameManager gameManager;
    private Tower rangeTower;
    private LineRenderer rangeLine;
    private Material rangeMaterial;

    private void Awake()
    {
        Instance = this;
        worldCamera = Camera.main;
        gameManager = FindObjectOfType<GameManager>();
        infoPanel.Hide();
    }

    private void Update()
    {
        bool rightClick = Input.GetMouseButtonDown(1);
        if (rightClick || Input.GetMouseButtonDown(0))
            HandleRangeClick(Input.mousePosition, rightClick);
        RefreshRange();
        UpdateHover(Input.mousePosition, Input.GetMouseButton(0));
    }

    private void HandleRangeClick(Vector2 position, bool rightClick)
    {
        var target = rightClick ? FindTarget(position) as Tower : null;
        rangeTower = target != rangeTower ? target : null;
        if (rangeLine != null && rangeTower == null) rangeLine.enabled = false;
        ClearHover();
    }

    private void RefreshRange()
    {
        if (rangeLine == null && rangeTower != null)
        {
            var obj = new GameObject("TowerRange");
            obj.transform.SetParent(transform, false);
            rangeLine = obj.AddComponent<LineRenderer>();
            rangeMaterial = new Material(Shader.Find("Sprites/Default"));
            rangeLine.sharedMaterial = rangeMaterial;
            rangeLine.useWorldSpace = true;
            rangeLine.loop = true;
            rangeLine.positionCount = 128;
            rangeLine.widthMultiplier = .085f;
            rangeLine.startColor = rangeLine.endColor = new Color(.35f, 1f, .65f, .85f);
            rangeLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        if (rangeLine == null) return;
        rangeLine.enabled = rangeTower != null && rangeTower.isActiveAndEnabled && !gameManager.HasEnded;
        if (!rangeLine.enabled) return;
        var center = rangeTower.transform.position + Vector3.up * .12f;
        for (int i = 0; i < 128; i++)
        {
            float angle = i * Mathf.PI * 2f / 128;
            rangeLine.SetPosition(i, center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * rangeTower.CurrentRange);
        }
    }

    // Girdi okuma ile hedef/gösterim akışını ayrı tutar.
    private void UpdateHover(Vector2 position, bool pointerDown)
    {
        bool ended = gameManager != null && (gameManager.CurrentState == GameManager.GameState.Victory || gameManager.CurrentState == GameManager.GameState.Defeat);
        if (ended || pointerDown || position.x < 0 || position.y < 0 || position.x > Screen.width || position.y > Screen.height)
        {
            ClearHover();
            return;
        }
        UnityEngine.Object target = FindTarget(position);
        if (target is Tower) target = null;
        if (target != hovered)
        {
            hovered = target;
            showAt = Time.unscaledTime + hoverDelay;
            refreshAt = 0f;
            infoPanel.Hide();
        }
        if (hovered == null) { infoPanel.Hide(); return; }
        if (Time.unscaledTime < showAt) return;
        if (Time.unscaledTime >= refreshAt)
        {
            infoPanel.Show(hovered, position);
            refreshAt = Time.unscaledTime + 0.1f;
        }
        else if (infoPanel.IsVisible) infoPanel.MoveTo(position);
    }

    // UI önce gelir. Boş panel alanında bile arkasındaki dünya nesnesini seçme.
    public UnityEngine.Object FindTarget(Vector2 screenPosition)
    {
        if (EventSystem.current != null)
        {
            if (pointer == null || pointerEventSystem != EventSystem.current)
            {
                pointerEventSystem = EventSystem.current;
                pointer = new PointerEventData(pointerEventSystem);
            }
            pointer.position = screenPosition;
            uiHits.Clear();
            pointerEventSystem.RaycastAll(pointer, uiHits);
            foreach (var hit in uiHits)
            {
                if (!(hit.module is GraphicRaycaster)) continue;
                var slot = hit.gameObject.GetComponentInParent<ShopSlotUI>();
                if (slot != null) return slot.isPurchased ? null : slot.currentItem;
                var item = hit.gameObject.GetComponentInParent<BackpackDraggable>();
                return item;
            }
        }
        if (worldCamera == null) worldCamera = Camera.main;
        if (worldCamera == null) return null;
        RaycastHit[] hits = Physics.RaycastAll(worldCamera.ScreenPointToRay(screenPosition), worldCamera.farClipPlane, ~0, QueryTriggerInteraction.Collide);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (var hit in hits)
        {
            var collider = hit.collider;
            var tower = collider.GetComponentInParent<Tower>();
            // Kule menzil küresi görsel gövde değildir; düşman/birlik trigger'ları ise gövdedir.
            if (tower != null && collider.isTrigger) continue;
            var unit = collider.GetComponentInParent<Unit>();
            if (unit != null) return unit.CurrentHealth > 0 ? unit : null;
            var enemy = collider.GetComponentInParent<Enemy>();
            if (enemy != null) return enemy.CurrentHealth > 0 ? enemy : null;
            if (tower != null) return tower;
            if (!collider.isTrigger) return null; // Duvar/zemin arkasını seçme.
        }
        return null;
    }

    private void ClearHover()
    {
        hovered = null;
        if (infoPanel != null) infoPanel.Hide();
    }
    private void OnDisable()
    {
        rangeTower = null;
        if (rangeLine != null) rangeLine.enabled = false;
        ClearHover();
    }
    private void OnDestroy() { if (rangeMaterial != null) Destroy(rangeMaterial); if (Instance == this) Instance = null; }
}
