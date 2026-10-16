using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Events;
using TMPro;
using System.Linq;

public static class ResultsAndSkillsBuilder
{
    static readonly Color Ink=new Color(.055f,.085f,.075f),Panel=new Color(.09f,.14f,.12f),Gold=new Color(.73f,.58f,.34f),Cream=new Color(.94f,.91f,.82f),Muted=new Color(.64f,.71f,.66f),Rust=new Color(.60f,.28f,.22f);
    static Sprite rounded;
    static TMP_FontAsset font;
    static RectTransform Rect(string name,Transform parent,float x,float y,float w,float h)
    {
        var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;
    }
    static void Stretch(RectTransform r){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;}
    static void Center(RectTransform r,float w,float h){r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=Vector2.zero;r.sizeDelta=new Vector2(w,h);}
    static UnityEngine.UI.Image Image(string name,Transform parent,float x,float y,float w,float h,Color color,bool ray=false)
    {var r=Rect(name,parent,x,y,w,h);var i=r.gameObject.AddComponent<UnityEngine.UI.Image>();i.sprite=rounded;i.type=UnityEngine.UI.Image.Type.Sliced;i.color=color;i.raycastTarget=ray;return i;}
    static TMP_Text Text(string name,Transform parent,string value,float x,float y,float w,float h,float size,Color color,bool bold=false)
    {var r=Rect(name,parent,x,y,w,h);var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.font=font;t.text=value;t.fontSize=size;t.fontStyle=bold?FontStyles.Bold:FontStyles.Normal;t.color=color;t.raycastTarget=false;return t;}
    static UnityEngine.UI.Button Button(string name,Transform parent,string value,float x,float y,float w,float h,UnityAction action,Color color)
    {
        var i=Image(name,parent,x,y,w,h,color,true);var b=i.gameObject.AddComponent<UnityEngine.UI.Button>();b.targetGraphic=i;
        var cb=UnityEngine.UI.ColorBlock.defaultColorBlock;cb.highlightedColor=new Color(.87f,.93f,.88f);cb.pressedColor=new Color(.67f,.74f,.69f);cb.disabledColor=new Color(.5f,.5f,.5f,.6f);b.colors=cb;
        if(value.Length>0){var t=Text("Label",i.transform,value,12,0,w-24,h,22,color==Gold?Ink:Cream,true);t.alignment=TextAlignmentOptions.Center;}
        if(action!=null)UnityEditor.Events.UnityEventTools.AddPersistentListener(b.onClick,action);return b;
    }
    static GameObject Canvas(string name,int order)
    {var go=new GameObject(name,typeof(RectTransform),typeof(Canvas),typeof(UnityEngine.UI.CanvasScaler),typeof(UnityEngine.UI.GraphicRaycaster));go.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;go.GetComponent<Canvas>().sortingOrder=order;var s=go.GetComponent<UnityEngine.UI.CanvasScaler>();s.uiScaleMode=UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;s.referenceResolution=new Vector2(1920,1080);s.screenMatchMode=UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;return go;}
    static void Line(Transform parent,float x1,float y1,float x2,float y2)
    {var i=Image("BranchLink",parent,x1,y1,Vector2.Distance(new Vector2(x1,y1),new Vector2(x2,y2)),2,new Color(.29f,.36f,.30f));i.rectTransform.pivot=new Vector2(0,.5f);i.rectTransform.localRotation=Quaternion.Euler(0,0,-Mathf.Atan2(y2-y1,x2-x1)*Mathf.Rad2Deg);}
    static void Field(Object target,string name,Object value){var so=new SerializedObject(target);so.FindProperty(name).objectReferenceValue=value;so.ApplyModifiedPropertiesWithoutUndo();}
    static void PlaceExisting(RectTransform r,Transform parent,float x,float y,float w,float h){r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);}
    static void StyleExisting(UnityEngine.UI.Button b,string label,Color color)
    {var i=b.GetComponent<UnityEngine.UI.Image>();i.sprite=rounded;i.type=UnityEngine.UI.Image.Type.Sliced;i.color=color;b.colors=UnityEngine.UI.ColorBlock.defaultColorBlock;var t=b.GetComponentInChildren<TMP_Text>(true);t.font=font;t.text=label;t.fontSize=22;t.color=Cream;t.alignment=TextAlignmentOptions.Center;Stretch(t.rectTransform);}
    static GameObject SkillPrefab()
    {
        var root=Canvas("SkillTreeUI",220);var ui=root.AddComponent<SkillTreePanelUI>();
        var modal=Rect("Modal",root.transform,0,0,1920,1080);Stretch(modal);ui.modal=modal.gameObject;
        var shade=Button("Backdrop",modal,"",0,0,1920,1080,ui.Close,new Color(0,0,0,.82f));Stretch(shade.GetComponent<RectTransform>());
        var card=Image("Card",modal,0,0,1600,900,Ink,true);Center(card.rectTransform,1600,900);
        Image("TopAccent",card.transform,32,26,64,3,Gold);
        Text("Eyebrow",card.transform,"LEJYON AKADEMİSİ",32,48,900,28,16,Gold,true);
        Text("Title",card.transform,"KULE UZMANLIKLARI",32,86,1300,62,42,Cream,true);
        Button("Close",card.transform,"KAPAT",1410,42,154,52,ui.Close,new Color(.16f,.22f,.19f));
        Text("PreviewNotice",card.transform,"TASARIM ÖNİZLEMESİ · Henüz yetenek açılmaz veya buff uygulanmaz.",32,846,1500,30,17,Muted);
        Image("Sidebar",card.transform,24,168,244,650,Panel);
        string[] types={"OKÇU","BÜYÜCÜ","HAVAN","BİRLİK"};
        ui.tabs=new UnityEngine.UI.Image[4];
        for(int i=0;i<4;i++){var b=Button("Type"+i,card.transform,types[i],40,196+i*115,212,88,null,Panel);UnityEditor.Events.UnityEventTools.AddIntPersistentListener(b.onClick,ui.SelectBranch,i);ui.tabs[i]=b.GetComponent<UnityEngine.UI.Image>();}
        Text("SharedHint",card.transform,"Her türün kendi ortak yetenek ağacı.",44,694,200,88,20,Muted);
        ui.branchTitle=Text("BranchTitle",card.transform,"",300,180,800,42,30,Cream,true);
        ui.branchSubtitle=Text("BranchSubtitle",card.transform,"",300,228,800,52,19,Muted);
        float[] x={698,488,908,488,908,698},y={330,480,480,650,650,780};
        int[,] edges={{0,1},{0,2},{1,3},{2,4},{3,5},{4,5}};
        for(int i=0;i<6;i++)Line(card.transform,x[edges[i,0]],y[edges[i,0]],x[edges[i,1]],y[edges[i,1]]);
        ui.nodes=new UnityEngine.UI.Image[6];ui.nodeIcons=new UnityEngine.UI.Image[6];ui.nodeLabels=new TMP_Text[6];
        for(int i=0;i<6;i++)
        {
            var b=Button("Node"+i,card.transform,"",x[i]-92,y[i]-45,184,90,null,Panel);UnityEditor.Events.UnityEventTools.AddIntPersistentListener(b.onClick,ui.SelectNode,i);ui.nodes[i]=b.GetComponent<UnityEngine.UI.Image>();
            ui.nodeIcons[i]=Image("Icon",b.transform,10,17,52,52,Color.white);ui.nodeIcons[i].type=UnityEngine.UI.Image.Type.Simple;ui.nodeIcons[i].preserveAspect=true;
            ui.nodeLabels[i]=Text("Name",b.transform,"",72,10,102,72,17,Cream,true);ui.nodeLabels[i].alignment=TextAlignmentOptions.MidlineLeft;
        }
        var detail=Image("Details",card.transform,1142,168,426,650,Panel,true);
        ui.detailIndex=Text("Index",detail.transform,"",28,28,370,28,16,Gold,true);
        ui.detailIcon=Image("Icon",detail.transform,28,84,96,96,Color.white);ui.detailIcon.type=UnityEngine.UI.Image.Type.Simple;ui.detailIcon.preserveAspect=true;
        Text("DetailEyebrow",detail.transform,"YETENEK DOSYASI",145,110,245,52,17,Muted,true);
        ui.detailTitle=Text("Title",detail.transform,"",28,218,370,108,32,Cream,true);
        Image("Rule",detail.transform,28,336,370,1,new Color(.28f,.35f,.29f));
        ui.detailBody=Text("Description",detail.transform,"",28,366,370,142,22,Cream);
        var unavailable=Button("PreviewOnly",detail.transform,"HENÜZ ETKİN DEĞİL",28,540,370,62,null,new Color(.2f,.26f,.22f));unavailable.interactable=false;
        Text("RulesNote",detail.transform,"Maliyet ve açılma koşulları belirlenecek.",28,610,370,28,15,Muted);
        string[][] names={new[]{"Okçu Eğitimi I","Hafif Yaylar","Ağır Kiriş","Kritik Odak","Element Okları","Kusursuz Nişancılık"},new[]{"Mana Akışı I","Rün Bilgisi","Katalizör","Kaotik Element","Yıkım Büyüsü","Kıyamet Sevdalısı"},new[]{"Barut Eğitimi","Seri Yükleme","Geniş Patlama","Ağır Mühimmat","Element Barutu","Kuşatma Ustası"},new[]{"Ordu Düzeni I","Demir Dövme","Kışla İkmali","Element Zırhı","Çifte Sancak","Lejyoner Sinerjisi"}};
        string[][] descriptions={new[]{"Okçuların temel hasarını artırır. Belgedeki öneri: +%5.","Atış hızına odaklanır. Belgedeki öneri: +%8.","Menzil ve zırh delme uzmanlığı. Zırh kuralları uygulama öncesi netleşecek.","Kritik vuruş şansını ve etkisini geliştirir. Nihai değerler dengeyle birlikte seçilecek.","Elementli hedeflere karşı uzmanlaşma. Mevcut dört element döngüsüne uyarlanacak.","Aynı hedefte ardışık isabetlerden sonra güçlü bir vuruş. Sayaç ve sersemletme kuralları belirlenecek."},new[]{"Büyü eşyalarının mağaza tekliflerinde görünme şansına odaklanır.","Çantadaki büyü eşyalarını saldırı hızına katkıya dönüştürür.","Element değiştirme maliyetini azaltmaya odaklanır.","Doğru element eşleşmesinin hasar avantajını geliştirir.","Alan hasarı ve büyü kritiklerine odaklanır. Mevcut tek hedef saldırısına etkisi belirlenecek.","Element değiştirdikten sonra geçici güç sağlar. Etkin olduğu dalga ve süre birlikte seçilecek."},new[]{"Havan hasarını geliştiren başlangıç düğümü. Havan ağacı için yeni tasarım önerisi.","Atışlar arasındaki yükleme süresini iyileştirir. Öneri; henüz uygulanmadı.","Patlamanın etkili olduğu alanı genişletir. Menzil ile patlama alanı ayrı tutulur.","Dayanıklı tek hedeflere karşı uzmanlaşma önerisi.","Elementli havan kurulumlarının gücünü geliştirir. Sayısal değerler belirlenecek.","Kalabalık dalgalara karşı üst kademe uzmanlık önerisi. Nihai davranış birlikte tasarlanacak."},new[]{"Birliklerin dayanıklılığını artırır. Belgedeki öneri: +%15 can.","Silah eşyalarının birlik hasarına katkısını güçlendirir.","Kaybedilen birliklerin yeniden doğmasını hızlandırır.","Elemente bağlı savunma ve saldırı uzmanlığı. Savunma hesabı ayrıca belirlenecek.","Ek birlik üretimi ihtimali. Kapasite ve geçici birlik kuralları henüz belirlenmedi.","Aynı kuleye ait birliklerin beraber savaşmasını ödüllendiren uzmanlık."}};
        TowerType[] owners={TowerType.ArrowTower,TowerType.MagicTower,TowerType.CannonTower,TowerType.UnitTower};
        Color[] accents={new Color(.25f,.39f,.29f),new Color(.34f,.27f,.43f),new Color(.43f,.29f,.19f),new Color(.22f,.35f,.42f)};
        string[] subtitles={"İsabet · Menzil · Kritik","Element · Büyü eşyaları · Uzmanlık","Alan hasarı · Yükleme · Kuşatma  /  ÖNERİ","Dayanıklılık · İkmal · Birlik uyumu"};
        string[][] iconFiles={
            new[]{"v1_shortbow","v1_archer_gloves","v1_warbow","v1_focus_ring","v1_moonbow","v1_hawk_pendant"},
            new[]{"v1_mana_elixir","v1_rune_scroll","v1_flow_amulet","v1_void_seal","v1_elder_staff","v1_archmage_robe"},
            new[]{"mortar_barut_kesesi_normal","mortar_seri_doldurma_takimi_normal","mortar_patlayici_gulle_normal","mortar_agir_gulle_normal","mortar_hedef_tilsimi_normal","mortar_kusatma_muhimmati_normal"},
            new[]{"v1_leather_armor","v1_legion_sword","v1_march_boots","v1_fortress_shield","v1_captain_armor","v1_legion_crown"}};
        ui.branches=new SkillTreePanelUI.Branch[4];
        for(int i=0;i<4;i++)
        {var icons=iconFiles[i].Select(f=>AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Resources/Data/Items/Shop/Normal/"+f+".asset").itemIcon).ToArray();if(icons.Length!=6)throw new System.Exception("Missing skill preview icons");ui.branches[i]=new SkillTreePanelUI.Branch{title=types[i]+" AĞACI",subtitle=subtitles[i]+" · Bağlantılar taslak",accent=accents[i],names=names[i],descriptions=descriptions[i],icons=icons};}
        ui.SelectBranch(0);modal.gameObject.SetActive(false);
        var prefab=PrefabUtility.SaveAsPrefabAsset(root,"Assets/Prefabs/UI/SkillTreeUI.prefab");Object.DestroyImmediate(root);return prefab;
    }
    static Transform ResultCard(GameObject root,Color accent)
    {
        Stretch(root.GetComponent<RectTransform>());var shade=root.GetComponent<UnityEngine.UI.Image>();if(shade==null)shade=root.AddComponent<UnityEngine.UI.Image>();shade.sprite=null;shade.color=new Color(0,0,0,.80f);shade.raycastTarget=true;
        var card=Image("Card",root.transform,0,0,1000,660,Panel,true);Center(card.rectTransform,1000,660);card.transform.SetAsFirstSibling();
        Image("Accent",card.transform,48,40,72,4,accent);Text("Eyebrow",card.transform,"SEFER RAPORU",48,66,700,32,18,accent,true);
        Image("Rule",card.transform,48,224,904,1,new Color(.28f,.35f,.29f));return card.transform;
    }
    public static string Build()
    {
        if(EditorApplication.isPlaying||UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)throw new System.Exception("Stop Play and save scene first.");
        if(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/SkillTreeUI.prefab")!=null)throw new System.Exception("Already built; edit existing objects.");
        rounded=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Frontend/RoundedPanel.png");font=TMP_Settings.defaultFontAsset;
        var prefab=SkillPrefab();
        var game=EditorSceneManager.OpenScene("Assets/Scenes/GameScane.unity");var gm=Object.FindObjectOfType<GameManager>();
        var canvas=Canvas("ResultsCanvas",180);var old=gm.GameOverPanel;old.transform.SetParent(canvas.transform,false);
        var title=old.transform.Find("Text (TMP)").GetComponent<TMP_Text>();var retry=old.transform.Find("NewGameButton").GetComponent<UnityEngine.UI.Button>();var menu=old.transform.Find("BackMainMenuButton").GetComponent<UnityEngine.UI.Button>();
        var defeat=ResultCard(old,Rust);PlaceExisting(title.rectTransform,defeat,48,116,904,82);title.font=font;title.fontSize=58;title.color=Cream;title.text="KULE DÜŞTÜ";title.alignment=TextAlignmentOptions.MidlineLeft;
        Text("Subtitle",defeat,"Bu savunma sona erdi. Yeni bir planla tekrar dene.",48,194,904,28,20,Muted);
        gm.defeatSummary=Text("Summary",defeat,"",48,268,904,150,26,Cream);
        PlaceExisting(retry.GetComponent<RectTransform>(),defeat,48,470,904,76);StyleExisting(retry,"TEKRAR DENE",Rust);
        PlaceExisting(menu.GetComponent<RectTransform>(),defeat,48,566,904,54);StyleExisting(menu,"ANA MENÜ",new Color(.16f,.22f,.19f));old.SetActive(false);
        var victory=Rect("VictoryPanel",canvas.transform,0,0,1920,1080).gameObject;var success=ResultCard(victory,Gold);gm.victoryPanel=victory;
        Text("Title",success,"BÖLÜM GEÇİLDİ",48,116,904,78,58,Cream,true);Text("Subtitle",success,"Geçit korundu. Bir sonraki savunmaya hazırsın.",48,194,904,28,20,Muted);
        gm.victorySummary=Text("Summary",success,"",48,268,904,150,26,Cream);
        gm.nextChapterButton=Button("NextChapter",success,"SONRAKİ BÖLÜM  ›",48,470,904,76,gm.NextChapter,Gold);
        Button("Retry",success,"TEKRAR OYNA",48,566,440,54,gm.RestartGame,new Color(.16f,.22f,.19f));Button("MainMenu",success,"ANA MENÜ",512,566,440,54,gm.ExitToMainMenu,new Color(.16f,.22f,.19f));victory.SetActive(false);
        var skill=(GameObject)PrefabUtility.InstantiatePrefab(prefab,game);gm.skillTreePanel=skill.GetComponent<SkillTreePanelUI>();
        var pause=gm.pausePanel.GetComponent<RectTransform>();pause.sizeDelta=new Vector2(480,500);
        foreach(var t in gm.pausePanel.GetComponentsInChildren<TMP_Text>(true).Where(t=>t.transform.parent==gm.pausePanel.transform)){Center(t.rectTransform,400,60);t.rectTransform.anchoredPosition=new Vector2(0,195);}
        string[] existing={"CountineButton","SettingsButton","MainMenuButton"};float[] yy={95,-65,-145};for(int i=0;i<3;i++){var r=gm.pausePanel.transform.Find(existing[i]).GetComponent<RectTransform>();Center(r,360,60);r.anchoredPosition=new Vector2(0,yy[i]);}
        var skillButton=Button("SkillTreeButton",gm.pausePanel.transform,"YETENEK AĞAÇLARI",60,205,360,60,gm.OpenSkillTree,new Color(.16f,.22f,.19f));
        EditorUtility.SetDirty(gm);EditorSceneManager.MarkSceneDirty(game);EditorSceneManager.SaveScene(game);
        var main=EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");var manager=Object.FindObjectOfType<MainMenuManager>();var mainSkill=(GameObject)PrefabUtility.InstantiatePrefab(prefab,main);Field(manager,"skillTreePanel",mainSkill.GetComponent<SkillTreePanelUI>());
        var home=GameObject.Find("FrontendCanvas").transform.Find("SafeFrame/HomePanel");if(home==null)throw new System.Exception("Home panel missing");
        string[] buttons={"Play","Settings","Quit"};float[] ypos={650,818,900};for(int i=0;i<3;i++){var r=home.Find(buttons[i]).GetComponent<RectTransform>();r.anchoredPosition=new Vector2(116,-ypos[i]);r.sizeDelta=new Vector2(464,66);}
        Button("SkillTree",home,"YETENEK AĞAÇLARI",116,734,464,66,manager.OpenSkillTree,new Color(.14f,.20f,.17f));
        EditorSceneManager.MarkSceneDirty(main);EditorSceneManager.SaveScene(main);AssetDatabase.SaveAssets();return "Separate result panels, next chapter flow, four preview skill trees installed.";
    }
}
