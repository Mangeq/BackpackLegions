using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class HudReferenceBuilder
{
    const string Folder="Assets/Resources/UI/HudReference/";
    static Sprite Art(string name) { return Resources.Load<Sprite>("UI/HudReference/"+name); }
    static void Alpha(string name,string source,Rect region)
    {
        if(AssetDatabase.LoadAssetAtPath<Sprite>(Folder+name+".asset")!=null)return;
        var t=AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+source+".png");var p=t.GetPixels32();int x0=(int)(region.xMin*t.width),x1=(int)(region.xMax*t.width),y0=(int)((1-region.yMax)*t.height),y1=(int)((1-region.yMin)*t.height),l=x1,r=x0,b=y1,top=y0;
        for(int y=y0;y<y1;y++)for(int x=x0;x<x1;x++)if(p[y*t.width+x].a>25){l=Mathf.Min(l,x);r=Mathf.Max(r,x);b=Mathf.Min(b,y);top=Mathf.Max(top,y);}
        if(r<=l||top<=b)throw new InvalidOperationException("Empty HUD sprite: "+name);
        l=Mathf.Max(x0,l-2);r=Mathf.Min(x1-1,r+2);b=Mathf.Max(y0,b-2);top=Mathf.Min(y1-1,top+2);
        var bounds=new Rect(l,b,r-l+1,top-b+1);var border=name=="TooltipSliced"?new Vector4(bounds.width*.14f,bounds.height*.14f,bounds.width*.14f,bounds.height*.325f):Vector4.zero;
        var sprite=Sprite.Create(t,bounds,Vector2.one*.5f,100,0,SpriteMeshType.FullRect,border);sprite.name=name;AssetDatabase.CreateAsset(sprite,Folder+name+".asset");
    }
    static void Place(RectTransform r,Transform parent,float x,float y,float w,float h)
    {r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);r.localScale=Vector3.one;r.localRotation=Quaternion.identity;}
    static RectTransform Rect(string name,Transform parent,float x,float y,float w,float h)
    {var r=parent.Find(name) as RectTransform;if(r==null)r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();Place(r,parent,x,y,w,h);return r;}
    static TMP_Text Text(string name,Transform p,float x,float y,float w,float h,int size,bool title=true)
    {
        var r=Rect(name,p,x,y,w,h);var t=r.GetComponent<TMP_Text>();if(t==null)t=r.gameObject.AddComponent<TextMeshProUGUI>();bool displayHeading=title&&name!="ReferenceGold"&&name!="ReferenceWave"&&name!="ReferenceCountdown";t.font=displayHeading?GameFonts.Title:GameFonts.Body;t.fontSize=size;t.enableAutoSizing=true;t.fontSizeMin=12;t.fontSizeMax=size;t.color=new Color(1,.85f,.56f);t.alignment=TextAlignmentOptions.Center;t.enableWordWrapping=false;t.enabled=true;t.raycastTarget=false;return t;
    }
    static void Hide(RectTransform root)
    {foreach(var g in root.GetComponentsInChildren<Graphic>(true))g.enabled=false;foreach(var shadow in root.GetComponentsInChildren<Shadow>(true))shadow.enabled=false;}
    static Image Face(RectTransform root,string sprite,bool ray=false)
    {var image=root.GetComponent<Image>();if(image==null)image=root.gameObject.AddComponent<Image>();image.sprite=Art(sprite);image.color=Color.white;image.type=Image.Type.Simple;image.preserveAspect=true;image.enabled=true;image.raycastTarget=ray;return image;}
    static void Configure(UİManager ui,InfoPanelUI info,UnityEngine.SceneManagement.Scene scene)
    {
        var all=scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<UnityEngine.UI.Text>(true)).ToArray();var gold=all.First(x=>x.name=="GoldText");var wave=all.First(x=>x.name=="WaveText");
        var goldBar=(RectTransform)gold.transform.parent;var canvas=goldBar.parent;Hide(goldBar);Place(goldBar,canvas,20,20,210,76);Face(goldBar,"GoldFrame");var goldText=Text("ReferenceGold",goldBar,100,10,96,55,39);
        var waveBar=(RectTransform)wave.transform.parent;Hide(waveBar);Place(waveBar,canvas,20,101,210,76);Face(waveBar,"WaveFrame");var waveText=Text("ReferenceWave",waveBar,101,6,96,65,23);waveText.enableWordWrapping=true;
        Place((RectTransform)ui.startGameButton.transform,canvas,24,181,145,35);
        var speeds=(RectTransform)ui.speedButtons[0].transform.parent;Hide(speeds);Place(speeds,canvas,20,220,210,82);Face(speeds,"SpeedFrame");var labels=new TMP_Text[3];
        for(int i=0;i<3;i++)
        {
            var button=ui.speedButtons[i];Place((RectTransform)button.transform,speeds,8+65*i,13,62,59);
            var hit=button.GetComponent<Image>();hit.sprite=null;hit.enabled=true;hit.color=new Color(1,1,1,.001f);hit.raycastTarget=true;button.targetGraphic=null;button.transition=Selectable.Transition.None;
            var selected=ui.speedHighlights[i];Place(selected.rectTransform,button.transform,4,5,54,49);selected.sprite=Art("SpeedSelected");selected.type=Image.Type.Simple;selected.color=Color.white;selected.raycastTarget=false;selected.enabled=i==0;
            labels[i]=Text("ReferenceSpeed",button.transform,0,0,62,59,28);labels[i].text=(i+1)+"x";
        }
        var hearts=canvas.Find("HealBar") as RectTransform;
        if(hearts!=null){Place(hearts,canvas,28,308,150,36);int i=0;foreach(var heart in hearts.GetComponentsInChildren<Image>(true).Where(x=>x.name.StartsWith("Heart")))Place(heart.rectTransform,hearts,43*i++,0,35,35);}
        var timer=(RectTransform)ui.preparationCountdownText.transform.parent;Hide(timer);Place(timer,canvas,0,20,500,180);timer.anchorMin=timer.anchorMax=timer.pivot=new Vector2(.5f,1);timer.anchoredPosition=new Vector2(0,-20);Face(timer,"CountdownFrame");var countdown=Text("ReferenceCountdown",timer,124,65,350,80,25);countdown.enableWordWrapping=true;
        var skin=ui.GetComponent<HudReferenceSkin>();if(skin==null)skin=ui.gameObject.AddComponent<HudReferenceSkin>();skin.Configure(ui,gold,wave,goldText,waveText,countdown,labels);
        Hide(info.panel);var tooltipFace=Face(info.panel,"TooltipSliced");tooltipFace.preserveAspect=false;tooltipFace.type=Image.Type.Sliced;tooltipFace.pixelsPerUnitMultiplier=tooltipFace.sprite.rect.width/350f;info.panel.sizeDelta=new Vector2(350,356);info.panel.localScale=Vector3.one;info.panel.pivot=new Vector2(0,1);
        Place(info.icon.rectTransform,info.panel,29,30,66,68);info.icon.enabled=true;info.icon.preserveAspect=true;info.icon.raycastTarget=false;
        var heading=Text("ReferenceTitle",info.panel,107,28,219,37,24);heading.alignment=TextAlignmentOptions.MidlineLeft;
        var subtitle=Text("ReferenceSubtitle",info.panel,107,66,219,29,19,false);subtitle.alignment=TextAlignmentOptions.MidlineLeft;
        var body=Text("ReferenceBody",info.panel,26,138,298,198,19,false);body.alignment=TextAlignmentOptions.TopLeft;body.enableWordWrapping=true;body.color=new Color(.94f,.88f,.72f);body.fontSizeMin=12;
        var tooltip=info.GetComponent<InfoReferenceSkin>();if(tooltip==null)tooltip=info.gameObject.AddComponent<InfoReferenceSkin>();tooltip.Configure(info,heading,subtitle,body);info.Hide();
        if(Art("StartGame")!=null)StartReferenceBuilder.Style(ui);
    }
    [MenuItem("Tools/Backpack Defense/Apply Reference HUD Panels")]
    public static string Apply()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play first.");
        foreach(var name in new[]{"Tooltip","Gold","Wave","Countdown","Speed"}){var path=Folder+name+".png";AssetDatabase.ImportAsset(path);var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Default;importer.alphaIsTransparency=true;importer.isReadable=true;importer.mipmapEnabled=false;importer.npotScale=TextureImporterNPOTScale.None;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=4096;importer.SaveAndReimport();}
        foreach(var name in new[]{"Tooltip","Gold","Wave","Countdown"})Alpha(name+"Frame",name,new Rect(0,0,1,1));Alpha("TooltipSliced","Tooltip",new Rect(0,0,1,1));Alpha("SpeedFrame","Speed",new Rect(0,0,1,.59f));Alpha("SpeedSelected","Speed",new Rect(0,.61f,1,.39f));
        var scene=UnityEngine.SceneManagement.SceneManager.GetSceneByPath("Assets/Scenes/GameScane.unity");bool loaded=scene.isLoaded;if(!loaded)scene=EditorSceneManager.OpenScene("Assets/Scenes/GameScane.unity",OpenSceneMode.Additive);
        try{var objects=scene.GetRootGameObjects();var ui=objects.SelectMany(x=>x.GetComponentsInChildren<UİManager>(true)).First();var info=objects.SelectMany(x=>x.GetComponentsInChildren<InfoPanelUI>(true)).First();Configure(ui,info,scene);foreach(var component in objects.SelectMany(x=>x.GetComponentsInChildren<Component>(true)))if(component!=null)EditorUtility.SetDirty(component);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();}
        finally{if(!loaded)EditorSceneManager.CloseScene(scene,true);}
        return "Five native HUD and information panels updated";
    }
}
