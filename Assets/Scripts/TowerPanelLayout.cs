using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>Approved raster artwork, at its original 1678 x 937 coordinates.</summary>
public sealed partial class TowerPanelLayout : MonoBehaviour
{
    const float Width=1678, Height=937, Cell=103;
    [SerializeField] BackpackUIManager bags;
    [SerializeField] UİManager ui;
    [SerializeField] ShopUIManager shop;
    [SerializeField] RectTransform root;
    [SerializeField] UnityEngine.UI.ScrollRect storageScroll;
    [SerializeField] List<Card> cards=new List<Card>();
    [SerializeField] Value[] stats=new Value[4], details=new Value[3];
    [SerializeField] Value title,xp,status,offers;
    [SerializeField] UnityEngine.UI.Image progress;
    [SerializeField] Sprite shopShelf;
    Vector2 canvasSize;
    [SerializeField] TMP_FontAsset serif,body;
    bool ownsFonts;
    readonly Color cream=new Color(1,.95f,.84f),gold=new Color(1,.74f,.35f);

    [System.Serializable] sealed class Value
    {
        public TMP_Text text;
        public UnityEngine.UI.Image original;
        public string match;
        public void Set(string value)
        {
            bool exact=original!=null&&value==match;
            text.enabled=!exact;
            if(original!=null)original.enabled=exact;
            if(!exact&&text.text!=value)text.text=value;
        }
    }
    [System.Serializable] sealed class Card { public ShopSlotUI slot; public Value name,cost,footprint; }

