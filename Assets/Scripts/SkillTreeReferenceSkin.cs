using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Reference artwork with live skill data and the existing unlock callbacks.</summary>
public sealed class SkillTreeReferenceSkin : MonoBehaviour
{
    [SerializeField] SkillTreePanelUI ui;
    [SerializeField] RectTransform card;
    [SerializeField] Sprite nodeFace, selectedNodeFace, tabFace, selectedTabFace, lockedAction;
    [SerializeField] TMP_FontAsset font;
    readonly Color cream=new Color(.94f,.88f,.74f);
    public static Sprite Art(string name)=>Resources.Load<Sprite>("UI/FrontendReference/Sprites/"+name);
    public static void Place(RectTransform r,Transform parent,float x,float y,float w,float h)
    {
        r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);
        r.localScale=Vector3.one;r.localRotation=Quaternion.identity;r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);
    }
    public void Configure(SkillTreePanelUI owner)
    {
        ui=owner;card=(RectTransform)ui.nodes[0].transform.parent;
        font=GameFonts.Body;
        nodeFace=Art("NodeTransparent");selectedNodeFace=Art("SelectedNodeTransparent");tabFace=Art("Tab");selectedTabFace=Art("SelectedTab");lockedAction=Art("UnlockLocked");
        foreach(var graphic in card.GetComponentsInChildren<Graphic>(true))graphic.enabled=false;
        foreach(var effect in card.GetComponentsInChildren<Shadow>(true))effect.enabled=false;
        var background=card.GetComponent<Image>();background.enabled=true;background.sprite=Art("SkillBackground");background.color=Color.white;background.type=Image.Type.Simple;background.raycastTarget=true;
        card.anchorMin=card.anchorMax=card.pivot=Vector2.one*.5f;card.anchoredPosition=Vector2.zero;card.sizeDelta=new Vector2(1024,572);
        foreach(var name in new[]{"TreeBranches","ArcherBranches","DataConnections"}){var child=card.Find(name);if(child!=null)child.gameObject.SetActive(false);}
        var close=card.Find("Close").GetComponent<Button>();ButtonFace(close,Art("Close"));Place((RectTransform)close.transform,card,894,31,111,35);
        ButtonFace(ui.resetButton,Art("Reset"));Place((RectTransform)ui.resetButton.transform,card,30,498,136,28);
        Label(ui.pointsLabel,card,640,35,245,26,13);
        Label(ui.branchTitle,card,197,120,510,29,20);ui.branchTitle.fontStyle=FontStyles.Bold;
        Label(ui.branchSubtitle,card,197,151,510,23,11);
        Label(ui.detailIndex,card,748,130,235,21,11);
        Label(ui.detailTitle,card,748,251,235,62,22);ui.detailTitle.fontStyle=FontStyles.Bold;
        Label(ui.detailBody,card,748,346,235,103,12);ui.detailBody.enableWordWrapping=true;
        Label(ui.rulesLabel,card,748,501,235,23,9);ui.rulesLabel.enableWordWrapping=true;
        ui.detailIcon.enabled=true;ui.detailIcon.color=Color.white;ui.detailIcon.preserveAspect=true;
        Place(ui.detailIcon.rectTransform,card,751,162,59,62);
        ButtonFace(ui.unlockButton,lockedAction);Place((RectTransform)ui.unlockButton.transform,card,745,461,239,36);
        Label(ui.unlockLabel,ui.unlockButton.transform,5,0,229,36,15);ui.unlockLabel.alignment=TextAlignmentOptions.Center;ui.unlockLabel.fontStyle=FontStyles.Bold;
        string[] names={"OKÇU","BÜYÜCÜ","HAVAN","BİRLİK"};
        for(int i=0;i<ui.tabs.Length;i++)
        {
            ButtonFace(ui.tabs[i].GetComponent<Button>(),tabFace);Place(ui.tabs[i].rectTransform,card,20,126+i*75,156,65);
            var caption=ui.tabs[i].GetComponentInChildren<TMP_Text>(true);Label(caption,ui.tabs[i].transform,9,6,133,48,15);caption.text=names[i];caption.alignment=TextAlignmentOptions.Center;caption.fontStyle=FontStyles.Bold;
        }
        Fit();ui.SelectBranch(0);
    }
    void Label(TMP_Text label,Transform parent,float x,float y,float w,float h,float size)
    {
        Place(label.rectTransform,parent,x,y,w,h);label.enabled=true;
        label.font=label.GetComponentInParent<Button>()!=null||label==ui.branchTitle||label==ui.detailTitle?GameFonts.Title:font;
        label.fontSize=size;label.color=cream;label.raycastTarget=false;label.alignment=TextAlignmentOptions.TopLeft;label.fontStyle=FontStyles.Normal;
        label.enableAutoSizing=false;label.enableWordWrapping=false;label.margin=Vector4.zero;
    }
    static void ButtonFace(Button button,Sprite sprite)
    {
        var image=button.GetComponent<Image>();image.enabled=true;image.sprite=sprite;image.type=Image.Type.Simple;image.color=Color.white;image.raycastTarget=true;button.targetGraphic=image;
        var colors=ColorBlock.defaultColorBlock;colors.highlightedColor=new Color(1.08f,1.08f,1.08f);colors.selectedColor=Color.white;colors.disabledColor=Color.white;button.colors=colors;
    }
    public void LayoutBranch(SkillTreeData tree,int branch)
    {
        for(int i=0;i<ui.tabs.Length;i++)
        {
            ui.tabs[i].sprite=i==branch?selectedTabFace:tabFace;ui.tabs[i].color=Color.white;
            var caption=ui.tabs[i].GetComponentInChildren<TMP_Text>(true);
            if(caption!=null){caption.font=GameFonts.Title;caption.fontStyle=FontStyles.Normal;}
        }
        for(int i=0;i<tree.nodes.Count;i++)
        {
            if(tree.nodes[i]==null||tree.nodes[i].skill==null)continue;
            var p=tree.nodes[i].position;
            // Existing authoring coordinates continue to control every node.
            float x=196+p.x*.635f,y=184+p.y*.64f;
            Place(ui.nodes[i].rectTransform,card,x-50,y-27,100,54);
            ButtonFace(ui.nodes[i].GetComponent<Button>(),nodeFace);
            ui.nodeIcons[i].enabled=true;ui.nodeIcons[i].color=Color.white;ui.nodeIcons[i].preserveAspect=true;Place(ui.nodeIcons[i].rectTransform,ui.nodes[i].transform,6,18,18,19);
            Label(ui.nodeLabels[i],ui.nodes[i].transform,28,8,66,39,10);ui.nodeLabels[i].enableWordWrapping=true;ui.nodeLabels[i].alignment=TextAlignmentOptions.MidlineLeft;
            ui.nodeLabels[i].enableAutoSizing=true;ui.nodeLabels[i].fontSizeMin=8;ui.nodeLabels[i].fontSizeMax=10;
        }
    }
    public void RefreshSelection(SkillTreeData tree,int selected)
    {
        var progress=new SkillTreeProgress();
        for(int i=0;i<tree.nodes.Count;i++)
        {
            var skill=tree.nodes[i].skill;if(skill==null)continue;
            ui.nodes[i].sprite=i==selected?selectedNodeFace:nodeFace;
            ui.nodes[i].color=i!=selected&&progress.IsUnlocked(skill.id)?new Color(.8f,1,.8f):Color.white;
        }
        ui.unlockButton.GetComponent<Image>().sprite=ui.unlockButton.interactable?selectedNodeFace:lockedAction;
        ui.unlockLabel.color=ui.unlockButton.interactable?cream:new Color(.64f,.46f,.38f);
    }
    void Fit()
    {
        if(card==null)return;var parent=(RectTransform)card.parent;
        card.localScale=Vector3.one*Mathf.Min(1600f/1024,Mathf.Min(parent.rect.width/1024,parent.rect.height/572));
    }
    void OnEnable(){Fit();}
    void LateUpdate(){Fit();}
}
