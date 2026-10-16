if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Stop Play first");
var path="Assets/Prefabs/UI/SkillTreeUI.prefab";var root=UnityEditor.PrefabUtility.LoadPrefabContents(path);
try{
 var ui=root.GetComponent<SkillTreePanelUI>();string[] names={"Archer","Magic","Mortar","Unit"};
 ui.trees=names.Select(n=>UnityEditor.AssetDatabase.LoadAssetAtPath<SkillTreeData>("Assets/Resources/Data/SkillTrees/"+n+".asset")).ToArray();
 foreach(var tree in ui.trees)if(tree==null||tree.ConfigurationError()!=null)throw new System.Exception(tree==null?"Missing tree":tree.ConfigurationError());
 root.transform.Find("Modal/Card/PreviewNotice").GetComponent<TMPro.TMP_Text>().text="Yetenekler SkillData üzerinden düzenlenir · Açılan yetenekler kalıcıdır.";
 UnityEditor.PrefabUtility.SaveAsPrefabAsset(root,path);
}finally{UnityEditor.PrefabUtility.UnloadPrefabContents(root);}
return new {skills=Resources.LoadAll<SkillData>("Data/Skills").Length,trees=Resources.LoadAll<SkillTreeData>("Data/SkillTrees").Length};
