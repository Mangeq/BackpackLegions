using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public static class ChapterReferenceBuilder
{
    const string Folder="Assets/Resources/UI/ChapterReference/";
    static void Crop(string name,string source,Rect bounds)
    {
        if(AssetDatabase.LoadAssetAtPath<Sprite>(Folder+name+".asset")!=null)return;
        var t=AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+source+".png");var r=new Rect(bounds.x/1024*t.width,(572-bounds.y-bounds.height)/572*t.height,bounds.width/1024*t.width,bounds.height/572*t.height);
        var sprite=Sprite.Create(t,r,Vector2.one*.5f,100,0,SpriteMeshType.FullRect);sprite.name=name;AssetDatabase.CreateAsset(sprite,Folder+name+".asset");
    }
    static void AlphaPiece(string name,Rect region)
    {
        if(AssetDatabase.LoadAssetAtPath<Sprite>(Folder+name+".asset")!=null)return;
        var t=AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+"ChapterParts.png");var pixels=t.GetPixels32();int x0=(int)(region.xMin*t.width),x1=(int)(region.xMax*t.width),y0=(int)((1-region.yMax)*t.height),y1=(int)((1-region.yMin)*t.height);
        int left=x1,right=x0,bottom=y1,top=y0;
        for(int y=y0;y<y1;y++)for(int x=x0;x<x1;x++)if(pixels[y*t.width+x].a>25){left=Mathf.Min(left,x);right=Mathf.Max(right,x);bottom=Mathf.Min(bottom,y);top=Mathf.Max(top,y);}
        if(right<=left||top<=bottom)throw new InvalidOperationException("Missing sprite quadrant: "+name);
        left=Mathf.Max(x0,left-3);right=Mathf.Min(x1-1,right+3);bottom=Mathf.Max(y0,bottom-3);top=Mathf.Min(y1-1,top+3);
        var sprite=Sprite.Create(t,new Rect(left,bottom,right-left+1,top-bottom+1),Vector2.one*.5f,100,0,SpriteMeshType.FullRect);sprite.name=name;AssetDatabase.CreateAsset(sprite,Folder+name+".asset");
    }
    static RectTransform Rect(string name,Transform parent,float x,float y,float w,float h)
    {
        var r=parent.Find(name) as RectTransform;if(r==null)r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();Place(r,parent,x,y,w,h);return r;
    }
    static void Place(RectTransform r,Transform parent,float x,float y,float w,float h)
    {r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);r.localScale=Vector3.one;r.localRotation=Quaternion.identity;}
    static TMP_Text Text(string name,Transform parent,float x,float y,float w,float h,int size,Color color,TextAlignmentOptions align=TextAlignmentOptions.Center)
    {
        var r=Rect(name,parent,x,y,w,h);var t=r.GetComponent<TMP_Text>();if(t==null)t=r.gameObject.AddComponent<TextMeshProUGUI>();
        t.font=name=="ReferenceNumber"||name=="ReferenceName"?GameFonts.Title:GameFonts.Body;t.fontSize=size;t.enableAutoSizing=true;t.fontSizeMin=Mathf.Min(10,size);t.fontSizeMax=size;t.enableWordWrapping=false;t.color=color;t.alignment=align;t.enabled=true;t.raycastTarget=false;return t;
    }
    static Image Picture(string name,Transform parent,string source,float x,float y,float w,float h)
    {
        var r=Rect(name,parent,x,y,w,h);var image=r.GetComponent<Image>();if(image==null)image=r.gameObject.AddComponent<Image>();image.enabled=true;image.sprite=ChapterReferenceSkin.Art(source);image.color=Color.white;image.type=Image.Type.Simple;image.preserveAspect=true;image.raycastTarget=false;return image;
    }
    static void Button(Button b,RectTransform panel,string art,Rect rect)
    {
        Place((RectTransform)b.transform,panel,rect.x,rect.y,rect.width,rect.height);var image=b.GetComponent<Image>();image.enabled=true;image.sprite=ChapterReferenceSkin.Art(art);image.type=Image.Type.Simple;image.color=Color.white;image.raycastTarget=true;b.targetGraphic=image;
        var colors=ColorBlock.defaultColorBlock;colors.highlightedColor=new Color(1.12f,1.12f,1.12f);colors.selectedColor=Color.white;b.colors=colors;
    }
    static void Configure(MainMenuManager owner)
    {
        var so=new SerializedObject(owner);var panel=((GameObject)so.FindProperty("chapterPanel").objectReferenceValue).GetComponent<RectTransform>();
        foreach(var graphic in panel.GetComponentsInChildren<Graphic>(true))graphic.enabled=false;
        foreach(var effect in panel.GetComponentsInChildren<Shadow>(true))effect.enabled=false;
        var face=panel.GetComponent<Image>();if(face==null)face=panel.gameObject.AddComponent<Image>();face.sprite=ChapterReferenceSkin.Art("ChapterBackground");face.color=Color.white;face.type=Image.Type.Simple;face.enabled=true;face.raycastTarget=true;
        panel.anchorMin=panel.anchorMax=panel.pivot=Vector2.one*.5f;panel.sizeDelta=new Vector2(1024,572);panel.anchoredPosition=Vector2.zero;
        var back=panel.GetComponentsInChildren<Button>(true).First(b=>b.name=="Back");Button(back,panel,"ChapterBack",new Rect(18,9,136,54));
        var settings=panel.GetComponentsInChildren<Button>(true).FirstOrDefault(b=>b.name=="Settings");if(settings!=null)settings.gameObject.SetActive(false);
        var play=(Button)so.FindProperty("chapterPlayButton").objectReferenceValue;Button(play,panel,"ChapterPlay",new Rect(750,448,136,60));
        var chapterArray=so.FindProperty("chapters");var chapters=new ChapterData[chapterArray.arraySize];for(int i=0;i<chapters.Length;i++)chapters[i]=(ChapterData)chapterArray.GetArrayElementAtIndex(i).objectReferenceValue;
        var markerArray=so.FindProperty("chapterMarkers");int count=markerArray.arraySize;
        var markers=new Image[count];var digits=new TMP_Text[count];var labels=new TMP_Text[count];var stars=new Image[count*3];
        var points=new[]{new Vector2(120,250),new Vector2(254,194),new Vector2(404,207),new Vector2(402,355),new Vector2(548,272)};
        for(int i=0;i<count;i++)
        {
            markers[i]=(Image)markerArray.GetArrayElementAtIndex(i).objectReferenceValue;var p=points[Mathf.Min(i,points.Length-1)];
            var b=markers[i].GetComponent<Button>();Button(b,panel,"ChapterNode",new Rect(p.x-39,p.y-39,78,78));markers[i].preserveAspect=true;
            digits[i]=markers[i].GetComponentInChildren<TMP_Text>(true);var labelRect=digits[i].rectTransform;Place(labelRect,markers[i].transform,15,15,48,48);digits[i].font=GameFonts.Title;digits[i].enabled=true;digits[i].fontSize=25;digits[i].alignment=TextAlignmentOptions.Center;digits[i].color=Color.white;digits[i].raycastTarget=false;
            var tag=Picture("ReferenceTag"+i,panel,"ChapterTag",p.x-82,p.y+32,164,29);tag.preserveAspect=false;
            labels[i]=Text("Caption",tag.transform,5,1,154,27,12,new Color(.19f,.11f,.065f));labels[i].fontStyle=FontStyles.Bold;
            for(int s=0;s<3;s++)stars[i*3+s]=Picture("MapStar"+i+"_"+s,panel,"ChapterStar",p.x-31+s*21,p.y-64,21,23);
        }
        var cream=new Color(1,.9f,.73f);
        var heading=Text("ReferenceNumber",panel,674,266,284,23,20,cream);heading.fontStyle=FontStyles.Bold;
        var title=Text("ReferenceName",panel,674,289,284,23,16,cream);title.fontStyle=FontStyles.Bold;
        var details=new TMP_Text[5];for(int i=0;i<5;i++)details[i]=Text("ReferenceDetail"+i,panel,674,344+i*21,287,22,15,cream,TextAlignmentOptions.MidlineLeft);
        var large=new Image[3];for(int s=0;s<3;s++)large[s]=Picture("DetailStar"+s,panel,"ChapterStar",765+s*39,313,31,31);
        var total=Text("ReferenceTotalStars",panel,842,20,56,29,14,cream);
        var available=Text("ReferencePoints",panel,944,20,51,29,13,cream);
        var skin=panel.GetComponent<ChapterReferenceSkin>();if(skin==null)skin=panel.gameObject.AddComponent<ChapterReferenceSkin>();
        skin.Configure(panel,chapters,markers,digits,labels,heading,title,details,stars,large,total,available,play);
        panel.gameObject.SetActive(false);
    }
    [MenuItem("Tools/Backpack Defense/Apply Reference Chapter Map")]
    public static string Apply()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play first.");
        foreach(var name in new[]{"ChapterMap","ChapterParts"})
        {var path=Folder+name+".png";AssetDatabase.ImportAsset(path);var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Default;importer.alphaIsTransparency=name=="ChapterParts";importer.isReadable=name=="ChapterParts";importer.mipmapEnabled=false;importer.npotScale=TextureImporterNPOTScale.None;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=4096;importer.SaveAndReimport();}
        Crop("ChapterBackground","ChapterMap",new Rect(0,0,1024,572));Crop("ChapterBack","ChapterMap",new Rect(18,9,136,54));Crop("ChapterPlay","ChapterMap",new Rect(750,448,136,60));
        AlphaPiece("ChapterNode",new Rect(0,0,.43f,.58f));AlphaPiece("SelectedNode",new Rect(.45f,0,.55f,.6f));AlphaPiece("ChapterTag",new Rect(0,.62f,.62f,.38f));AlphaPiece("ChapterStar",new Rect(.65f,.62f,.35f,.38f));
        var menu=UnityEngine.SceneManagement.SceneManager.GetSceneByPath("Assets/Scenes/MainMenu.unity");bool wasLoaded=menu.isLoaded;
        if(!wasLoaded)menu=EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity",OpenSceneMode.Additive);
        try
        {
            var manager=menu.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<MainMenuManager>(true)).First();Configure(manager);
            foreach(var component in menu.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<Component>(true)))if(component!=null)EditorUtility.SetDirty(component);
            EditorSceneManager.MarkSceneDirty(menu);EditorSceneManager.SaveScene(menu);AssetDatabase.SaveAssets();
        }
        finally{if(!wasLoaded)EditorSceneManager.CloseScene(menu,true);}
        return "Reference chapter map saved with native chapter callbacks";
    }
}
