using UnityEngine;
using UnityEngine.UI;

public enum CellHighlightState
{
    None,
    Valid,
    Invalid
}

/// <summary>
/// Backpack grid'indeki TEK bir hücrenin görsel temsili.
/// Kasıtlı olarak "dumb" (aptal) - eski GridCell.cs'in aksine burada hiçbir
/// OnDrop, hiçbir CheckValidPlacement/PlaceItem çağrısı YOK. Sadece kendi
/// koordinatını taşır ve kendi rengini gösterir. Yerleştirme kararı tamamen
/// BackpackDraggable + Backpack'te veriliyor.
/// </summary>
public class BackpackCellView : MonoBehaviour
{
    public int GridX { get; private set; }
    public int GridY { get; private set; }

    [SerializeField] private Image background;

    public void Setup(int x, int y)
    {
        GridX = x;
        GridY = y;
        if (background != null && BackpackUIManager.Instance != null &&
            BackpackUIManager.Instance.GetComponent<TowerPanelLayout>() != null)
        {
            background.sprite = null;
            background.type = Image.Type.Simple;
            var outline=GetComponent<UnityEngine.UI.Outline>();
            if(outline!=null)outline.enabled=false;
        }
    }

    private RectTransform hatch;
    public void SetHighlight(CellHighlightState state)
    {
        if (background == null || BackpackUIManager.Instance == null)
            return;

        if(state==CellHighlightState.Invalid && !hatch)
        {
            var go=new GameObject("Blocked cell",typeof(RectTransform),typeof(Image));
            hatch=(RectTransform)go.transform;hatch.SetParent(transform,false);
            hatch.sizeDelta=new Vector2(((RectTransform)transform).rect.width*.8f,3);
            hatch.localRotation=Quaternion.Euler(0,0,40);
            var stripe=go.GetComponent<Image>();stripe.raycastTarget=false;stripe.color=new Color(1,.8f,.6f);
        }
        if(hatch)hatch.gameObject.SetActive(state==CellHighlightState.Invalid);
        switch (state)
        {
            case CellHighlightState.Valid:
                background.color = BackpackUIManager.Instance.validColor;
                break;
            case CellHighlightState.Invalid:
                background.color = BackpackUIManager.Instance.invalidColor;
                break;
            default:
                background.color = BackpackUIManager.Instance.defaultCellColor;
                break;
        }
    }
}
