using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;
using System.Linq;

// Editor-only construction tool. Gameplay remains in the three approved UI classes.
public static class FrontendBuilder
{
    static readonly Color Ink=new Color(.065f,.10f,.09f), Panel=new Color(.09f,.14f,.12f), Brass=new Color(.73f,.58f,.34f), Cream=new Color(.94f,.91f,.82f), Muted=new Color(.63f,.70f,.64f);
    static Sprite rounded;
    static TMP_FontAsset font;
    const string Art="Assets/Art/Frontend/";
    static void Folder(string path) { if(AssetDatabase.IsValidFolder(path))return;var p=System.IO.Path.GetDirectoryName(path).Replace('\\','/');Folder(p);AssetDatabase.CreateFolder(p,System.IO.Path.GetFileName(path)); }
    static RectTransform Rect(string name,Transform parent,float x,float y,float w,float h)
    {
        var go=new GameObject(name,typeof(RectTransform));var r=go.GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;
    }
    static void Stretch(RectTransform r){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;}
    static RectTransform Frame(Transform parent){var r=Rect("SafeFrame",parent,0,0,1920,1080);r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=Vector2.zero;return r;}
    static UnityEngine.UI.Image Image(string name,Transform p,float x,float y,float w,float h,Color color,bool ray=false,bool round=false)
    {var r=Rect(name,p,x,y,w,h);var i=r.gameObject.AddComponent<UnityEngine.UI.Image>();i.color=color;i.raycastTarget=ray;if(round){i.sprite=rounded;i.type=UnityEngine.UI.Image.Type.Sliced;}return i;}
    static TMP_Text Text(string name,Transform p,string text,float x,float y,float w,float h,float size,Color color,bool bold=false)
    {var r=Rect(name,p,x,y,w,h);var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.font=font;t.text=text;t.fontSize=size;t.color=color;t.fontStyle=bold?FontStyles.Bold:FontStyles.Normal;t.raycastTarget=false;t.alignment=TextAlignmentOptions.MidlineLeft;t.margin=Vector4.zero;return t;}
    static UnityEngine.UI.Button Button(string name,Transform p,string label,float x,float y,float w,float h,UnityAction action,bool primary=false)
    {
        var i=Image(name,p,x,y,w,h,primary?Brass:new Color(.14f,.20f,.17f),true,true);var b=i.gameObject.AddComponent<UnityEngine.UI.Button>();b.targetGraphic=i;
        var colors=b.colors;colors.highlightedColor=new Color(1.16f,1.16f,1.10f);colors.pressedColor=new Color(.72f,.78f,.72f);colors.selectedColor=new Color(1.08f,1.08f,1.02f);colors.disabledColor=new Color(.5f,.5f,.5f,.65f);b.colors=colors;
        float pad=w<100?4:20;var t=Text("Label",i.transform,label,pad,0,w-pad*2,h,23,primary?Ink:Cream,true);t.alignment=TextAlignmentOptions.Center;t.enableWordWrapping=false;
        if(action!=null)UnityEditor.Events.UnityEventTools.AddPersistentListener(b.onClick,action);return b;
    }
    static UnityEngine.UI.Slider Slider(Transform p,string name,float x,float y,float w,float h)
    {
        var r=Rect(name,p,x,y,w,h);var s=r.gameObject.AddComponent<UnityEngine.UI.Slider>();var bg=Image("Track",r,0,h/2-3,w,6,new Color(.22f,.28f,.24f),true,true);
        var fillArea=Rect("FillArea",r,0,h/2-3,w,6);var fill=Image("Fill",fillArea,0,0,w,6,Brass,false,true);fill.rectTransform.sizeDelta=Vector2.zero;fill.rectTransform.anchoredPosition=Vector2.zero;s.fillRect=fill.rectTransform;
        var handleArea=Rect("HandleArea",r,9,0,w-18,h);var handle=Image("Handle",handleArea,0,0,18,h, Cream,true,true);handle.rectTransform.sizeDelta=new Vector2(18,0);handle.rectTransform.anchoredPosition=Vector2.zero;s.handleRect=handle.rectTransform;s.targetGraphic=handle;s.minValue=0;s.maxValue=1;s.value=1;return s;
    }
    static UnityEngine.UI.Toggle Toggle(Transform p,string name,float x,float y)
    {var i=Image(name,p,x,y,36,36,new Color(.22f,.28f,.24f),true,true);var t=i.gameObject.AddComponent<UnityEngine.UI.Toggle>();var mark=Image("Mark",i.transform,7,7,22,22,Brass,false,true);t.targetGraphic=i;t.graphic=mark;return t;}
    static GameObject Canvas(string name,int order)
    {var go=new GameObject(name,typeof(RectTransform),typeof(Canvas),typeof(UnityEngine.UI.CanvasScaler),typeof(UnityEngine.UI.GraphicRaycaster));go.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;go.GetComponent<Canvas>().sortingOrder=order;var s=go.GetComponent<UnityEngine.UI.CanvasScaler>();s.uiScaleMode=UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;s.referenceResolution=new Vector2(1920,1080);s.screenMatchMode=UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;return go;}
    static void Field(Object target,string name,Object value){var so=new SerializedObject(target);so.FindProperty(name).objectReferenceValue=value;so.ApplyModifiedPropertiesWithoutUndo();}
    static void StringField(Object target,string name,string value){var so=new SerializedObject(target);so.FindProperty(name).stringValue=value;so.ApplyModifiedPropertiesWithoutUndo();}
    static void Picture(string name,Transform p,string file,float x,float y,float w,float h,Color color)
    {var r=Rect(name,p,x,y,w,h);var raw=r.gameObject.AddComponent<UnityEngine.UI.RawImage>();raw.texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Art+file);raw.color=color;raw.raycastTarget=false;}
    static void PrepareArt()
    {
        Folder(Art.TrimEnd('/'));Folder("Assets/Resources/UI");Folder("Assets/Prefabs/UI");
        var tex=new Texture2D(64,64,TextureFormat.RGBA32,false);for(int y=0;y<64;y++)for(int x=0;x<64;x++){float dx=Mathf.Max(8-x,x-55),dy=Mathf.Max(8-y,y-55);float d=Mathf.Sqrt(Mathf.Max(0,dx)*Mathf.Max(0,dx)+Mathf.Max(0,dy)*Mathf.Max(0,dy));tex.SetPixel(x,y,new Color(1,1,1,Mathf.Clamp01(8-d)));}tex.Apply();System.IO.File.WriteAllBytes(Art+"RoundedPanel.png",tex.EncodeToPNG());Object.DestroyImmediate(tex);AssetDatabase.ImportAsset(Art+"RoundedPanel.png");var ti=(TextureImporter)AssetImporter.GetAtPath(Art+"RoundedPanel.png");ti.textureType=TextureImporterType.Sprite;ti.spriteBorder=new Vector4(10,10,10,10);ti.mipmapEnabled=false;ti.SaveAndReimport();rounded=AssetDatabase.LoadAssetAtPath<Sprite>(Art+"RoundedPanel.png");
        foreach(var file in new[]{"MenuFortress.png","CampaignMap.png"}){AssetDatabase.ImportAsset(Art+file);var imp=(TextureImporter)AssetImporter.GetAtPath(Art+file);imp.mipmapEnabled=false;imp.maxTextureSize=2048;imp.textureCompression=TextureImporterCompression.CompressedHQ;imp.SaveAndReimport();}
        font=TMP_Settings.defaultFontAsset;
    }
    static GameObject Settings()
    {
        var canvas=Canvas("SettingsUI",200);var script=canvas.AddComponent<SettingsPanelUI>();var modal=Rect("SettingsModal",canvas.transform,0,0,1920,1080);Stretch(modal);
        var dim=Image("DismissBackdrop",modal,0,0,1920,1080,new Color(0,0,0,.76f),true);Stretch(dim.rectTransform);var dismiss=dim.gameObject.AddComponent<UnityEngine.UI.Button>();dismiss.transition=UnityEngine.UI.Selectable.Transition.None;UnityEditor.Events.UnityEventTools.AddPersistentListener(dismiss.onClick,script.Close);
        var frame=Frame(modal);var edge=Image("SettingsCardBorder",frame,470,120,980,840,new Color(.40f,.38f,.27f),false,true);var card=Image("SettingsCard",edge.transform,2,2,976,836,Panel,true,true);
        Text("Eyebrow",card.transform,"KARARGÂH / TERCİHLER",52,34,720,30,17,Brass,true);
        Text("Title",card.transform,"AYARLAR",52,77,700,60,44,Cream,true);Button("Close",card.transform,"×",866,35, sixty,60,script.Close);
        Text("Hint",card.transform,"Değişiklikler Uygula ile kaydedilir.",52,147,800,32,20,Muted);
        Image("Divider",card.transform,52,198,872,1,new Color(.3f,.36f,.29f));
        Text("SoundTitle",card.transform,"SES",52,225,300,30,17,Brass,true);Text("VolumeLabel",card.transform,"Genel ses",52,277,300,42,24,Cream);
        var volume=Slider(card.transform,"MasterVolume",432,282,370,28);var volumeText=Text("VolumeValue",card.transform,"100%",826,274,94,40,23,Cream,true);
        Text("DisplayTitle",card.transform,"GÖRÜNTÜ",52,350,300,30,17,Brass,true);
        Text("ResolutionLabel",card.transform,"Çözünürlük",52,396,350,48,24,Cream);var resolution=Text("ResolutionValue",card.transform,"1920 × 1080",495,396,360,48,23,Cream,true);resolution.alignment=TextAlignmentOptions.Center;
        var prev=Button("ResolutionPrevious",card.transform,"‹",432,396,54,48,null);UnityEditor.Events.UnityEventTools.AddIntPersistentListener(prev.onClick,script.ChangeResolution,-1);
        var next=Button("ResolutionNext",card.transform,"›",866,396,54,48,null);UnityEditor.Events.UnityEventTools.AddIntPersistentListener(next.onClick,script.ChangeResolution,1);
        Text("QualityLabel",card.transform,"Grafik kalitesi",52,464,350,48,24,Cream);var quality=Text("QualityValue",card.transform,"YÜKSEK",495,464,360,48,22,Cream,true);quality.alignment=TextAlignmentOptions.Center;
        prev=Button("QualityPrevious",card.transform,"‹",432,464,54,48,null);UnityEditor.Events.UnityEventTools.AddIntPersistentListener(prev.onClick,script.ChangeQuality,-1);
        next=Button("QualityNext",card.transform,"›",866,464,54,48,null);UnityEditor.Events.UnityEventTools.AddIntPersistentListener(next.onClick,script.ChangeQuality,1);
        Text("FullscreenLabel",card.transform,"Tam ekran",52,539,720,44,24,Cream);var fullscreen=Toggle(card.transform,"Fullscreen",884,543);
        Text("VSyncLabel",card.transform,"Dikey senkronizasyon (VSync)",52,601,800,44,24,Cream);var vsync=Toggle(card.transform,"VSync",884,605);
        Image("FooterDivider",card.transform,52,676,872,1,new Color(.3f,.36f,.29f));
        Button("Cancel",card.transform,"VAZGEÇ",432,719,220,64,script.Close);Button("Apply",card.transform,"UYGULA",676,719,244,64,script.Apply,true);
        Field(script,"modal",modal.gameObject);Field(script,"volume",volume);Field(script,"volumeText",volumeText);Field(script,"qualityText",quality);Field(script,"resolutionText",resolution);Field(script,"fullscreen",fullscreen);Field(script,"vsync",vsync);
        UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(volume.onValueChanged,script.RefreshLabels);
        modal.gameObject.SetActive(false);
        var prefab=PrefabUtility.SaveAsPrefabAsset(canvas,"Assets/Prefabs/UI/SettingsUI.prefab");Object.DestroyImmediate(canvas);return prefab;
    }
    const int sixty=60;
    static GameObject Loading()
    {
        var canvas=Canvas("SceneLoadingUI",500);var script=canvas.AddComponent<SceneLoadingUI>();var bg=Image("Background",canvas.transform,0,0,1920,1080,Ink,true);Stretch(bg.rectTransform);var frame=Frame(canvas.transform);
        Picture("Fortress",frame,"MenuFortress.png",440,0,1480,833,new Color(.65f,.73f,.67f));
        Text("Brand",frame,"BACKPACK\nLEGIONS",116,252,760,222,82,Cream,true);
        Text("Tagline",frame,"HER KARE BİR KARAR.",124,500,730,50,23,Brass,true);
        Image("Rule",frame,124,582,72,3,Brass);
        Text("TipLabel",frame,"SEFER NOTU",124,640,620,32,16,Brass,true);
        Text("Tip",frame,"Uyumlu eşyaları yan yana yerleştir.\nGüçlü bir savunma, iyi bir çantayla başlar.",124,686,640,95,25,Muted);
        var status=Text("LoadingStatus",frame,"KARARGÂH HAZIRLANIYOR",124,917,1300,40,18,Cream,true);
        var progress=Slider(frame,"Progress",124,971,1672,12);progress.interactable=false;progress.handleRect.gameObject.SetActive(false);progress.value=0;
        var retry=Button("Retry",frame,"TEKRAR DENE",1516,870,280,62,script.Retry);retry.gameObject.SetActive(false);
        Field(script,"status",status);Field(script,"progress",progress);Field(script,"retry",retry);
        var prefab=PrefabUtility.SaveAsPrefabAsset(canvas,"Assets/Resources/UI/SceneLoadingUI.prefab");Object.DestroyImmediate(canvas);return prefab;
    }
    public static string Build()
    {
        if(EditorApplication.isPlaying)throw new System.Exception("Exit Play mode first.");
        if(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/SettingsUI.prefab")!=null)throw new System.Exception("Front-end exists: inspect and edit in place.");
        PrepareArt();var settingsPrefab=Settings();var loadingPrefab=Loading();
        var scene=SceneManager.GetSceneByPath("Assets/Scenes/MainMenu.unity");if(!scene.isLoaded)scene=EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity",OpenSceneMode.Additive);SceneManager.SetActiveScene(scene);
        var oldCanvas=scene.GetRootGameObjects().FirstOrDefault(x=>x.GetComponent<Canvas>()!=null);if(oldCanvas!=null)oldCanvas.SetActive(false);
        foreach(var root in scene.GetRootGameObjects())if(root.name=="Button")root.SetActive(false);
        var manager=scene.GetRootGameObjects().Select(x=>x.GetComponent<MainMenuManager>()).First(x=>x!=null);
        var canvas=Canvas("FrontendCanvas",10);var bg=Image("Backdrop",canvas.transform,0,0,1920,1080,Ink);Stretch(bg.rectTransform);var frame=Frame(canvas.transform);
        var home=Rect("HomePanel",frame,0,0,1920,1080);Picture("Fortress",home,"MenuFortress.png",0,0,1920,1080,Color.white);
        Image("LeftShade",home,0,0,760,1080,new Color(.045f,.075f,.064f,.92f));
        Text("Edition",home,"BİR ÇANTA. BİR LEJYON.",116,104,630,40,19,Brass,true);
        Text("Title",home,"BACKPACK\nLEGIONS",108,200,690,240,88,Cream,true);
        Image("TitleRule",home,116,471,82,3,Brass);
        Text("Subtitle",home,"Çantanı düzenle. Kulelerini güçlendir.\nGeçidi savun.",116,513,620,90,25,Muted);
        Button("Play",home,"OYNA  ›",116,658,464,78,manager.StartGame,true);
        Button("Settings",home,"AYARLAR",116,754,464,66,manager.OpenSettings);
        Button("Quit",home,"ÇIKIŞ",116,838,464,66,manager.ExitGame);
        Text("Version",home,"GELİŞTİRME SÜRÜMÜ · 01",116,987,650,28,15,Muted);
        Text("WorldCaption",home,"KARAÇAM GEÇİDİ",1320,960,485,45,21,Cream,true);
        Text("WorldSubcaption",home,"Kuzey yolu seni bekliyor.",1320,1006,485,30,18,Muted);
        var chapter=Rect("ChapterPanel",frame,0,0,1920,1080);Image("ChapterBackground",chapter,0,0,1920,1080,Ink);
        Text("ChapterEyebrow",chapter,"SEFER HARİTASI",80,48,1000,34,18,Brass,true);Text("ChapterHeading",chapter,"BİR SONRAKİ SAVUNMAN",80,94,1200,68,43,Cream,true);
        Button("Back",chapter,"‹  GERİ",1450,73,160, sixty,manager.ShowHome);Button("Settings",chapter,"AYARLAR",1630,73,210,sixty,manager.OpenSettings);
        var mapBorder=Image("MapBorder",chapter,80,208,1220,764,new Color(.4f,.38f,.28f),false,true);Picture("Map",mapBorder.transform,"CampaignMap.png",4,4,1212,756,Color.white);
        Text("MapName",mapBorder.transform,"KUZEY TOPRAKLARI",34,25,600,45,24,new Color(.19f,.22f,.17f),true);
        Text("MapNote",mapBorder.transform,"01 — KARAÇAM SEFERİ",34,78,600,25,15,new Color(.26f,.28f,.22f));
        var points=Newtonsoft.Json.JsonConvert.DeserializeObject<float[][]>(System.IO.File.ReadAllText("ArtSource/Frontend/map_nodes.json"));
        var markers=new UnityEngine.UI.Image[5];
        for(int i=0;i<5;i++){
            float x=4+points[i][0]*1212-32,y=4+(1-points[i][1])*756-32;
            var b=Button("Chapter"+(i+1),mapBorder.transform,"0"+(i+1),x,y,64,64,null,i==0);UnityEditor.Events.UnityEventTools.AddIntPersistentListener(b.onClick,manager.SelectChapter,i);markers[i]=b.GetComponent<UnityEngine.UI.Image>();
            var tag=Image("Tag",mapBorder.transform,x-51,y+74,166,30,new Color(.075f,.11f,.09f,.92f),false,true);var tt=Text("Label",tag.transform,i==0?"KARAÇAM GEÇİDİ":"YAKINDA",8,0,150,30,13,Cream,true);tt.alignment=TextAlignmentOptions.Center;
        }
        var info=Image("ChapterDetails",chapter,1332,208,508,764,Panel,false,true);Text("InfoEyebrow",info.transform,"SEFER DOSYASI",34,30,440,30,16,Brass,true);
        var status=Text("Status",info.transform,"BÖLÜM 01 / OYNANABİLİR",34,91,440,35,17,Brass,true);
        var title=Text("Title",info.transform,"KARAÇAM GEÇİDİ",34,148,440,105,39,Cream,true);
        Image("InfoRule",info.transform,34,280,440,1,new Color(.3f,.36f,.29f));
        var desc=Text("Description",info.transform,"Orman yolunu savun. Kulelerini kur, çantanı düzenle ve geçidi koru.\n\n5 dalga · 3. dalgada uçan düşmanlar\nSon dalgada bölüm sonu düşmanı",34,319,440,240,24,Muted);
        Text("InfoFootnote",info.transform,"Savunmanı hazırlamak için\nher dalga öncesinde 60 saniyen var.",34,585,440,65,19,Muted);
        var play=Button("StartChapter",info.transform,"BÖLÜME BAŞLA",34,674,440, sixty,manager.PlaySelectedChapter,true);
        Text("MapFooter",chapter,"Bir bölge seç. Savunmanı planla.",80,1010,1200,30,18,Muted);
        var settings=(GameObject)PrefabUtility.InstantiatePrefab(settingsPrefab,scene);Field(manager,"homePanel",home.gameObject);Field(manager,"chapterPanel",chapter.gameObject);Field(manager,"settingsPanel",settings.GetComponent<SettingsPanelUI>());Field(manager,"chapterTitle",title);Field(manager,"chapterDescription",desc);Field(manager,"chapterStatus",status);Field(manager,"chapterPlayButton",play);
        var so=new SerializedObject(manager);var arr=so.FindProperty("chapterMarkers");arr.arraySize=markers.Length;for(int i=0;i<markers.Length;i++)arr.GetArrayElementAtIndex(i).objectReferenceValue=markers[i];so.ApplyModifiedPropertiesWithoutUndo();chapter.gameObject.SetActive(false);
        EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
        var game=SceneManager.GetSceneByPath("Assets/Scenes/GameScane.unity");if(!game.isLoaded)game=EditorSceneManager.OpenScene("Assets/Scenes/GameScane.unity",OpenSceneMode.Additive);SceneManager.SetActiveScene(game);
        var gm=game.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<GameManager>(true)).First();var gameSettings=(GameObject)PrefabUtility.InstantiatePrefab(settingsPrefab,game);gm.settingsPanel=gameSettings.GetComponent<SettingsPanelUI>();
        var pause=gm.pausePanel.GetComponent<RectTransform>();pause.anchorMin=pause.anchorMax=pause.pivot=new Vector2(.5f,.5f);pause.anchoredPosition=Vector2.zero;pause.sizeDelta=new Vector2(480,420);
        var pauseImg=gm.pausePanel.GetComponent<UnityEngine.UI.Image>();if(pauseImg!=null){pauseImg.color=Panel;pauseImg.sprite=rounded;pauseImg.type=UnityEngine.UI.Image.Type.Sliced;}
        foreach(var item in new[]{"CountineButton","MainMenuButton"}){var r=gm.pausePanel.transform.Find(item).GetComponent<RectTransform>();r.anchorMin=r.anchorMax=new Vector2(.5f,.5f);r.pivot=new Vector2(.5f,.5f);r.sizeDelta=new Vector2(360, sixty);r.anchoredPosition=new Vector2(0,item=="CountineButton"?40:-120);var i=r.GetComponent<UnityEngine.UI.Image>();i.color=item=="CountineButton"?Brass:new Color(.14f,.2f,.17f);i.sprite=rounded;i.type=UnityEngine.UI.Image.Type.Sliced;var t=r.GetComponentInChildren<TMP_Text>();if(t!=null){t.font=font;t.fontSize=23;t.color=item=="CountineButton"?Ink:Cream;t.text=item=="CountineButton"?"DEVAM ET":"ANA MENÜ";}}
        Button("SettingsButton",gm.pausePanel.transform,"AYARLAR",60,235,360,sixty,gm.OpenSettings);
        foreach(var b in gm.pausePanel.GetComponentsInChildren<UnityEngine.UI.Button>(true)){var colors=UnityEngine.UI.ColorBlock.defaultColorBlock;colors.highlightedColor=new Color(.88f,.91f,.85f);b.colors=colors;}
        var heading=gm.pausePanel.GetComponentsInChildren<TMP_Text>().FirstOrDefault(t=>t.transform.parent==gm.pausePanel.transform);if(heading!=null){heading.text="DURAKLATILDI";heading.font=font;heading.fontSize=30;heading.color=Cream;var r=heading.rectTransform;r.anchorMin=r.anchorMax=new Vector2(.5f,1);r.pivot=new Vector2(.5f,1);r.anchoredPosition=new Vector2(0,-30);r.sizeDelta=new Vector2(440, sixty);}
        EditorUtility.SetDirty(gm);EditorSceneManager.MarkSceneDirty(game);EditorSceneManager.SaveScene(game);
        var boot=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);SceneManager.SetActiveScene(boot);var bootUI=(GameObject)PrefabUtility.InstantiatePrefab(loadingPrefab,boot);StringField(bootUI.GetComponent<SceneLoadingUI>(),"loadOnStart","MainMenu");new GameObject("EventSystem",typeof(UnityEngine.EventSystems.EventSystem),typeof(UnityEngine.EventSystems.StandaloneInputModule));EditorSceneManager.SaveScene(boot,"Assets/Scenes/Boot.unity");
        var builds=EditorBuildSettings.scenes.Where(s=>s.path!="Assets/Scenes/Boot.unity").ToList();builds.Insert(0,new EditorBuildSettingsScene("Assets/Scenes/Boot.unity",true));EditorBuildSettings.scenes=builds.ToArray();
        AssetDatabase.SaveAssets();EditorSceneManager.OpenScene("Assets/Scenes/Boot.unity",OpenSceneMode.Single);
        return "Front-end built: Boot → MainMenu → chapter map → GameScane; shared settings prefab.";
    }
}
