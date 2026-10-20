var scene=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/UI/SceneLoadingUI.prefab");
var so=new SerializedObject(prefab.GetComponent<SceneLoadingUI>());
return new{playing=EditorApplication.isPlaying,dirty=scene.isDirty,path=scene.path,progress=so.FindProperty("progress").objectReferenceValue.name,status=so.FindProperty("status").objectReferenceValue.name,retry=so.FindProperty("retry").objectReferenceValue.name};
