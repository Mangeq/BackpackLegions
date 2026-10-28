var ui=UnityEngine.Object.FindObjectOfType<UİManager>(true);var output=new System.Collections.Generic.List<string>();
foreach(var p in new[]{ui.ArcherTowerPrefab,ui.MegicTowerPrefab,ui.MortarTowerPrefab}){
 var archer=p.GetComponent<ArcherTower>();var magic=p.GetComponent<MegicTower>();var mortar=p.GetComponent<MortarTower>();
 var projectile=archer!=null?archer.ArcherBulletP:magic!=null?magic.MegicBulletP:mortar.projectilePrefab.gameObject;
 output.Add(p.name+" -> "+UnityEditor.AssetDatabase.GetAssetPath(projectile)+" "+projectile.transform.localScale);
 if(archer!=null)projectile.transform.localScale=UnityEngine.Vector3.one*1f;
 if(mortar!=null)projectile.transform.localScale=UnityEngine.Vector3.one*2.5f;
 if(archer!=null||mortar!=null)UnityEditor.EditorUtility.SetDirty(projectile);
}
var old=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>("Assets/Prefabs/MortarProjectile.prefab");old.transform.localScale=UnityEngine.Vector3.one*.65f;UnityEditor.EditorUtility.SetDirty(old);
UnityEditor.AssetDatabase.SaveAssets();return string.Join("\n",output);
