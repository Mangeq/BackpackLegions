if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Stop Play first");
if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)throw new System.Exception("Unsaved scene");
UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/GameScane.unity");
return new {castle=UnityEngine.Object.FindObjectOfType<Base>().maxHealth,slots=UnityEngine.Object.FindObjectsOfType<TowerSlot>().Select(s=>new{s.name,x=s.transform.position.x,y=s.transform.position.y,z=s.transform.position.z}).ToArray(),units=UnityEditor.AssetDatabase.FindAssets("t:Prefab",new[]{"Assets/Prefabs"}).Select(g=>UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(UnityEditor.AssetDatabase.GUIDToAssetPath(g))).Where(p=>p.GetComponent<UnitTower>()!=null).Select(p=>new{path=UnityEditor.AssetDatabase.GetAssetPath(p),p.GetComponent<UnitTower>().spawnCooldown,p.GetComponent<UnitTower>().maxUnitCount,health=p.GetComponent<UnitTower>().UnitPrefab.GetComponent<Unit>().health}).ToArray()};

