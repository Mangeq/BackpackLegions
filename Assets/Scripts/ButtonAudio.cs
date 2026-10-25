using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// EventSystem handlers survive UI code replacing Button.onClick during refreshes.
[RequireComponent(typeof(Button))]
public sealed class ButtonAudio : MonoBehaviour, IPointerClickHandler, ISubmitHandler
{
    Button button;
    void Awake() { button = GetComponent<Button>(); }
    void Play()
    {
        if (button == null || !button.isActiveAndEnabled || !button.IsInteractable()) return;
        string id = name.ToLowerInvariant();
        int variant = id.Contains("close") || id.Contains("back") ? 2
            : id.Contains("buy") || id.Contains("confirm") || id.Contains("unlock") ? 3
            : id.Contains("start") || id.Contains("play") || id.Contains("next") ? 4
            : id.Contains("speed") || id.Contains("target") ? 5
            : id.Contains("refresh") || id.Contains("reset") ? 6 : 1;
        GameAudio.Play("click_" + variant, .45f);
    }
    public void OnPointerClick(PointerEventData data) { if (data.button == PointerEventData.InputButton.Left) Play(); }
    public void OnSubmit(BaseEventData data) { Play(); }
}
