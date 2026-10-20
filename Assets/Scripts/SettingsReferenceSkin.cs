using UnityEngine;

public sealed class SettingsReferenceSkin : MonoBehaviour
{
    [SerializeField] RectTransform card;
    public void Configure(RectTransform rect) { card=rect;Fit(); }
    void LateUpdate() { if(card!=null&&card.gameObject.activeInHierarchy)Fit(); }
    void Fit()
    {
        if(card==null||card.parent==null)return;
        var area=((RectTransform)card.parent).rect;float scale=Mathf.Min(area.width*.92f/1024,area.height*.88f/874);
        card.localScale=new Vector3(scale,scale,1);card.anchoredPosition=Vector2.zero;
    }
}
