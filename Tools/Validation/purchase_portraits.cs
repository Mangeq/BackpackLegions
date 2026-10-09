if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Stop Play");var ui=UnityEngine.Object.FindObjectOfType<UİManager>();var prefabs=new[]{ui.MegicTowerPrefab,ui.ArcherTowerPrefab,ui.UnitTowerPrefab};var names=new[]{"Magic","Archer","Unit"};var sprites=new UnityEngine.Sprite[3];
for(int i=0;i<3;i++){
    var texture=UnityEditor.AssetPreview.GetAssetPreview(prefabs[i]);if(texture==null)throw new System.Exception("Preview still loading: "+names[i]);
    string path="Assets/Prefabs/UI/PurchaseArt/"+names[i]+".png";System.IO.File.WriteAllBytes(path,UnityEngine.ImageConversion.EncodeToPNG(texture));UnityEditor.AssetDatabase.ImportAsset(path);var importer=(UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath(path);importer.textureType=UnityEditor.TextureImporterType.Sprite;importer.alphaIsTransparency=true;importer.SaveAndReimport();sprites[i]=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Sprite>(path);
}
ui.magicPurchasePortrait=sprites[0];ui.archerPurchasePortrait=sprites[1];ui.unitPurchasePortrait=sprites[2];
var root=UnityEngine.GameObject.Find("TowerShopUI");var controls=new[]{"MegicTower","ArcherTower","UnitTower"};for(int i=0;i<3;i++){var icon=root.transform.Find("IconRail/"+controls[i]+"/PortraitIcon").GetComponent<UnityEngine.UI.Image>();icon.sprite=sprites[i];}
UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());return UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
