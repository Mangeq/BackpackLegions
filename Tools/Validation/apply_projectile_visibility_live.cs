var output=new System.Collections.Generic.List<string>();
var paths=new[]{"Assets/Prefabs/ArcherBullet.prefab","Assets/Prefabs/MegicBullet.prefab","Assets/Prefabs/MortarProjectile.prefab","Assets/Prefabs/NewMap/MortarProjectile.prefab"};
var scales=new[]{.7f,.7f,.5f,2f};
for(int i=0;i<paths.Length;i++){var asset=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(paths[i]);asset.transform.localScale=UnityEngine.Vector3.one*scales[i];UnityEditor.EditorUtility.SetDirty(asset);output.Add(paths[i]+"="+asset.transform.localScale);}
var ui=UnityEngine.Object.FindObjectOfType<UİManager>(true);
foreach(var button in ui.transitionPanel.GetComponentsInChildren<UnityEngine.UI.Button>(true)){
 var label=button.GetComponentInChildren<UnityEngine.UI.Text>(true);if(label==null)continue;
 for(int i=0;i<button.onClick.GetPersistentEventCount();i++){
 var method=button.onClick.GetPersistentMethodName(i);
 if(method=="ConfirmTransition")label.text="Kabul et";
 if(method=="CloseTransition")label.text="Reddet";
 }
}
UnityEditor.AssetDatabase.SaveAssets();
UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(ui.gameObject.scene);
UnityEditor.SceneManagement.EditorSceneManager.SaveScene(ui.gameObject.scene);
return string.Join("\n",output);
