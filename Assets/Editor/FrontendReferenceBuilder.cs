using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Applies approved raster art to existing controls without replacing game logic.</summary>
public static class FrontendReferenceBuilder
{
    const string Folder="Assets/Resources/UI/FrontendReference/";
    const string Sprites=Folder+"Sprites/";
    static void Import(string name)
    {
        var importer=(TextureImporter)AssetImporter.GetAtPath(Folder+name+".png");
        importer.textureType=TextureImporterType.Default;importer.mipmapEnabled=false;importer.npotScale=TextureImporterNPOTScale.None;
        importer.alphaIsTransparency=true;
        importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=4096;importer.wrapMode=TextureWrapMode.Clamp;importer.SaveAndReimport();
    }
    static void Crop(string name,string source,Rect rect)
    {
        if(AssetDatabase.LoadAssetAtPath<Sprite>(Sprites+name+".asset")!=null)return;
        var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+source+".png");
        var sprite=Sprite.Create(texture,new Rect(rect.x/1024*texture.width,(572-rect.y-rect.height)/572*texture.height,rect.width/1024*texture.width,rect.height/572*texture.height),Vector2.one*.5f,100,0,SpriteMeshType.FullRect);
        sprite.name="Reference_"+name;AssetDatabase.CreateAsset(sprite,Sprites+name+".asset");
    }
    static void Backup(string path)
    {
        string target="Docs/Validation/frontend-reference/"+Path.GetFileName(path)+".before.txt";
        Directory.CreateDirectory(Path.GetDirectoryName(target));if(!File.Exists(target))File.Copy(path,target);
    }
    static void NodeSprite(string name,Rect rect)
    {
        if(AssetDatabase.LoadAssetAtPath<Sprite>(Sprites+name+".asset")!=null)return;
        var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+"NodeFaces.png");
        var sprite=Sprite.Create(texture,rect,Vector2.one*.5f,100,0,SpriteMeshType.FullRect);
        sprite.name="Reference_"+name;AssetDatabase.CreateAsset(sprite,Sprites+name+".asset");
    }
    [MenuItem("Tools/Backpack Defense/Apply Reference Frontend")]
    public static void Apply()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play Mode first.");
        var original=EditorSceneManager.GetActiveScene();
        if(original.isDirty&&!EditorSceneManager.SaveScene(original))throw new IOException("Cannot save scene.");
        string returnPath=original.path;
        if(!AssetDatabase.IsValidFolder(Sprites.TrimEnd('/')))AssetDatabase.CreateFolder(Folder.TrimEnd('/'),"Sprites");
        Import("MainMenu");Import("SkillPieces");Import("SkillBackground");Import("NodeFaces");
        NodeSprite("NodeTransparent",new Rect(84,170,817,387));
        NodeSprite("SelectedNodeTransparent",new Rect(1279,168,816,389));
        Crop("MainMenu","MainMenu",new Rect(0,0,1024,572));
        Crop("SkillBackground","SkillBackground",new Rect(0,0,1024,572));
        Crop("Node","SkillPieces",new Rect(214,233,102,56));
        Crop("SelectedNode","SkillPieces",new Rect(399,485,102,56));
        Crop("Tab","SkillPieces",new Rect(20,201,156,65));
        Crop("SelectedTab","SkillPieces",new Rect(20,126,156,65));
        Crop("Close","SkillPieces",new Rect(894,31,111,35));
        Crop("Reset","SkillPieces",new Rect(30,498,136,28));
        Crop("UnlockLocked","SkillPieces",new Rect(745,461,239,36));
        string prefabPath="Assets/Prefabs/UI/SkillTreeUI.prefab";Backup(prefabPath);
        var contents=PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            var skill=contents.GetComponent<SkillTreePanelUI>();
            var skin=contents.GetComponent<SkillTreeReferenceSkin>();if(skin==null)skin=contents.AddComponent<SkillTreeReferenceSkin>();
            skin.Configure(skill);skill.modal.SetActive(false);PrefabUtility.SaveAsPrefabAsset(contents,prefabPath);
        }
        finally{PrefabUtility.UnloadPrefabContents(contents);}
        foreach(var path in new[]{"Assets/Scenes/MainMenu.unity","Assets/Scenes/GameScane.unity"})
        {
            Backup(path);var scene=EditorSceneManager.OpenScene(path);
            foreach(var skill in UnityEngine.Object.FindObjectsOfType<SkillTreePanelUI>(true))
            {
                var skin=skill.GetComponent<SkillTreeReferenceSkin>();if(skin==null)skin=skill.gameObject.AddComponent<SkillTreeReferenceSkin>();
                skin.Configure(skill);skill.modal.SetActive(false);
            }
            if(path.EndsWith("MainMenu.unity"))ApplyHome();
            foreach(var component in scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<Component>(true)))if(component!=null)EditorUtility.SetDirty(component);
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
        }
        AssetDatabase.SaveAssets();EditorSceneManager.OpenScene(returnPath);
    }
    static void ApplyHome()
    {
        var manager=UnityEngine.Object.FindObjectOfType<MainMenuManager>(true);
        var property=new SerializedObject(manager).FindProperty("homePanel");
        var home=((GameObject)property.objectReferenceValue).GetComponent<RectTransform>();
        foreach(var graphic in home.GetComponentsInChildren<Graphic>(true))graphic.enabled=false;
        foreach(var effect in home.GetComponentsInChildren<Shadow>(true))effect.enabled=false;
        var background=home.GetComponent<Image>();if(background==null)background=home.gameObject.AddComponent<Image>();
        background.enabled=true;background.sprite=SkillTreeReferenceSkin.Art("MainMenu");background.type=Image.Type.Simple;background.color=Color.white;background.raycastTarget=true;
        string[] names={"Play","SkillTree","Settings","Quit"};float[] ys={328,381,428,475};
        for(int i=0;i<names.Length;i++)
        {
            var button=home.Find(names[i]).GetComponent<Button>();var image=button.GetComponent<Image>();
            Crop("Menu"+names[i],"MainMenu",new Rect(49,ys[i],300,46));
            image.enabled=true;image.sprite=SkillTreeReferenceSkin.Art("Menu"+names[i]);image.type=Image.Type.Simple;image.color=Color.white;image.raycastTarget=true;button.targetGraphic=image;
            button.colors=ColorBlock.defaultColorBlock;
            SkillTreeReferenceSkin.Place((RectTransform)button.transform,home,49,ys[i],300,46);
        }
        home.anchorMin=home.anchorMax=home.pivot=Vector2.one*.5f;home.anchoredPosition=Vector2.zero;
        home.sizeDelta=new Vector2(1024,572);home.localScale=Vector3.one*1.875f;
    }
}
