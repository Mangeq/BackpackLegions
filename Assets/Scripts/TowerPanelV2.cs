using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed partial class TowerPanelLayout
{
    [SerializeField] Material selectedFace;
    [SerializeField] TMP_Text shopHeading;
    static void ArtPlace(RectTransform r,float x,float y,float w,float h)
    {Place(r,x*Width/1660f,y*Height/948f,w*Width/1660f,h*Height/948f);}
    Sprite ArtCrop(string name,float x,float y,float w,float h)
    {
        var cached=Resources.Load<Sprite>("UI/ApprovedScene/ApprovedV2_"+name);
        if(cached!=null)return cached;
        var texture=Resources.Load<Texture2D>("UI/TowerPanel/Reference/PanelV2");
        var sprite=Sprite.Create(texture,new Rect(x/1660f*texture.width,(948-y-h)/948f*texture.height,w/1660f*texture.width,h/948f*texture.height),Vector2.one*.5f,100,0,SpriteMeshType.FullRect);
        sprite.name="ApprovedV2_"+name;return sprite;
    }
    void ApplyPanelV2()
    {
        if(root==null||Resources.Load<Texture2D>("UI/TowerPanel/Reference/PanelV2")==null)return;
        root.GetComponent<Image>().sprite=ReferencePanelAssets.Sprite("PanelV2");
        foreach(var name in new[]{"ElementRowCover","TitleSource","DamageSource","IntervalSource","CriticalSource","RangeSource"})Hide(root,name);
        foreach(var value in stats)if(value.original!=null)value.original.gameObject.SetActive(false);
        if(title.original!=null)title.original.gameObject.SetActive(false);
        foreach(var button in ui.elementButtons)button.gameObject.SetActive(false);
        status.text.gameObject.SetActive(false);
        ArtPlace(title.text.rectTransform,100,44,770,49);title.original=null;title.text.fontSize=44;
        for(int i=0;i<4;i++){ArtPlace(stats[i].text.rectTransform,355,96+i*43,160,39);stats[i].text.fontSize=32;}
        for(int i=0;i<3;i++){ArtPlace(details[i].text.rectTransform,515,99+i*43,360,36);details[i].text.fontSize=22;}
        ArtPlace(xp.text.rectTransform,906,49,680,30);
        ArtPlace(progress.rectTransform,909,86,548,13);
        Place(bags.gridContainer,80*Width/1660f,393*Height/948f,Cell*5,Cell*5);
        bags.gridContainer.localScale=new Vector3(500*Width/1660f/(Cell*5),410*Height/948f/(Cell*5),1);
        ArtPlace(storageScroll.viewport,650,393,230,502);
        storageScroll.viewport.GetComponent<Image>().color=Color.clear;
        storageScroll.viewport.GetComponent<Image>().sprite=null;
        bags.storageGridContainer.localScale=Vector3.one;
        var physics=bags.storageGridContainer.GetComponent<StoragePileUI>();
        if(physics!=null){physics.enabled=false;if(Application.isPlaying)Destroy(physics);else DestroyImmediate(physics);}
        var depot=bags.storageGridContainer.GetComponent<StorageGridUI>();
        if(depot==null)depot=bags.storageGridContainer.gameObject.AddComponent<StorageGridUI>();
        depot.cellSprite=ArtCrop("StorageCell",650,393,115,83.67f);
        depot.Prepare(bags.ActiveTower!=null?bags.ActiveTower.Storage.Items.Count:0);
        var bulk=ArtButton("MoveAllToStorage",new Rect(766,312,171,58));
        if(Application.isPlaying){bulk.onClick.RemoveAllListeners();bulk.onClick.AddListener(bags.MoveAllToStorage);}
        SetupExisting(ui.openElementChangeButton,new Rect(74,817,244,72),"ChangeElement");
        SetupExisting(ui.openTypeChangeButton,new Rect(329,817,253,72),"ChangeType");
        if(Application.isPlaying)
        {
            ui.openElementChangeButton.onClick.RemoveAllListeners();ui.openElementChangeButton.onClick.AddListener(ui.OpenElementChange);
            ui.openTypeChangeButton.onClick.RemoveAllListeners();ui.openTypeChangeButton.onClick.AddListener(ui.OpenTowerTypeChange);
        }
        if(selectedFace==null)
        {
            selectedFace=Resources.Load<Material>("UI/ApprovedScene/PanelSelected");
            if(selectedFace==null)selectedFace=new Material(Shader.Find("UI/PanelSelection")){name="PanelSelected"};
        }
        var both=new Rect(1050,144,151,39);var first=new Rect(1050,188,222,39);
        SetupTarget(ui.targetTypeButtons[0],both,"İkisi de");
        SetupTarget(ui.targetTypeButtons[1],new Rect(1213,144,151,39),"Kara");
        SetupTarget(ui.targetTypeButtons[2],new Rect(1375,144,171,39),"Hava");
        SetupTarget(ui.targetOrderButtons[0],first,"İlk giren");
        SetupTarget(ui.targetOrderButtons[1],new Rect(1297,188,249,39),"Son giren");
        ArtPlace((RectTransform)shop.offersScroll.transform,955,389,640,353);
        var grid=shop.offersScroll.content.GetComponent<GridLayoutGroup>();
        grid.cellSize=new Vector2(330,177);grid.spacing=new Vector2(10,4);
        shop.offersScroll.content.sizeDelta=new Vector2(0,358);
        shop.offersScroll.content.localScale=new Vector3((640*Width/1660f)/670f,(353*Height/948f)/358f,1);
        shop.offersScroll.vertical=false;
        shopShelf=ArtCrop("Shelf",956,389,310,170);
        foreach(var card in cards)StyleShelfCard(card);
        SetupExisting(shop.rerollButton,new Rect(1043,748,459,62),"Reroll");
        ArtPlace(offers.text.rectTransform,967,352,620,31);offers.text.alignment=TextAlignmentOptions.Center;
        var heading=root.Find("ShopHeading") as RectTransform;
        if(heading==null)
        {
            var label=MakeValue("ShopHeading",root,0,0,10,10,33,cream,null,null,serif);
            shopHeading=label.text;
        }
        else shopHeading=heading.GetComponent<TMP_Text>();
        // A clean strip from the same header covers only its baked heading.
        var cover=root.Find("ShopHeadingCover") as RectTransform;
        if(cover==null){cover=NewRect("ShopHeadingCover",root);cover.gameObject.AddComponent<Image>();}
        ArtPlace(cover,958,314,630,38);var coverImage=cover.GetComponent<Image>();
        coverImage.sprite=ArtCrop("HeaderWood",970,318,90,34);coverImage.raycastTarget=false;
        ArtPlace(shopHeading.rectTransform,958,311,630,41);shopHeading.alignment=TextAlignmentOptions.Center;
        shopHeading.text="EŞYA MAĞAZASI";shopHeading.transform.SetAsLastSibling();
        if(bags.sellArea!=null)
        {
            bags.sellArea.SetParent(root,false);bags.sellArea.gameObject.SetActive(true);
            ArtPlace(bags.sellArea,950,815,647,96);
            foreach(var graphic in bags.sellArea.GetComponentsInChildren<Graphic>(true))graphic.enabled=false;
            var sell=bags.sellArea.GetComponent<Image>();sell.enabled=true;sell.sprite=null;sell.color=Color.clear;sell.raycastTarget=true;
        }
        RefreshPanelV2(null);Resize();
    }
    Button ArtButton(string name,Rect rect)
    {
        var r=root.Find(name) as RectTransform;
        if(r==null){r=NewRect(name,root);r.gameObject.AddComponent<Image>();r.gameObject.AddComponent<Button>();}
        var button=r.GetComponent<Button>();SetupExisting(button,rect,name);return button;
    }
    void SetupExisting(Button button,Rect rect,string name)
    {
        button.gameObject.SetActive(true);button.transform.SetParent(root,false);
        ArtPlace((RectTransform)button.transform,rect.x,rect.y,rect.width,rect.height);
        foreach(var graphic in button.GetComponentsInChildren<Graphic>(true))graphic.enabled=false;
        var face=button.GetComponent<Image>();face.enabled=true;face.sprite=ArtCrop(name,rect.x,rect.y,rect.width,rect.height);
        face.type=Image.Type.Simple;face.color=Color.white;face.raycastTarget=true;button.targetGraphic=face;
        var colors=button.colors;colors.normalColor=colors.selectedColor=Color.white;colors.disabledColor=new Color(.65f,.65f,.65f);button.colors=colors;
    }
    void SetupTarget(Button button,Rect rect,string text)
    {
        SetupExisting(button,rect,"Target"+text);
        var caption=button.transform.Find("LiveCaption") as RectTransform;
        if(caption==null)caption=NewRect("LiveCaption",button.transform);
        var label=caption.GetComponent<TextMeshProUGUI>();if(label==null)label=caption.gameObject.AddComponent<TextMeshProUGUI>();
        Stretch(caption);label.font=body;label.text=text;label.fontSize=24;label.alignment=TextAlignmentOptions.Center;label.color=cream;label.raycastTarget=false;label.enabled=true;
    }
    void RefreshPanelV2(Tower tower)
    {
        if(selectedFace==null)return;
        for(int i=0;i<ui.targetTypeButtons.Length;i++)ui.targetTypeButtons[i].GetComponent<Image>().material=i==(tower!=null?(int)tower.SelectedTargetType:0)?selectedFace:null;
        for(int i=0;i<ui.targetOrderButtons.Length;i++)ui.targetOrderButtons[i].GetComponent<Image>().material=i==(tower!=null?(int)tower.SelectedTargetOrder:0)?selectedFace:null;
        if(shopHeading!=null)shopHeading.text=tower!=null&&tower.IsElementShop?"ELEMENT MAĞAZASI":"EŞYA MAĞAZASI";
    }
}
