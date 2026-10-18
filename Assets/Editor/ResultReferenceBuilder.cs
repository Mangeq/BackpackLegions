using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ResultReferenceBuilder
{
    const string Folder="Assets/Resources/UI/ResultReference/";
    static void Import(string source)
    {
        AssetDatabase.ImportAsset(Folder+source+".png");var importer=(TextureImporter)AssetImporter.GetAtPath(Folder+source+".png");
        importer.textureType=TextureImporterType.Default;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.npotScale=TextureImporterNPOTScale.None;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=4096;importer.SaveAndReimport();
    }
    static void Piece(string name,string source,Rect rect)
    {
        if(AssetDatabase.LoadAssetAtPath<Sprite>(Folder+name+".asset")!=null)return;
        var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+source+".png");var sprite=Sprite.Create(texture,new Rect(rect.x/1024*texture.width,(572-rect.y-rect.height)/572*texture.height,rect.width/1024*texture.width,rect.height/572*texture.height),Vector2.one*.5f,100,0,SpriteMeshType.FullRect);sprite.name=name;AssetDatabase.CreateAsset(sprite,Folder+name+".asset");
    }
    [MenuItem("Tools/Backpack Defense/Apply Reference Result Panels")]
    public static void Apply()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play Mode first.");
        foreach(var source in new[]{"Pause","Victory","Defeat","VictoryClean"})Import(source);
        foreach(var source in new[]{"PauseCutout","VictoryCutout","DefeatCutout"})
        {Import(source);Piece(source,source,new Rect(0,0,1024,572));}
        Piece("Pause","Pause",new Rect(0,0,1024,572));Piece("Victory","Victory",new Rect(0,0,1024,572));Piece("Defeat","Defeat",new Rect(0,0,1024,572));
        Piece("PauseContinue","Pause",new Rect(344,197,347,77));Piece("PauseSkills","Pause",new Rect(344,284,347,78));Piece("PauseSettings","Pause",new Rect(344,371,347,77));Piece("PauseMenu","Pause",new Rect(344,458,347,76));
        Piece("VictoryNext","Victory",new Rect(348,323,337,69));Piece("VictoryNextComplete","VictoryClean",new Rect(348,323,337,69));Piece("VictorySkills","Victory",new Rect(348,397,337,69));Piece("VictoryRetry","Victory",new Rect(289,474,219,53));Piece("VictoryMenu","Victory",new Rect(523,474,219,53));
        Piece("DefeatRetry","Defeat",new Rect(380,321,272,63));Piece("DefeatSkills","Defeat",new Rect(380,386,272,62));Piece("DefeatMenu","Defeat",new Rect(390,452,252,50));
        Piece("PauseContinueCutout","PauseCutout",new Rect(344,197,347,77));Piece("PauseSkillsCutout","PauseCutout",new Rect(344,284,347,78));Piece("PauseSettingsCutout","PauseCutout",new Rect(344,371,347,77));Piece("PauseMenuCutout","PauseCutout",new Rect(344,458,347,76));
        Piece("VictoryNextCutout","VictoryCutout",new Rect(348,323,337,69));Piece("VictorySkillsCutout","VictoryCutout",new Rect(348,397,337,69));Piece("VictoryRetryCutout","VictoryCutout",new Rect(289,474,219,53));Piece("VictoryMenuCutout","VictoryCutout",new Rect(523,474,219,53));
        Piece("DefeatRetryCutout","DefeatCutout",new Rect(380,321,272,63));Piece("DefeatSkillsCutout","DefeatCutout",new Rect(380,386,272,62));Piece("DefeatMenuCutout","DefeatCutout",new Rect(390,452,252,50));
        var scene=EditorSceneManager.GetActiveScene();var game=UnityEngine.Object.FindObjectOfType<GameManager>();if(game==null)throw new InvalidOperationException("Open gameplay scene first.");
        var skin=game.GetComponent<ResultReferenceSkin>();if(skin==null)skin=game.gameObject.AddComponent<ResultReferenceSkin>();skin.Configure(game);
        game.pausePanel.SetActive(false);game.GameOverPanel.SetActive(false);game.victoryPanel.SetActive(false);
        foreach(var component in scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<Component>(true)))if(component!=null)EditorUtility.SetDirty(component);
        AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
    }
}
