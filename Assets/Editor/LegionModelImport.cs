using UnityEditor;

public sealed class LegionModelImport : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        if (!assetPath.StartsWith("Assets/Resources/BlackpineLegion/") || !assetPath.EndsWith(".fbx")) return;
        var model = (ModelImporter)assetImporter;
        bool actor=!assetPath.Contains("/Effects/");
        model.animationType = actor ? ModelImporterAnimationType.Legacy : ModelImporterAnimationType.None;
        model.importAnimation = actor;
        model.useFileScale = true;
        model.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
        model.materialLocation = ModelImporterMaterialLocation.InPrefab;
    }
}
