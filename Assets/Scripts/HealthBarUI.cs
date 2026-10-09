using UnityEngine;

// Aynı bileşen hem asker hem düşman prefabında çalışır. Canın sahibi değildir.
public class HealthBarUI : MonoBehaviour
{
    public RectTransform barPrefab;
    public float heightOffset = 0.25f;
    private Unit unit;
    private Enemy enemy;
    private Collider bodyCollider;
    private Camera worldCamera;
    private RectTransform bar, fill;
    private UnityEngine.UI.Text label;
    private Canvas canvas;
    private int lastHealth = -1, lastMax = -1;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        enemy = GetComponent<Enemy>();
        bodyCollider = GetComponent<Collider>();
        worldCamera = Camera.main;
    }

    private void LateUpdate()
    {
        var controller = HoverInfoController.Instance;
        if (controller == null || controller.healthBarLayer == null) { RemoveBar(); return; }
        if (worldCamera == null) worldCamera = Camera.main;
        if (worldCamera == null || barPrefab == null) return;
        float current = unit != null ? unit.CurrentHealth : enemy != null ? enemy.CurrentHealth : 0f;
        float maximum = unit != null ? unit.MaxHealth : enemy != null ? enemy.MaxHealth : 0f;
        if (current <= 0f || maximum <= 0f) { if (bar != null) bar.gameObject.SetActive(false); return; }
        if (bar == null)
        {
            bar = Instantiate(barPrefab, controller.healthBarLayer);
            bar.name = name + " HealthBar";
            fill = (RectTransform)bar.Find("Track/Fill");
            label = bar.Find("HealthText").GetComponent<UnityEngine.UI.Text>();
            canvas = bar.GetComponentInParent<Canvas>();
            fill.GetComponent<UnityEngine.UI.Image>().color = unit != null ? new Color(0.23f, 0.86f, 0.48f) : new Color(0.96f, 0.3f, 0.3f);
        }
        Vector3 position = bodyCollider != null ? new Vector3(bodyCollider.bounds.center.x, bodyCollider.bounds.max.y, bodyCollider.bounds.center.z) : transform.position;
        if (enemy != null && enemy.IsAir && enemy.visualRoot != null)
            position += enemy.visualRoot.position - enemy.transform.position;
        Vector3 screen = worldCamera.WorldToScreenPoint(position + Vector3.up * heightOffset);
        bool visible = screen.z > 0 && screen.x >= 0 && screen.x <= Screen.width && screen.y >= 0 && screen.y <= Screen.height;
        bar.gameObject.SetActive(visible);
        if (!visible) return;
        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(controller.healthBarLayer, screen, uiCamera, out Vector2 point);
        bar.localPosition = new Vector3(point.x, point.y, 0f);
        fill.anchorMax = new Vector2(Mathf.Clamp01(current / maximum), 1f);
        int displayedHealth = Mathf.CeilToInt(current), displayedMax = Mathf.CeilToInt(maximum);
        if (displayedHealth != lastHealth || displayedMax != lastMax)
        {
            label.text = displayedHealth + " / " + displayedMax;
            lastHealth = displayedHealth; lastMax = displayedMax;
        }
    }

    private void RemoveBar()
    {
        if (bar != null) Destroy(bar.gameObject);
        bar = null;
        lastHealth = lastMax = -1;
    }
    private void OnDisable() => RemoveBar();
    private void OnDestroy() => RemoveBar();
}
