var ui=UnityEngine.Object.FindObjectOfType<UİManager>(true);
var output=new System.Collections.Generic.List<string>();
foreach(var p in new[]{"Assets/Prefabs/ArcherBullet.prefab","Assets/Prefabs/MegicBullet.prefab","Assets/Prefabs/NewMap/MortarProjectile.prefab"})output.Add(p+" "+UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(p).transform.localScale);
foreach(var p in new[]{ui.ArcherTowerPrefab,ui.MegicTowerPrefab,ui.MortarTowerPrefab})output.Add(UnityEditor.AssetDatabase.GetAssetPath(p)+" scale="+p.transform.localScale);
foreach(var t in ui.transitionPanel.GetComponentsInChildren<UnityEngine.UI.Text>(true))output.Add(t.name+": "+t.text);
return string.Join("\n",output);
