var ui=UnityEngine.Object.FindObjectOfType<UİManager>();ui.CloseTowerPanel();
var cam=UnityEngine.Camera.main;var plane=new UnityEngine.Plane(UnityEngine.Vector3.up,new UnityEngine.Vector3(0,2,0));
var paths=new[]{"Assets/Prefabs/ArcherBullet.prefab","Assets/Prefabs/MegicBullet.prefab","Assets/Prefabs/NewMap/MortarProjectile.prefab"};
var output=new System.Collections.Generic.List<string>();
for(int i=0;i<3;i++){
 var ray=cam.ViewportPointToRay(new UnityEngine.Vector3(.35f+.13f*i,.58f,0));float distance;plane.Raycast(ray,out distance);var point=ray.GetPoint(distance);
 var obj=UnityEngine.Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(paths[i]),point,UnityEngine.Quaternion.identity);obj.name="VisibilityPreview_"+i;
 var bullet=obj.GetComponent<Bullet>();if(bullet!=null){var target=new UnityEngine.GameObject("PreviewTarget");target.transform.position=point+UnityEngine.Vector3.right*10;bullet.Initialize(target.transform,0,0);}
 var renderer=obj.GetComponent<UnityEngine.Renderer>();output.Add(paths[i]+" world-size="+renderer.bounds.size+" screen="+cam.WorldToScreenPoint(point));
}
UnityEditor.EditorWindow.GetWindow(System.Type.GetType("UnityEditor.GameView,UnityEditor")).Focus();
return string.Join("\n",output);
