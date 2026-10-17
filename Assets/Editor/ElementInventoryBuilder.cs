using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ElementInventoryBuilder
{
    [MenuItem("Tools/Backpack Defense/Apply Element Inventory System")]
    public static void Apply()
    {
        if(EditorApplication.isPlaying)throw new System.InvalidOperationException("Exit Play Mode first.");
        string folder="Assets/Resources/Data/Items/Elements";
        if(!AssetDatabase.IsValidFolder(folder))AssetDatabase.CreateFolder("Assets/Resources/Data/Items","Elements");
        string[] names={"Fire","Water","Earth","Air"};
        string[] labels={"Ateş Topu","Su Küresi","Toprak Özü","Hava Küresi"};
        for(int i=0;i<names.Length;i++)
        {
            string image="Assets/Resources/UI/Elements/"+names[i]+".png";
            var importer=(TextureImporter)AssetImporter.GetAtPath(image);
            importer.textureType=TextureImporterType.Default;importer.mipmapEnabled=false;
            importer.npotScale=TextureImporterNPOTScale.None;importer.alphaIsTransparency=true;
            importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=512;
            importer.wrapMode=TextureWrapMode.Clamp;importer.SaveAndReimport();
            string spritePath="Assets/Resources/UI/Elements/"+names[i]+"Icon.asset";
            var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if(sprite==null)
            {
                var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(image);
                sprite=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),Vector2.one*.5f);
                sprite.name=names[i]+"Catalyst";AssetDatabase.CreateAsset(sprite,spritePath);
            }
            string path=folder+"/"+names[i]+".asset";var item=AssetDatabase.LoadAssetAtPath<ItemData>(path);
            if(item==null){item=ScriptableObject.CreateInstance<ItemData>();AssetDatabase.CreateAsset(item,path);}
            item.Name=labels[i];item.itemID="element_catalyst_"+names[i].ToLowerInvariant();
            item.itemShape=new List<Vector2Int>{Vector2Int.zero};item.centerOffset=Vector2Int.zero;
            item.isElementCatalyst=true;item.element=(ElementType)(i+1);item.Rarity=ItemRarity.Rare;
            item.itemOwners=new[]{TowerType.ArrowTower,TowerType.MagicTower,TowerType.CannonTower,TowerType.UnitTower};
            item.cost=80;item.Type=ItemType.Potion;item.itemIcon=sprite;EditorUtility.SetDirty(item);
        }
        AssetDatabase.SaveAssets();ApprovedUISceneBuilder.Apply();
    }
}