    public static void Install(BackpackUIManager owner)
    {
        if(owner.combinedPanel==null||owner.GetComponent<TowerPanelLayout>()!=null)return;
        owner.gameObject.AddComponent<TowerPanelLayout>().Initialize(owner);
    }
    void Initialize(BackpackUIManager owner)
    {
        bags=owner;ui=FindObjectOfType<UİManager>();shop=FindObjectOfType<ShopUIManager>();
        if(ui==null||shop==null){enabled=false;return;}
        root=(RectTransform)bags.combinedPanel.transform;
        var image=root.GetComponent<UnityEngine.UI.Image>();
        image.sprite=ReferencePanelAssets.Sprite("Background");image.type=UnityEngine.UI.Image.Type.Simple;
        image.color=Color.white;image.raycastTarget=true;
        // Keep the existing callbacks and model objects; replace their presentation.
        foreach(var g in root.GetComponentsInChildren<UnityEngine.UI.Graphic>(true))if(g!=image)g.enabled=false;
        foreach(var effect in root.GetComponentsInChildren<UnityEngine.UI.Shadow>(true))effect.enabled=false;
        Hide(root,"Preview");Hide(root,"Help");Hide(bags.backpackPanel.transform,"GridLayout");
        if(bags.evolutionStatus!=null)bags.evolutionStatus.gameObject.SetActive(false);
        if(bags.sellArea!=null)bags.sellArea.gameObject.SetActive(false);
        if(bags.deliveryContainer!=null)bags.deliveryContainer.gameObject.SetActive(false);
        if(bags.deliveryLabel!=null)bags.deliveryLabel.gameObject.SetActive(false);
        Hide(bags.storageGridContainer.parent,"NextDelivery");
        if(ui.openElementChangeButton!=null)ui.openElementChangeButton.gameObject.SetActive(false);
        if(ui.openTypeChangeButton!=null)ui.openTypeChangeButton.gameObject.SetActive(false);
        if(shop.freezeButton!=null)shop.freezeButton.gameObject.SetActive(false);
        if(ui.towerPreviewCamera!=null)ui.towerPreviewCamera.enabled=false;
        bags.cellSize=Cell;bags.defaultCellColor=Color.clear;
        bags.gridContainer.SetParent(root,false);
        Place(bags.gridContainer,66,414,Cell*5,Cell*5);
        bags.gridContainer.localScale=new Vector3(1,455f/(Cell*5),1);
        CreateStorageViewport();
        serif=GameFonts.Title;
        body=GameFonts.Body;
        ownsFonts=false;
        title=MakeValue("Title",root,81,30,750,57,45,cream,"TitleReference","Okçu Kulesi • Seviye 5",serif);
        stats[0]=MakeValue("Damage",root,341,85,131,42,35,gold,"DamageReference","55,76");
        stats[1]=MakeValue("Interval",root,341,133,132,42,35,gold,"IntervalReference","0,41 sn");
        stats[2]=MakeValue("Critical",root,341,180,132,42,35,gold,"CriticalReference","%74");
        stats[3]=MakeValue("Range",root,341,225,132,42,35,gold,"RangeReference","5 birim");
        for(int i=0;i<3;i++)details[i]=MakeValue("Breakdown"+i,root,476,88+i*47,385,37,23,cream);
        xp=MakeValue("XP",root,914,32,670,38,28,cream);
        status=MakeValue("ElementStatus",root,915,96,680,31,22,cream);
        offers=MakeValue("OfferCount",root,1152,368,460,32,23,cream);
        progress=NewRect("XPFill",root).gameObject.AddComponent<UnityEngine.UI.Image>();
        progress.sprite=ReferencePanelAssets.Sprite("ProgressFill");progress.raycastTarget=false;
        Place(progress.rectTransform,922,77,570,14);progress.type=UnityEngine.UI.Image.Type.Filled;
        progress.fillMethod=UnityEngine.UI.Image.FillMethod.Horizontal;
        string[] elements={"Fire","Water","Earth","Air"};
        Rect[] rects={new Rect(914,129,166,54),new Rect(1087,129,160,54),new Rect(1253,129,161,54),new Rect(1419,129,179,54)};
        for(int i=0;i<ui.elementButtons.Length;i++)BindButton(ui.elementButtons[i],elements[i],rects[i]);
        BindButton(ui.targetTypeButtons[0],"Both",new Rect(1044,188,184,47));
        BindButton(ui.targetTypeButtons[1],"Ground",new Rect(1230,188,167,47));
        BindButton(ui.targetTypeButtons[2],"Flying",new Rect(1401,188,183,47));
        BindButton(ui.targetOrderButtons[0],"First",new Rect(1043,236,266,49));
        BindButton(ui.targetOrderButtons[1],"Last",new Rect(1314,236,272,49));
        BindButton(shop.rerollButton,"Reroll",new Rect(1054,804,484,79));
        var scroll=shop.offersScroll;scroll.transform.SetParent(root,false);
        Place((RectTransform)scroll.transform,964,407,670,395);Stretch(scroll.viewport);
        var grid=scroll.content.GetComponent<UnityEngine.UI.GridLayoutGroup>();
        grid.constraint=UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount;grid.constraintCount=2;
        grid.cellSize=new Vector2(330,194);grid.spacing=new Vector2(10,6);grid.padding=new RectOffset();
        scroll.content.anchorMin=new Vector2(0,1);scroll.content.anchorMax=Vector2.one;
        scroll.content.pivot=new Vector2(0,1);scroll.content.sizeDelta=new Vector2(0,394);
        scroll.horizontal=false;scroll.vertical=true;
        foreach(var slot in shop.shopSlots)PrepareCard(slot);
        ApplyShopShelfLayout();
        ApplyInventoryRulesLayout();
        Resize();
    }
    public void ApplyInventoryRulesLayout() { ApplyPanelV2(); }
    void Start()
    {
        if(root==null)return;
        ApplyInventoryRulesLayout();
        ApplyShopShelfLayout();
    }
    public void ApplyShopShelfLayout()
    {
        if(shopShelf==null)
        {
            var texture=Resources.Load<Texture2D>("UI/TowerPanel/Reference/ShopShelf");
            if(texture==null)return;
            shopShelf=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),Vector2.one*.5f,100,0,SpriteMeshType.FullRect);
            shopShelf.name="Approved_ShopShelf";
        }
        foreach(var card in cards)StyleShelfCard(card);
    }
    void StyleShelfCard(Card card)
    {
        var face=card.slot.GetComponent<UnityEngine.UI.Image>();
        face.enabled=true;face.sprite=shopShelf;face.color=Color.white;face.type=UnityEngine.UI.Image.Type.Simple;face.raycastTarget=true;
        card.name.text.gameObject.SetActive(false);
        card.footprint.text.gameObject.SetActive(false);
        card.slot.footprint.gameObject.SetActive(false);
        Place(card.slot.image.rectTransform,90,2,150,115);
        card.slot.image.preserveAspect=true;
        Place(card.cost.text.rectTransform,10,133,310,36);
        card.cost.text.fontSize=34;card.cost.text.color=cream;card.cost.text.alignment=TextAlignmentOptions.Center;
    }
    void BindBulkButton()
    {
        var button=root.Find("MoveAllToStorage").GetComponent<UnityEngine.UI.Button>();
        button.onClick.RemoveListener(bags.MoveAllToStorage);button.onClick.AddListener(bags.MoveAllToStorage);
    }
    void CreateStorageViewport()
    {
        // Two visible columns match the source; horizontal scrolling retains every
        // existing storage cell and the placement rules for multi-cell equipment.
        var viewport=NewRect("ReferenceStorageViewport",root);Place(viewport,655,414,242,455);
        viewport.gameObject.AddComponent<UnityEngine.UI.RectMask2D>();
        var ray=viewport.gameObject.AddComponent<UnityEngine.UI.Image>();ray.color=Color.clear;ray.raycastTarget=true;
        storageScroll=viewport.gameObject.AddComponent<UnityEngine.UI.ScrollRect>();
        storageScroll.viewport=viewport;storageScroll.content=bags.storageGridContainer;
        storageScroll.horizontal=true;storageScroll.vertical=false;storageScroll.inertia=false;
        storageScroll.movementType=UnityEngine.UI.ScrollRect.MovementType.Clamped;storageScroll.scrollSensitivity=35;
        bags.storageGridContainer.SetParent(viewport,false);Place(bags.storageGridContainer,0,0,Cell*3,Cell*5);
        bags.storageGridContainer.localScale=new Vector3(121f/Cell,91f/Cell,1);
    }
    public void PrepareToOpen(Tower tower)
    {
        if(root!=null&&root.Find("MoveAllToStorage")!=null)BindBulkButton();
        if(ui!=null&&UİManager.TowerShopUI!=null)ui.ClosePurchasePanel();
        if(storageScroll!=null&&tower!=null)
        {
            if(tower.Storage.IsLooseStorage)bags.storageGridContainer.GetComponent<StorageGridUI>().Prepare(tower.Storage.Items.Count);
            else bags.storageGridContainer.sizeDelta=new Vector2(tower.Storage.Width*Cell,tower.Storage.Height*Cell);
            storageScroll.verticalNormalizedPosition=1;
        }
        Resize();
    }
    void PrepareCard(ShopSlotUI slot)
    {
        var ray=slot.GetComponent<UnityEngine.UI.Image>();
        if(ray!=null){ray.enabled=true;ray.sprite=null;ray.color=Color.clear;ray.raycastTarget=true;}
        foreach(var effect in slot.GetComponents<UnityEngine.UI.Shadow>())effect.enabled=false;
        slot.image.enabled=true;slot.image.color=Color.white;slot.image.preserveAspect=true;
        Place(slot.image.rectTransform,14,34,115,120);Place(slot.footprint,142,98,66,50);
        var card=new Card{slot=slot};
        card.name=MakeValue("ReferenceName",slot.transform,132,15,185,42,23,cream);
        card.name.text.enableAutoSizing=true;card.name.text.fontSizeMin=18;card.name.text.fontSizeMax=23;
        card.cost=MakeValue("ReferenceCost",slot.transform,132,61,175,37,29,gold);
        card.cost.text.alignment=TextAlignmentOptions.Center;
        card.footprint=MakeValue("ReferenceFootprint",slot.transform,140,151,182,28,19,cream);
        cards.Add(card);
        if(shopShelf!=null)StyleShelfCard(card);
    }
    void LateUpdate()
    {
        if(root==null)return;
        if(((RectTransform)root.parent).rect.size!=canvasSize)Resize();
        if(!root.gameObject.activeInHierarchy||bags.ActiveTower==null)return;
        var t=bags.ActiveTower;
        title.Set(ui.towerPanelTitle.text.Replace("  ·  "," • "));
        stats[0].Set(Number(t.currentDamage));stats[1].Set(Number(t.currentAttackSpeed)+" sn");
        stats[2].Set("%"+Number(100*t.currentCriticalChance));
        stats[3].Set(t is UnitTower?"%"+Number(100*t.currentHealthBonus):Number(t.CurrentRange)+" birim");
        details[0].Set(Breakdown(t.baseDamage,t.currentDamage));
        details[1].Set(Breakdown(t.baseAttackSpeed,t.currentAttackSpeed," sn"));
        details[2].Set(Breakdown(t.baseCriticalChance*100,t.currentCriticalChance*100));
        xp.Set(ui.towerExperienceText.text);status.text.gameObject.SetActive(false);offers.Set(shop.offerCountText.text);
        RefreshPanelV2(t);
        progress.fillAmount=Mathf.Clamp01(t.Experience/t.ExperienceRequired);
        for(int i=cards.Count;i<shop.shopSlots.Length;i++)PrepareCard(shop.shopSlots[i]);
        foreach(var card in cards)
        {
            var item=card.slot.currentItem;
            card.name.Set(item!=null?item.Name:"Alındı");card.cost.Set(item!=null?item.cost.ToString():"");
            card.footprint.Set(card.slot.footprintText.text);
            if(item!=null)
            {
                var sprite=ReferencePanelAssets.IconFor(item);card.slot.image.sprite=sprite;
                Place(card.slot.image.rectTransform,90,2,150,115);
            }
        }
    }
    void Resize()
    {
        if(root==null)return;canvasSize=((RectTransform)root.parent).rect.size;
        root.anchorMin=root.anchorMax=root.pivot=Vector2.one*.5f;root.anchoredPosition=Vector2.zero;
        float scale=Mathf.Min(1,Mathf.Min(canvasSize.x/1920f,canvasSize.y/1080f));
        root.sizeDelta=new Vector2(Width,Height);root.localScale=new Vector3(1600f/Width*scale,930f/Height*scale,1);
        if(ui.transitionPanel!=null)Stretch((RectTransform)ui.transitionPanel.transform);
    }
    void BindButton(UnityEngine.UI.Button button,string resource,Rect rect)
    {
        button.transform.SetParent(root,false);Place((RectTransform)button.transform,rect.x,rect.y,rect.width,rect.height);
        var image=button.GetComponent<UnityEngine.UI.Image>();image.sprite=ReferencePanelAssets.Sprite(resource);
        image.enabled=true;image.color=Color.white;image.type=UnityEngine.UI.Image.Type.Simple;
        image.raycastTarget=true;button.targetGraphic=image;
        var colors=button.colors;colors.normalColor=colors.disabledColor=colors.selectedColor=Color.white;
        colors.highlightedColor=new Color(1.06f,1.06f,1.06f);colors.pressedColor=new Color(.85f,.85f,.85f);button.colors=colors;
    }
    Value MakeValue(string name,Transform parent,float x,float y,float w,float h,int size,Color color,
        string resource=null,string match=null,TMP_FontAsset font=null)
    {
        var r=NewRect(name,parent);Place(r,x,y,w,h);var text=r.gameObject.AddComponent<TextMeshProUGUI>();
        if(font!=null)text.font=font;
        else if(body!=null)text.font=body;
        text.fontSize=size;text.fontStyle=font!=null||size<=28?FontStyles.Normal:FontStyles.Bold;text.color=color;text.enableWordWrapping=false;
        text.raycastTarget=false;text.alignment=TextAlignmentOptions.MidlineLeft;
        var shadow=r.gameObject.AddComponent<UnityEngine.UI.Shadow>();
        shadow.effectColor=Color.black;shadow.effectDistance=new Vector2(1.4f,-1.4f);
        var value=new Value{text=text,match=match};
        if(resource!=null)
        {
            var exact=NewRect(name+"Source",parent);value.original=exact.gameObject.AddComponent<UnityEngine.UI.Image>();
            value.original.sprite=ReferencePanelAssets.Sprite(resource);value.original.raycastTarget=false;
            var native=value.original.sprite.rect;Place(exact,x,y,native.width,native.height);
        }
        return value;
    }
    static string Number(float value)=>value.ToString("0.##",System.Globalization.CultureInfo.GetCultureInfo("tr-TR"));
    static string Breakdown(float basic,float current,string suffix="")=>
        "(Temel "+Number(basic)+" + Eşya "+Number(current-basic)+suffix+")";
    static RectTransform NewRect(string name,Transform parent)
    {var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);return r;}
    static void Hide(Transform parent,string name)
    {var child=parent.Find(name);if(child!=null)child.gameObject.SetActive(false);}
    static void Place(RectTransform r,float x,float y,float w,float h)
    {r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.localScale=Vector3.one;r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);}
    static void Stretch(RectTransform r)
    {r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;}
    void OnDestroy()
    {
        if(!ownsFonts)return;
        ReleaseFont(serif);ReleaseFont(body);
    }
    static void ReleaseFont(TMP_FontAsset font)
    {
        if(font==null)return;
#if UNITY_EDITOR
        if(UnityEditor.EditorUtility.IsPersistent(font))return;
#endif
        if(Application.isPlaying)Destroy(font);else DestroyImmediate(font);
    }
}
