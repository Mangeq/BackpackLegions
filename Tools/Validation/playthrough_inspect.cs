var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
return Newtonsoft.Json.JsonConvert.SerializeObject(new{playing=UnityEditor.EditorApplication.isPlaying,dirty=scene.isDirty,scene=scene.path,background=UnityEditor.PlayerSettings.runInBackground,save=UnityEngine.PlayerPrefs.GetString(SkillTreeProgress.SaveKey,"")});
