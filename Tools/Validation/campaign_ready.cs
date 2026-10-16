if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Unexpected Play mode.");
if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)throw new System.Exception("Unsaved scene.");
UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/Boot.unity");
return new {scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,playing=UnityEditor.EditorApplication.isPlaying,dirty=UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty};
