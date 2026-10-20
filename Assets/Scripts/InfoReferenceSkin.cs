using TMPro;
using UnityEngine;

/// <summary>Keep every native item/tower/enemy description in the reference tooltip card.</summary>
public sealed class InfoReferenceSkin : MonoBehaviour
{
    [SerializeField] InfoPanelUI info;
    [SerializeField] TMP_Text title,subtitle,body;
    public void Configure(InfoPanelUI owner,TMP_Text heading,TMP_Text rarity,TMP_Text description)
    {info=owner;title=heading;subtitle=rarity;body=description;}
    public void Refresh()
    {
        if(info==null||title==null)return;
        title.text=info.titleText.text;subtitle.text=info.subtitleText.text;subtitle.color=info.subtitleText.color;body.text=info.bodyText.text;
        float desired=body.GetPreferredValues(body.text,298,2000).y+162;
        var parent=(RectTransform)info.panel.parent;
        float height=Mathf.Min(Mathf.Max(356,desired),Mathf.Max(200,Mathf.Min(720,parent.rect.height-16)));
        info.panel.sizeDelta=new Vector2(350,height);body.rectTransform.sizeDelta=new Vector2(298,Mathf.Max(40,height-158));
    }
}
