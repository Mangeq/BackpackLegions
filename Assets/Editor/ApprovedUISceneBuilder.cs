using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Persists the approved UI hierarchy and its generated render assets.</summary>
public static class ApprovedUISceneBuilder
{
    const string ArtFolder="Assets/Resources/UI/ApprovedScene";

    [MenuItem("Tools/Backpack Defense/Apply Approved UI To Scene")]
    public static void Apply()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play Mode first.");
        var scene=EditorSceneManager.GetActiveScene();
        var ui=UnityEngine.Object.FindObjectOfType<UİManager>(true);
        var bags=UnityEngine.Object.FindObjectOfType<BackpackUIManager>(true);
        var shop=UnityEngine.Object.FindObjectOfType<ShopUIManager>(true);
        if(ui==null||bags==null||shop==null)throw new InvalidOperationException("Open the gameplay scene first.");
        // Keep the user's current scene contents, including previously unsaved edits.
        var backup="Docs/Validation/ui-scene-persistence/before.unity.txt";
        if(!File.Exists(backup))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(backup));
            if(!EditorSceneManager.SaveScene(scene))throw new IOException("Cannot save the current scene.");
            File.Copy(scene.path,backup);
        }
        var purchase=ui.purchaseDetails.transform.parent.gameObject;
        SetReference(ui,"backpackUI",bags);
        SetReference(ui,"itemShopUI",shop);
        SetReference(ui,"towerShopPanel",purchase);
        SetReference(ui,"TowerShopBuyBTN",ui.purchasePrice.transform.parent.GetComponent<Button>());
        foreach(var name in new[]{"UnitTowerGold","MegicTowerGold","ArcherTowerGold"})
            SetReference(ui,name,purchase.GetComponentsInChildren<Text>(true).First(x=>x.name==name));
        var itemShop=scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<Transform>(true))
            .First(x=>x.name=="İtemShopUİ").gameObject;
        SetReference(shop,"itemShopPanel",itemShop);
        UİManager.TowerShopUI=purchase;
        TowerPanelLayout.Install(bags);
        TowerPurchaseSkin.Install(ui);
        bags.GetComponent<TowerPanelLayout>().ApplyInventoryRulesLayout();
        EnsureFolder(ArtFolder);
        var roots=new[]{bags.combinedPanel,purchase};
        foreach(var root in roots)
        {
            foreach(var text in root.GetComponentsInChildren<TMP_Text>(true))PersistFont(text.font);
            foreach(var image in root.GetComponentsInChildren<Image>(true))PersistSprite(image.sprite);
        }
        foreach(var slot in shop.shopSlots)if(slot.currentItem==null)slot.image.enabled=false;
        // Closed panels are saved closed, with their complete new visuals intact.
        ui.purchasePortrait.enabled=false;
        ui.towerPurchaseFeedback.enabled=false;
        foreach(var h in new[]{ui.magicSelectionHighlight,ui.cannonSelectionHighlight,ui.archerSelectionHighlight,ui.unitSelectionHighlight})h.SetActive(false);
        bags.backpackPanel.SetActive(false);bags.combinedPanel.SetActive(false);
        itemShop.SetActive(false);purchase.SetActive(false);
        foreach(var root in scene.GetRootGameObjects())
            foreach(var component in root.GetComponentsInChildren<Component>(true))
                if(component!=null)EditorUtility.SetDirty(component);
        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(scene);
        if(!EditorSceneManager.SaveScene(scene))throw new IOException("Cannot save the approved UI.");
    }
    static void SetReference(UnityEngine.Object owner,string name,UnityEngine.Object value)
    {
        var serialized=new SerializedObject(owner);
        serialized.FindProperty(name).objectReferenceValue=value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
    static void EnsureFolder(string path)
    {
        if(AssetDatabase.IsValidFolder(path))return;
        string parent=Path.GetDirectoryName(path).Replace('\\','/');EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent,Path.GetFileName(path));
    }
    static void PersistSprite(Sprite sprite)
    {
        if(sprite==null||EditorUtility.IsPersistent(sprite))return;
        string path=ArtFolder+"/"+sprite.name+".asset";
        if(AssetDatabase.LoadMainAssetAtPath(path)!=null)
            throw new InvalidOperationException("A different generated sprite already uses "+path);
        sprite.hideFlags=HideFlags.None;AssetDatabase.CreateAsset(sprite,path);
    }
    static void PersistFont(TMP_FontAsset font)
    {
        if(font==null||EditorUtility.IsPersistent(font))return;
        if(string.IsNullOrEmpty(font.name))font.name="Approved_"+(font.sourceFontFile!=null?font.sourceFontFile.name:"Body");
        string path=ArtFolder+"/"+font.name+".asset";
        font.hideFlags=HideFlags.None;AssetDatabase.CreateAsset(font,path);
        var material=font.material;
        if(material!=null&&!EditorUtility.IsPersistent(material))
        {material.hideFlags=HideFlags.None;AssetDatabase.AddObjectToAsset(material,font);}
        foreach(var atlas in font.atlasTextures)
            if(atlas!=null&&!EditorUtility.IsPersistent(atlas))
            {atlas.hideFlags=HideFlags.None;AssetDatabase.AddObjectToAsset(atlas,font);}
        EditorUtility.SetDirty(font);
    }
}
