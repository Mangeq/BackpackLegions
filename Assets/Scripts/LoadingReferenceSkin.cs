using UnityEngine;

/// <summary>Fits the supplied loading artwork without stretching or cropping its frame.</summary>
[ExecuteAlways]
public sealed class LoadingReferenceSkin : MonoBehaviour
{
    [SerializeField] RectTransform artwork;
    public void Configure(RectTransform stage) { artwork=stage; Fit(); }
    void LateUpdate() { Fit(); }
    void Fit()
    {
        if(artwork==null || artwork.parent==null)return;
        var area=((RectTransform)artwork.parent).rect;
        float scale=Mathf.Min(area.width/1024f,area.height/565f);
        artwork.localScale=new Vector3(scale,scale,1);
        artwork.anchoredPosition=Vector2.zero;
    }
}
