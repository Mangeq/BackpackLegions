using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Approved result and pause artwork, with authoritative game callbacks.</summary>
public sealed class ResultReferenceSkin : MonoBehaviour
{
    [SerializeField] GameManager game;
    [SerializeField] RectTransform pause,victory,defeat;
    [SerializeField] TMP_Text chapterLabel,nextCaption;
    [SerializeField] Sprite nextNormal,nextComplete;
    public static Sprite Art(string name)=>Resources.Load<Sprite>("UI/ResultReference/"+name);
    public void Configure(GameManager owner)
    {
        game=owner;pause=(RectTransform)game.pausePanel.transform;victory=(RectTransform)game.victoryPanel.transform;defeat=(RectTransform)game.GameOverPanel.transform;
        Prepare(pause,"Pause");Prepare(victory,"Victory");Prepare(defeat,"Defeat");
        PlaceButton(pause,"CountineButton","PauseContinue",new Rect(344,197,347,77));
        PlaceButton(pause,"SkillTreeButton","PauseSkills",new Rect(344,284,347,78));
        PlaceButton(pause,"SettingsButton","PauseSettings",new Rect(344,371,347,77));
        PlaceButton(pause,"MainMenuButton","PauseMenu",new Rect(344,458,347,76));
        PlaceButton(victory,"NextChapter","VictoryNext",new Rect(348,323,337,69));
        PlaceButton(victory,"SkillTree","VictorySkills",new Rect(348,397,337,69));
        PlaceButton(victory,"Retry","VictoryRetry",new Rect(289,474,219,53));
        PlaceButton(victory,"MainMenu","VictoryMenu",new Rect(523,474,219,53));
        PlaceButton(defeat,"NewGameButton","DefeatRetry",new Rect(380,321,272,63));
        PlaceButton(defeat,"SkillTree","DefeatSkills",new Rect(380,386,272,62));
        PlaceButton(defeat,"BackMainMenuButton","DefeatMenu",new Rect(390,452,252,50));
        nextNormal=Art("VictoryNextCutout");nextComplete=Art("VictoryNextComplete");
        nextCaption=game.nextChapterButton.GetComponentInChildren<TMP_Text>(true);
        TextStyle(nextCaption,game.nextChapterButton.transform,8,8,321,51,21);nextCaption.alignment=TextAlignmentOptions.Center;
        var label=defeat.Find("LiveChapter") as RectTransform;
        if(label==null){label=new GameObject("LiveChapter",typeof(RectTransform),typeof(TextMeshProUGUI)).GetComponent<RectTransform>();label.SetParent(defeat,false);}
        chapterLabel=label.GetComponent<TMP_Text>();TextStyle(chapterLabel,defeat,377,264,327,33,20);chapterLabel.fontStyle=FontStyles.Bold;chapterLabel.alignment=TextAlignmentOptions.Center;
        RefreshPresentation();
    }
    static void Prepare(RectTransform root,string source)
    {
        foreach(var graphic in root.GetComponentsInChildren<Graphic>(true))graphic.enabled=false;
        foreach(var effect in root.GetComponentsInChildren<Shadow>(true))effect.enabled=false;
        var face=root.GetComponent<Image>();if(face==null)face=root.gameObject.AddComponent<Image>();
        face.sprite=Art(source+"Cutout");face.type=Image.Type.Simple;face.enabled=true;face.raycastTarget=true;face.color=Color.white;
        root.anchorMin=root.anchorMax=root.pivot=Vector2.one*.5f;root.anchoredPosition=Vector2.zero;root.sizeDelta=new Vector2(1024,572);Fit(root);
    }
    static void Fit(RectTransform root)
    {
        var area=((RectTransform)root.parent).rect;
        // Preserve the artwork proportions and leave the world visible around the modal.
        float scale=Mathf.Min(area.width*.9f/1024,area.height*.8f/572);
        root.localScale=new Vector3(scale,scale,1);
        root.anchoredPosition=new Vector2(-5,(root.name=="PausePanel"?20:-7))*scale;
    }
    static void PlaceButton(RectTransform root,string name,string art,Rect bounds)
    {
        var button=System.Array.Find(root.GetComponentsInChildren<Button>(true),b=>b.name==name);
        if(button==null)throw new System.InvalidOperationException("Missing existing button: "+name);
        Place((RectTransform)button.transform,root,bounds.x,bounds.y,bounds.width,bounds.height);
        var image=button.GetComponent<Image>();image.sprite=Art(art+"Cutout");image.type=Image.Type.Simple;image.color=Color.white;image.enabled=true;image.raycastTarget=true;button.targetGraphic=image;
        var colors=ColorBlock.defaultColorBlock;colors.highlightedColor=new Color(1.08f,1.08f,1.08f);colors.selectedColor=Color.white;colors.disabledColor=new Color(.7f,.7f,.7f);button.colors=colors;
    }
    static void TextStyle(TMP_Text text,Transform parent,float x,float y,float w,float h,int size)
    {
        Place(text.rectTransform,parent,x,y,w,h);text.font=Resources.Load<TMP_FontAsset>("UI/ApprovedScene/Approved_BodyFont");text.enabled=true;text.color=new Color(1,.91f,.75f);text.fontSize=size;text.enableAutoSizing=true;text.fontSizeMin=13;text.fontSizeMax=size;text.enableWordWrapping=false;text.raycastTarget=false;
    }
    static void Place(RectTransform r,Transform parent,float x,float y,float w,float h)
    {r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.localScale=Vector3.one;r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);}
    public void RefreshPresentation()
    {
        if(game==null)return;
        foreach(var panel in new[]{pause,victory,defeat})if(panel!=null)Fit(panel);
        if(chapterLabel!=null)
        {
            var waves=FindObjectOfType<WaveManager>();var chapter=waves!=null?waves.ActiveChapter:null;
            if(chapter==null&&waves!=null&&waves.chapters.Count>0)chapter=waves.chapters[Mathf.Clamp(ChapterData.SelectedIndex,0,waves.chapters.Count-1)];
            chapterLabel.text=chapter!=null?"BÖLÜM "+chapter.chapterNumber.ToString("00")+" · "+chapter.chapterName:"SAVUNMA RAPORU";
        }
        if(nextCaption!=null)
        {
            bool complete=!game.nextChapterButton.interactable;
            nextCaption.enabled=complete;
            if(complete)nextCaption.text="TÜM BÖLÜMLER TAMAMLANDI";
            game.nextChapterButton.GetComponent<Image>().sprite=complete?nextComplete:nextNormal;
        }
    }
    void LateUpdate(){if(pause!=null&&(pause.gameObject.activeSelf||victory.gameObject.activeSelf||defeat.gameObject.activeSelf))RefreshPresentation();}
}
