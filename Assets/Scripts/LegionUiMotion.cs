using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// UI motions run on unscaled time so preparation and results can animate while combat is stopped.
public sealed class LegionUiMotion : MonoBehaviour
{
    Vector3 restScale,restPosition;
    bool moving;
    public static void Pulse(Transform target,bool shake=false,Vector3? from=null)
    {
        if(!target || !target.gameObject.activeInHierarchy) return;
        var fx=target.GetComponent<LegionUiMotion>();
        if(!fx) fx=target.gameObject.AddComponent<LegionUiMotion>();
        fx.StopAllCoroutines();fx.Restore();
        fx.StartCoroutine(fx.Bounce(shake,from));
    }
    IEnumerator Bounce(bool shake,Vector3? from)
    {
        restScale=transform.localScale;restPosition=transform.localPosition;moving=true;
        Vector3 start=from.HasValue?transform.parent.InverseTransformPoint(from.Value):restPosition;
        for(float t=0;t<1;t+=Time.unscaledDeltaTime/.26f)
        {
            float wave=Mathf.Sin(Mathf.PI*t);
            transform.localScale=restScale*(1+(shake?0:.14f)*wave);
            transform.localPosition=Vector3.Lerp(start,restPosition,1-(1-t)*(1-t))+
                (shake?Vector3.right*7*Mathf.Sin(4*Mathf.PI*t)*(1-t):Vector3.zero);
            yield return null;
        }
        Restore();
    }
    void Restore()
    {
        if(!moving)return;
        transform.localScale=restScale;transform.localPosition=restPosition;moving=false;
    }
    void OnDisable(){Restore();}

    public static void Link(RectTransform parent,Vector3 start,Vector3 end,Color color)
    {
        if(!parent||!parent.gameObject.activeInHierarchy)return;
        var go=new GameObject("Sinerji izi",typeof(RectTransform),typeof(Image),typeof(LegionUiMotion));
        var rect=(RectTransform)go.transform;rect.SetParent(parent,false);
        var image=go.GetComponent<Image>();image.color=color;image.raycastTarget=false;
        var fx=go.GetComponent<LegionUiMotion>();fx.StartCoroutine(fx.Flow(parent.InverseTransformPoint(start),parent.InverseTransformPoint(end),rect,image));
    }
    IEnumerator Flow(Vector3 a,Vector3 b,RectTransform rect,Image image)
    {
        rect.pivot=new Vector2(0,.5f);
        rect.localRotation=Quaternion.Euler(0,0,Mathf.Atan2(b.y-a.y,b.x-a.x)*Mathf.Rad2Deg);
        for(float t=0;t<1;t+=Time.unscaledDeltaTime/.44f)
        {
            float head=Mathf.Min(1,t*1.8f),tail=Mathf.Max(0,(t-.42f)/.58f);
            rect.localPosition=Vector3.Lerp(a,b,tail);
            rect.sizeDelta=new Vector2(Vector3.Distance(a,b)*(head-tail),4);
            yield return null;
        }
        Destroy(gameObject);
    }

    public static void Banner(string words)
    {
        if(!UİManager.WaveText)return;
        var canvas=UİManager.WaveText.canvas;
        if(!canvas)return;
        var go=new GameObject("Dalga bildirimi",typeof(RectTransform),typeof(TextMeshProUGUI),typeof(LegionUiMotion));
        var rect=(RectTransform)go.transform;rect.SetParent(canvas.rootCanvas.transform,false);
        rect.anchorMin=rect.anchorMax=new Vector2(.5f,.79f);rect.anchoredPosition=Vector2.zero;rect.sizeDelta=new Vector2(620,80);
        var label=go.GetComponent<TMP_Text>();label.text=words;label.font=GameFonts.Title;label.fontSize=32;
        label.alignment=TextAlignmentOptions.Center;label.color=LegionFx.Gold;label.raycastTarget=false;
        var fx=go.GetComponent<LegionUiMotion>();fx.StartCoroutine(fx.Caption(label));
    }
    IEnumerator Caption(TMP_Text label)
    {
        for(float t=0;t<1;t+=Time.unscaledDeltaTime/1.1f)
        {
            label.alpha=Mathf.Clamp01(Mathf.Min(t*8,(1-t)*4));
            yield return null;
        }
        Destroy(gameObject);
    }

    public static IEnumerator Reveal(GameObject panel,TMP_Text summary,bool victory,int gained)
    {
        if(!panel)yield break;
        var group=panel.GetComponent<CanvasGroup>();
        if(!group)group=panel.AddComponent<CanvasGroup>();
        group.alpha=0;
        if(!victory)yield return new WaitForSecondsRealtime(.22f);
        for(float t=0;t<1;t+=Time.unscaledDeltaTime/.30f){group.alpha=t;yield return null;}
        group.alpha=1;
        if(victory && summary && !summary.enabled)
        {
            var go=new GameObject("Dalga ödülleri",typeof(RectTransform),typeof(TextMeshProUGUI));
            var rect=(RectTransform)go.transform;rect.SetParent(panel.transform,false);
            rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(.5f,1);
            rect.anchoredPosition=new Vector2(0,-304);rect.sizeDelta=new Vector2(420,19);
            var award=go.GetComponent<TMP_Text>();award.raycastTarget=false;award.font=GameFonts.Body;
            award.fontSize=16;award.alignment=TextAlignmentOptions.Center;award.color=LegionFx.Gold;
            award.text=EconomyManager.Gold+" ALTIN";
            yield return new WaitForSecondsRealtime(.22f);
            award.text+="   ·   +"+gained+" YETENEK PUANI";
        }
        if(summary && summary.enabled)
        {
            summary.ForceMeshUpdate();
            int count=summary.textInfo.lineCount;
            for(int i=1;i<=count;i++){summary.maxVisibleLines=i;yield return new WaitForSecondsRealtime(.09f);}
            summary.maxVisibleLines=int.MaxValue;
        }
    }
}
