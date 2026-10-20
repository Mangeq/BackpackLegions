using System.Collections.Generic;
using UnityEngine;

/// <summary>The approved wood/parchment purchase card shared by all four tower types.</summary>
public sealed class TowerPurchaseSkin : MonoBehaviour
{
    [SerializeField] UİManager ui;
    [SerializeField] RectTransform root;
    [SerializeField] UnityEngine.UI.Button[] tabs;
    [SerializeField] UnityEngine.UI.Outline[] tabOutlines;
    [SerializeField] GameObject[] tabHighlights;
    [SerializeField] UnityEngine.UI.Button buy;
    Vector2 parentSize;
    // Rasterize dynamic text above screen density instead of magnifying 10px glyphs.
    const int TextDensity=4;
    static readonly Dictionary<string,Sprite> sprites=new Dictionary<string,Sprite>();

    public static void Install(UİManager owner)
    {
        if(UİManager.TowerShopUI==null||owner.GetComponent<TowerPurchaseSkin>()!=null)return;
        owner.gameObject.AddComponent<TowerPurchaseSkin>().Initialize(owner);
    }
    static Sprite Load(string name)
    {
        if(sprites.TryGetValue(name,out Sprite sprite)&&sprite!=null)return sprite;
        bool cardPart=name=="Buy"||name=="Close";
        string asset=name=="Card"||cardPart?"CardHD":name;
        if(name=="MagicTab"||name=="MortarTab"||name=="ArcherTab"||name=="UnitTab")asset=name+"Arch";
        var texture=Resources.Load<Texture2D>("UI/TowerPurchase/Reference/"+asset);
        if(texture==null&&asset!=name){texture=Resources.Load<Texture2D>("UI/TowerPurchase/Reference/"+name);cardPart=false;}
        if(texture==null)throw new System.InvalidOperationException("Missing approved tower purchase art: "+name);
        var area=new Rect(0,0,texture.width,texture.height);
        if(cardPart)
        {
            // Use the same restored pixels for the interactive overlays so no
            // low-resolution close/buy art is drawn over the high-resolution card.
            var source=name=="Buy"?new Rect(13,290,226,48):new Rect(212,8,26,28);
            area=new Rect(source.x/250f*texture.width,(381-source.y-source.height)/381f*texture.height,
                source.width/250f*texture.width,source.height/381f*texture.height);
        }
        sprite=Sprite.Create(texture,area,Vector2.one*.5f,100,0,SpriteMeshType.FullRect);
        sprite.name="TowerPurchaseReference_"+name;sprites[name]=sprite;return sprite;
    }
    void Initialize(UİManager owner)
    {
        ui=owner;root=(RectTransform)UİManager.TowerShopUI.transform;
        var card=(RectTransform)ui.purchaseDetails.transform;
        var rail=root.Find("IconRail") as RectTransform;
        foreach(var child in root.GetComponentsInChildren<UnityEngine.UI.Graphic>(true))child.enabled=false;
        foreach(var effect in root.GetComponentsInChildren<UnityEngine.UI.Shadow>(true))effect.enabled=false;
        var original=new List<Transform>();foreach(Transform child in root)original.Add(child);
        foreach(var child in original)if(child!=card&&child!=rail)child.gameObject.SetActive(false);
        Place(card,0,0,250,381);Surface(card,"Card",true);
        // Only the four live buttons belong here. The screenshot rail contained
        // both a baked gold selection and an unused extension below the last tab.
        Place(rail,251,60,45,229);
        var magic=ui.magicSelectionHighlight.transform.parent.GetComponent<UnityEngine.UI.Button>();
        var mortar=ui.cannonSelectionHighlight.transform.parent.GetComponent<UnityEngine.UI.Button>();
        var archer=ui.archerSelectionHighlight.transform.parent.GetComponent<UnityEngine.UI.Button>();
        var unit=ui.unitSelectionHighlight.transform.parent.GetComponent<UnityEngine.UI.Button>();
        tabs=new[]{magic,mortar,archer,unit};
        tabHighlights=new[]{ui.magicSelectionHighlight,ui.cannonSelectionHighlight,ui.archerSelectionHighlight,ui.unitSelectionHighlight};
        tabOutlines=new UnityEngine.UI.Outline[tabs.Length];
        string[] faces={"MagicTab","MortarTab","ArcherTab","UnitTab"};
        float[] y={0,58,115,171};float[] heights={58,57,56,58};
        for(int i=0;i<tabs.Length;i++)
        {
            tabs[i].transform.SetParent(rail,false);
            Place((RectTransform)tabs[i].transform,0,y[i],45,heights[i]);
            Surface((RectTransform)tabs[i].transform,faces[i],true);
            tabs[i].targetGraphic=tabs[i].GetComponent<UnityEngine.UI.Image>();
            NeutralColors(tabs[i]);
            foreach(Transform child in tabHighlights[i].transform)child.gameObject.SetActive(false);
            var selected=(RectTransform)tabHighlights[i].transform;
            Place(selected,0,0,45,heights[i]);
            // The old square rim would reintroduce opaque corners. Follow the
            // transparent sprite silhouette instead, including its curved top.
            var outline=tabs[i].GetComponent<UnityEngine.UI.Outline>();
            if(outline==null)outline=tabs[i].gameObject.AddComponent<UnityEngine.UI.Outline>();
            outline.effectColor=new Color(.95f,.64f,.2f,1);
            outline.effectDistance=new Vector2(.7f,-.7f);outline.useGraphicAlpha=true;
            outline.enabled=false;tabOutlines[i]=outline;
        }
        // These are the existing bound labels, portraits and persistent callbacks.
        var font=GameFonts.BodySource;
        Label(ui.purchaseTitle,14,10,194,25,16,new Color(.96f,.92f,.82f),font);
        Label(ui.purchaseDescription,122,58,109,97,10,new Color(.08f,.065f,.035f),font);
        Label(ui.purchaseStats,21,178,157,91,11,new Color(.06f,.045f,.025f),font);
        Label(ui.purchaseStatValues,177,178,49,91,11,new Color(.06f,.045f,.025f),font);
        ui.purchaseStatValues.alignment=TextAnchor.UpperRight;
        ui.purchaseStats.lineSpacing=ui.purchaseStatValues.lineSpacing=1.08f;
        ui.purchasePortrait.enabled=true;ui.purchasePortrait.color=Color.white;ui.purchasePortrait.preserveAspect=true;
        Place(ui.purchasePortrait.rectTransform,34,64,70,94);
        // Retain the existing purchase callback instead of introducing new logic.
        buy=ui.purchasePrice.transform.parent.GetComponent<UnityEngine.UI.Button>();
        Place((RectTransform)buy.transform,13,290,226,48);Surface((RectTransform)buy.transform,"Buy",true);
        buy.targetGraphic=buy.GetComponent<UnityEngine.UI.Image>();NeutralColors(buy);
        Label(ui.purchasePrice,22,12,184,24,14,new Color(.96f,.92f,.82f),font);
        ui.purchasePrice.alignment=TextAnchor.MiddleCenter;
        var shadow=ui.purchasePrice.GetComponent<UnityEngine.UI.Shadow>();
        if(shadow==null)shadow=ui.purchasePrice.gameObject.AddComponent<UnityEngine.UI.Shadow>();
        shadow.enabled=true;shadow.effectColor=Color.black;shadow.effectDistance=new Vector2(TextDensity,-TextDensity);
        var close=card.Find("CloseCard") as RectTransform;
        Place(close,212,8,26,28);Surface(close,"Close",true);
        var closeButton=close.GetComponent<UnityEngine.UI.Button>();closeButton.targetGraphic=close.GetComponent<UnityEngine.UI.Image>();NeutralColors(closeButton);
        Label(ui.towerPurchaseFeedback,21,266,208,20,10,new Color(.5f,.08f,.04f),font);
        ui.towerPurchaseFeedback.alignment=TextAnchor.MiddleCenter;
        Resize();
    }
    void LateUpdate()
    {
        if(root==null)return;
        if(((RectTransform)root.parent).rect.size!=parentSize)Resize();
        if(!root.gameObject.activeInHierarchy)return;
        string selected=UİManager.selectedTowerType;
        for(int i=0;i<tabs.Length;i++)
        {
            bool active=tabHighlights[i].activeInHierarchy;
            tabOutlines[i].enabled=active;
            tabs[i].targetGraphic.color=active?new Color(1,.94f,.77f):Color.white;
        }
        if(selected=="UnitTower")ui.purchasePortrait.sprite=Load("UnitPortrait");
        else if(selected=="MegicTower")ui.purchasePortrait.sprite=Load("MagicPortrait");
        else if(selected=="CannonTower")ui.purchasePortrait.sprite=Load("MortarPortrait");
        else if(selected=="ArcherTower")ui.purchasePortrait.sprite=Load("ArcherPortrait");
        bool insufficient=ui.towerPurchaseFeedback.text.StartsWith("Yetersiz");
        ui.towerPurchaseFeedback.enabled=insufficient;
        if(insufficient)ui.towerPurchaseFeedback.color=new Color(.55f,.08f,.04f);
        // Use the original button art, including its black outline and metal ends.
        buy.targetGraphic.color=buy.interactable?Color.white:new Color(.67f,.67f,.67f);
    }
    void Resize()
    {
        parentSize=((RectTransform)root.parent).rect.size;
        float scale=Mathf.Min(parentSize.x/1024f,parentSize.y/572f);
        root.anchorMin=root.anchorMax=root.pivot=new Vector2(1,.5f);
        root.sizeDelta=new Vector2(300,381);root.localScale=Vector3.one*scale;
        root.anchoredPosition=new Vector2(-4*scale,-21.5f*scale);
    }
    static void Surface(RectTransform rect,string resource,bool ray)
    {
        var image=rect.GetComponent<UnityEngine.UI.Image>();
        if(image==null)image=rect.gameObject.AddComponent<UnityEngine.UI.Image>();
        image.enabled=true;image.sprite=Load(resource);image.type=UnityEngine.UI.Image.Type.Simple;
        image.color=Color.white;image.raycastTarget=ray;
    }
    static void Label(UnityEngine.UI.Text text,float x,float y,float w,float h,int size,Color color,Font font)
    {
        Place(text.rectTransform,x,y,w,h);if(font!=null)text.font=font;
        text.rectTransform.sizeDelta*=TextDensity;text.rectTransform.localScale=Vector3.one/TextDensity;
        text.gameObject.SetActive(true);text.enabled=true;text.fontSize=size*TextDensity;text.fontStyle=FontStyle.Normal;
        text.color=color;text.raycastTarget=false;text.horizontalOverflow=HorizontalWrapMode.Wrap;text.verticalOverflow=VerticalWrapMode.Truncate;
    }
    static void NeutralColors(UnityEngine.UI.Button button)
    {
        var colors=button.colors;colors.normalColor=colors.selectedColor=Color.white;
        colors.disabledColor=Color.white;colors.highlightedColor=new Color(1.06f,1.06f,1.06f);
        colors.pressedColor=new Color(.82f,.82f,.82f);button.colors=colors;
    }
    static void Place(RectTransform rect,float x,float y,float w,float h)
    {
        rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(0,1);rect.localScale=Vector3.one;
        rect.anchoredPosition=new Vector2(x,-y);rect.sizeDelta=new Vector2(w,h);
    }
}
