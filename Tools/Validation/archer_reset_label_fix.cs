if(UnityEditor.EditorApplication.isPlaying||UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty) return "STOP: Play or unsaved scene";
var path="Assets/Prefabs/UI/SkillTreeUI.prefab";var root=UnityEditor.PrefabUtility.LoadPrefabContents(path);
try{var r=root.transform.Find("Modal/Card/ResetArcher").GetComponentInChildren<TMPro.TMP_Text>(true).rectTransform;r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(8,0);r.offsetMax=new Vector2(-8,0);UnityEditor.PrefabUtility.SaveAsPrefabAsset(root,path);}finally{UnityEditor.PrefabUtility.UnloadPrefabContents(root);}
UnityEditor.AssetDatabase.Refresh();return "Reset label aligned";
