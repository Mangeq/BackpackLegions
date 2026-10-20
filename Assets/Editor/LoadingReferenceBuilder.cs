using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public static class LoadingReferenceBuilder
{
    const string Folder="Assets/Resources/UI/LoadingReference/";
    const string Prefab="Assets/Resources/UI/SceneLoadingUI.prefab";
    static Sprite Sprite(string name,string source,Rect bounds)
    {
        var found=AssetDatabase.LoadAssetAtPath<Sprite>(Folder+name+".asset");if(found!=null)return found;
        var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+source+".png");
        var sprite=UnityEngine.Sprite.Create(texture,new Rect(bounds.x/1024*texture.width,(565-bounds.y-bounds.height)/565*texture.height,bounds.width/1024*texture.width,bounds.height/565*texture.height),Vector2.one*.5f,100,0,SpriteMeshType.FullRect);
        sprite.name=name;AssetDatabase.CreateAsset(sprite,Folder+name+".asset");return sprite;
    }
    static void Place(RectTransform r,Transform parent,float x,float y,float width,float height)
    {r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(width,height);r.localScale=Vector3.one;}
    static void Stretch(RectTransform r)
    {r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.pivot=Vector2.one*.5f;r.offsetMin=r.offsetMax=Vector2.zero;}
    static void ChainSprite()
    {
        if(AssetDatabase.LoadAssetAtPath<Sprite>(Folder+"LoadingChain.asset")!=null)return;
        var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+"LoadingChain.png");
        var pixels=texture.GetPixels32();int left=texture.width,right=0,bottom=texture.height,top=0;
        for(int y=0;y<texture.height;y++)for(int x=0;x<texture.width;x++)if(pixels[y*texture.width+x].a>12){left=Mathf.Min(left,x);right=Mathf.Max(right,x);bottom=Mathf.Min(bottom,y);top=Mathf.Max(top,y);}
        if(right<=left||top<=bottom)throw new InvalidOperationException("Chain sprite has no visible pixels.");
        left=Mathf.Max(0,left-3);bottom=Mathf.Max(0,bottom-3);right=Mathf.Min(texture.width-1,right+3);top=Mathf.Min(texture.height-1,top+3);
        var sprite=UnityEngine.Sprite.Create(texture,new Rect(left,bottom,right-left+1,top-bottom+1),Vector2.one*.5f,100,0,SpriteMeshType.FullRect);sprite.name="LoadingChain";AssetDatabase.CreateAsset(sprite,Folder+"LoadingChain.asset");
    }
    public static void Style(SceneLoadingUI loading)
    {
        var fields=new SerializedObject(loading);
        var progress=(Slider)fields.FindProperty("progress").objectReferenceValue;
        var status=(TMP_Text)fields.FindProperty("status").objectReferenceValue;
        var retry=(Button)fields.FindProperty("retry").objectReferenceValue;
        foreach(var graphic in loading.GetComponentsInChildren<Graphic>(true))graphic.enabled=false;
        foreach(var shadow in loading.GetComponentsInChildren<Shadow>(true))shadow.enabled=false;
        var background=loading.transform.Find("Background").GetComponent<Image>();background.enabled=true;background.color=Color.black;background.sprite=null;background.raycastTarget=true;
        var stage=loading.transform.Find("ReferenceArtwork") as RectTransform;
        if(stage==null)stage=new GameObject("ReferenceArtwork",typeof(RectTransform),typeof(CanvasRenderer),typeof(Image)).GetComponent<RectTransform>();
        stage.SetParent(loading.transform,false);stage.anchorMin=stage.anchorMax=stage.pivot=Vector2.one*.5f;stage.sizeDelta=new Vector2(1024,565);stage.SetAsLastSibling();
        var image=stage.GetComponent<Image>();image.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Folder+"LoadingBackground.asset");image.color=Color.white;image.type=Image.Type.Simple;image.enabled=true;image.raycastTarget=true;
        Place(progress.GetComponent<RectTransform>(),stage,279,507,470,29);progress.minValue=0;progress.maxValue=1;progress.wholeNumbers=false;progress.interactable=false;progress.transition=Selectable.Transition.None;
        if(progress.handleRect!=null)progress.handleRect.gameObject.SetActive(false);progress.handleRect=null;
        var fill=progress.transform.Find("LiveChainFill") as RectTransform;
        if(fill==null)fill=new GameObject("LiveChainFill",typeof(RectTransform),typeof(CanvasRenderer),typeof(Image)).GetComponent<RectTransform>();
        fill.SetParent(progress.transform,false);Stretch(fill);var chain=fill.GetComponent<Image>();chain.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Folder+"LoadingChain.asset");chain.type=Image.Type.Filled;chain.fillMethod=Image.FillMethod.Horizontal;chain.fillOrigin=0;chain.color=Color.white;chain.enabled=true;chain.raycastTarget=false;progress.fillRect=fill;progress.targetGraphic=null;progress.value=0;
        Place(status.rectTransform,stage,292,484,440,25);status.enabled=true;status.font=GameFonts.Body;status.fontSize=18;status.enableAutoSizing=true;status.fontSizeMin=12;status.fontSizeMax=18;status.enableWordWrapping=false;status.alignment=TextAlignmentOptions.Center;status.color=new Color(1,.86f,.63f);status.raycastTarget=false;status.text="BÖLÜM HAZIRLANIYOR";
        Place((RectTransform)retry.transform,stage,407,442,210,34);var buttonImage=retry.GetComponent<Image>();buttonImage.enabled=true;buttonImage.sprite=null;buttonImage.color=new Color(.19f,.13f,.075f,.96f);buttonImage.raycastTarget=true;retry.targetGraphic=buttonImage;
        var caption=retry.GetComponentInChildren<TMP_Text>(true);caption.enabled=true;caption.font=status.font;caption.text="TEKRAR DENE";caption.fontSize=17;caption.alignment=TextAlignmentOptions.Center;caption.color=status.color;Stretch(caption.rectTransform);retry.gameObject.SetActive(false);
        var skin=loading.GetComponent<LoadingReferenceSkin>();if(skin==null)skin=loading.gameObject.AddComponent<LoadingReferenceSkin>();skin.Configure(stage);
    }
    [MenuItem("Tools/Backpack Defense/Apply Reference Loading Screen")]
    public static string Apply()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play Mode first.");
        foreach(var name in new[]{"LoadingEmpty","LoadingOriginal","LoadingChain"})
        {var path=Folder+name+".png";AssetDatabase.ImportAsset(path);var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Default;importer.alphaIsTransparency=name=="LoadingChain";importer.isReadable=name=="LoadingChain";importer.mipmapEnabled=false;importer.npotScale=TextureImporterNPOTScale.None;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=4096;importer.SaveAndReimport();}
        Sprite("LoadingBackground","LoadingEmpty",new Rect(0,0,1024,565));
        ChainSprite();
        var root=PrefabUtility.LoadPrefabContents(Prefab);
        try{Style(root.GetComponent<SceneLoadingUI>());PrefabUtility.SaveAsPrefabAsset(root,Prefab);}finally{PrefabUtility.UnloadPrefabContents(root);}
        // Boot keeps its loadOnStart override and inherits the same persistent prefab.
        var boot=UnityEngine.SceneManagement.SceneManager.GetSceneByPath("Assets/Scenes/Boot.unity");bool wasLoaded=boot.isLoaded;
        if(!wasLoaded)boot=EditorSceneManager.OpenScene("Assets/Scenes/Boot.unity",OpenSceneMode.Additive);
        try{foreach(var go in boot.GetRootGameObjects())foreach(var loading in go.GetComponentsInChildren<SceneLoadingUI>(true)){Style(loading);EditorUtility.SetDirty(loading);}
            EditorSceneManager.MarkSceneDirty(boot);EditorSceneManager.SaveScene(boot);
        }finally{if(!wasLoaded)EditorSceneManager.CloseScene(boot,true);}
        AssetDatabase.SaveAssets();return "Loading prefab and Boot updated";
    }
}
