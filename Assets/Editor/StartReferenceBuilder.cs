using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class StartReferenceBuilder
{
    const string Folder="Assets/Resources/UI/HudReference/";
    static void Place(RectTransform rect,float x,float y,float width,float height)
    {rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(0,1);rect.anchoredPosition=new Vector2(x,-y);rect.sizeDelta=new Vector2(width,height);rect.localScale=Vector3.one;}
    public static void Style(UİManager ui)
    {
        var button=ui.startGameButton;
        foreach(var graphic in button.GetComponentsInChildren<Graphic>(true))graphic.enabled=false;
        foreach(var effect in button.GetComponentsInChildren<Shadow>(true))effect.enabled=false;
        var image=button.GetComponent<Image>();image.enabled=true;image.sprite=Resources.Load<Sprite>("UI/HudReference/StartGame");image.type=Image.Type.Simple;image.preserveAspect=true;image.color=Color.white;image.raycastTarget=true;button.targetGraphic=image;button.transition=Selectable.Transition.ColorTint;
        var colors=ColorBlock.defaultColorBlock;colors.highlightedColor=new Color(1.12f,1.12f,1.12f);colors.pressedColor=new Color(.8f,.8f,.8f);colors.selectedColor=Color.white;colors.disabledColor=new Color(.45f,.45f,.45f,.8f);button.colors=colors;
        Place((RectTransform)button.transform,20,180,210,100);
        Place((RectTransform)ui.speedButtons[0].transform.parent,20,288,210,82);
        var hearts=button.transform.parent.Find("HealBar") as RectTransform;if(hearts!=null)Place(hearts,28,376,150,36);
    }
    [MenuItem("Tools/Backpack Defense/Apply Reference Start Button")]
    public static string Apply()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play first.");
        string path=Folder+"StartGame.png";AssetDatabase.ImportAsset(path);var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Default;importer.alphaIsTransparency=true;importer.isReadable=true;importer.mipmapEnabled=false;importer.npotScale=TextureImporterNPOTScale.None;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=4096;importer.SaveAndReimport();
        if(AssetDatabase.LoadAssetAtPath<Sprite>(Folder+"StartGame.asset")==null)
        {
            var t=AssetDatabase.LoadAssetAtPath<Texture2D>(path);var p=t.GetPixels32();int l=t.width,r=0,b=t.height,top=0;
            for(int y=0;y<t.height;y++)for(int x=0;x<t.width;x++)if(p[y*t.width+x].a>25){l=Mathf.Min(l,x);r=Mathf.Max(r,x);b=Mathf.Min(b,y);top=Mathf.Max(top,y);}
            if(r<=l||top<=b)throw new InvalidOperationException("Start button sprite is empty.");
            var sprite=Sprite.Create(t,new Rect(l,b,r-l+1,top-b+1),Vector2.one*.5f,100,0,SpriteMeshType.FullRect);sprite.name="StartGame";AssetDatabase.CreateAsset(sprite,Folder+"StartGame.asset");
        }
        var scene=UnityEngine.SceneManagement.SceneManager.GetSceneByPath("Assets/Scenes/GameScane.unity");bool loaded=scene.isLoaded;if(!loaded)scene=EditorSceneManager.OpenScene("Assets/Scenes/GameScane.unity",OpenSceneMode.Additive);
        try{var ui=scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<UİManager>(true)).First();Style(ui);foreach(var c in scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<Component>(true)))if(c!=null)EditorUtility.SetDirty(c);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();}
        finally{if(!loaded)EditorSceneManager.CloseScene(scene,true);}
        return "Native Start button artwork replaced; original callbacks retained";
    }
}
