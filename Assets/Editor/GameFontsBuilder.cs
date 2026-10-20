using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class GameFontsBuilder
{
    const string Folder="Assets/Resources/UI/Fonts/";
    static TMP_FontAsset Bake(string name,string source,string family)
    {
        var old=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(Folder+name+".asset");if(old!=null)return old;
        AssetDatabase.ImportAsset(Folder+source+".ttf");var font=AssetDatabase.LoadAssetAtPath<Font>(Folder+source+".ttf");
        var asset=TMP_FontAsset.CreateFontAsset(font,80,8,GlyphRenderMode.SDFAA,2048,2048,AtlasPopulationMode.Dynamic,false);asset.name=name;
        string missing;string characters=File.ReadAllText("ArtSource/Fonts/"+family+"-Characters.txt");asset.TryAddCharacters(characters,out missing);
        if(!string.IsNullOrEmpty(missing))throw new InvalidOperationException("Font atlas missing characters: "+family+" "+missing);
        asset.atlasPopulationMode=AtlasPopulationMode.Static;asset.isMultiAtlasTexturesEnabled=false;
        asset.fallbackFontAssetTable=new System.Collections.Generic.List<TMP_FontAsset>{TMP_Settings.defaultFontAsset};
        var face=asset.faceInfo;face.scale=1;asset.faceInfo=face;
        AssetDatabase.CreateAsset(asset,Folder+name+".asset");asset.material.name=name+" Material";AssetDatabase.AddObjectToAsset(asset.material,asset);
        foreach(var texture in asset.atlasTextures){texture.name=name+" Atlas";AssetDatabase.AddObjectToAsset(texture,asset);EditorUtility.SetDirty(texture);}
        EditorUtility.SetDirty(asset);EditorUtility.SetDirty(asset.material);return asset;
    }
    static bool IsTitle(string value)
    {value=(value??"").ToLowerInvariant();return value.Contains("title")||value.Contains("serif")||value.Contains("heading")||value.Contains("cinzel")||value.Contains("branchname")||value=="referencename"||value=="referencenumber";}
    static bool Heading(Component component,TMP_FontAsset previous=null)
    {
        string role=component.name.ToLowerInvariant();
        if(role.Contains("subtitle")||role.Contains("description")||role.Contains("body")||role.Contains("value")||role.StartsWith("referencedetail")||role=="referencegold"||role=="referencewave"||role=="referencecountdown")return false;
        if(component.GetComponentInParent<Button>()!=null)return true;
        if(IsTitle(component.name))return true;
        return previous!=null&&IsTitle(previous.name);
    }
    static int Style(GameObject root)
    {
        int count=0;
        foreach(var t in root.GetComponentsInChildren<TMP_Text>(true))
        {
            var font=Heading(t,t.font)?GameFonts.Title:GameFonts.Body;t.font=font;t.fontSharedMaterial=font.material;
            if(font==GameFonts.Title){t.fontStyle&=~FontStyles.Bold;t.fontWeight=FontWeight.Regular;}
            t.SetAllDirty();EditorUtility.SetDirty(t);count++;
        }
        foreach(var t in root.GetComponentsInChildren<UnityEngine.UI.Text>(true))
        {bool heading=Heading(t)||t.font!=null&&IsTitle(t.font.name);t.font=heading?GameFonts.TitleSource:GameFonts.BodySource;if(heading)t.fontStyle=FontStyle.Normal;EditorUtility.SetDirty(t);count++;}
        // Skins also hold fonts for labels that they create when a panel opens.
        foreach(var c in root.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if(c==null||c is TMP_Text)continue;var so=new SerializedObject(c);var p=so.GetIterator();bool changed=false;
            while(p.NextVisible(true))if(p.propertyType==SerializedPropertyType.ObjectReference)
            {
                if(p.objectReferenceValue is TMP_FontAsset){var old=(TMP_FontAsset)p.objectReferenceValue;if(old==GameFonts.Title||old==GameFonts.Body)continue;p.objectReferenceValue=IsTitle(p.name)||IsTitle(old.name)?GameFonts.Title:GameFonts.Body;changed=true;}
                else if(p.objectReferenceValue is Font){var old=(Font)p.objectReferenceValue;if(old==GameFonts.TitleSource||old==GameFonts.BodySource)continue;p.objectReferenceValue=IsTitle(p.name)||IsTitle(old.name)?GameFonts.TitleSource:GameFonts.BodySource;changed=true;}
            }
            if(changed){so.ApplyModifiedPropertiesWithoutUndo();EditorUtility.SetDirty(c);}
        }
        return count;
    }
    static void Backup(string path)
    {var target="Docs/Validation/game-fonts/"+Path.GetFileName(path)+".before.txt";if(!File.Exists(target))File.Copy(path,target);}
    [MenuItem("Tools/Backpack Defense/Apply Cinzel and Alegreya Fonts")]
    public static object Apply()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play first.");
        Directory.CreateDirectory("Docs/Validation/game-fonts");
        Bake("CinzelBold SDF","Cinzel-Bold","Cinzel");Bake("AlegreyaMedium SDF","Alegreya-Medium","Alegreya");AssetDatabase.SaveAssets();
        int prefabLabels=0,sceneLabels=0;
        var prefabs=AssetDatabase.FindAssets("t:Prefab",new[]{"Assets/Prefabs/UI","Assets/Resources/UI"}).Select(AssetDatabase.GUIDToAssetPath).ToArray();
        foreach(var path in prefabs.Where(File.Exists)){Backup(path);var root=PrefabUtility.LoadPrefabContents(path);try{prefabLabels+=Style(root);PrefabUtility.SaveAsPrefabAsset(root,path);}finally{PrefabUtility.UnloadPrefabContents(root);}}
        var scenes=new[]{"Assets/Scenes/Boot.unity","Assets/Scenes/MainMenu.unity","Assets/Scenes/GameScane.unity",EditorSceneManager.GetActiveScene().path}.Where(p=>!string.IsNullOrEmpty(p)).Distinct();
        foreach(var path in scenes)
        {
            Backup(path);var scene=UnityEngine.SceneManagement.SceneManager.GetSceneByPath(path);bool loaded=scene.isLoaded;if(!loaded)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);
            try{foreach(var root in scene.GetRootGameObjects())sceneLabels+=Style(root);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);}
            finally{if(!loaded)EditorSceneManager.CloseScene(scene,true);}
        }
        AssetDatabase.SaveAssets();return new{prefabLabels=prefabLabels,sceneLabels=sceneLabels,title=GameFonts.Title.faceInfo.familyName,body=GameFonts.Body.faceInfo.familyName};
    }
}
