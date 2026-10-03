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
    }

    public void SetHighlight(CellHighlightState state)
    {
        if (background == null || BackpackUIManager.Instance == null)
            return;

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