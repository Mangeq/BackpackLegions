if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Stop Play first");
var path="Assets/Prefabs/UI/SkillTreeUI.prefab";
var root=UnityEditor.PrefabUtility.LoadPrefabContents(path);
try {
 var ui=root.GetComponent<SkillTreePanelUI>();var card=root.transform.Find("Modal/Card");
 var old=new System.Collections.Generic.List<GameObject>();
 foreach(Transform child in card)if(child.name=="BranchLink"||child.name=="TreeBranches")old.Add(child.gameObject);
 foreach(var go in old)UnityEngine.Object.DestroyImmediate(go);
 var tree=new GameObject("TreeBranches",typeof(RectTransform));tree.transform.SetParent(card,false);
 var tr=tree.GetComponent<RectTransform>();tr.anchorMin=tr.anchorMax=tr.pivot=new Vector2(0,1);tr.anchoredPosition=Vector2.zero;tr.sizeDelta=new Vector2(1120,830);tree.transform.SetSiblingIndex(ui.nodes[0].transform.GetSiblingIndex());
 // Curves are static UI geometry. They do not imply unlock prerequisites.
 System.Action<Vector2,Vector2,Vector2,Vector2,float,float> curve=(a,b,c,d,width,alpha)=>{
  Vector2 previous=a;
  for(int s=1;s<=20;s++){
   float t=s/20f,u=1-t;Vector2 next=u*u*u*a+3*u*u*t*b+3*u*t*t*c+t*t*t*d;
   var go=new GameObject("BranchSegment",typeof(RectTransform),typeof(UnityEngine.UI.Image));go.transform.SetParent(tree.transform,false);
   var r=go.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,.5f);r.anchoredPosition=new Vector2(previous.x,-previous.y);
   r.sizeDelta=new Vector2(Vector2.Distance(previous,next)+1,Mathf.Lerp(width,width*.5f,t));r.localRotation=Quaternion.Euler(0,0,-Mathf.Atan2(next.y-previous.y,next.x-previous.x)*Mathf.Rad2Deg);
   var im=go.GetComponent<UnityEngine.UI.Image>();im.color=new Color(.58f,.49f,.32f,alpha);im.raycastTarget=false;previous=next;
  }
 };
 curve(new Vector2(710,818),new Vector2(690,760),new Vector2(725,695),new Vector2(700,650),12,.85f);
 curve(new Vector2(704,685),new Vector2(655,630),new Vector2(525,660),new Vector2(490,570),8,.85f);
 curve(new Vector2(706,680),new Vector2(765,635),new Vector2(875,665),new Vector2(900,570),8,.85f);
 curve(new Vector2(490,570),new Vector2(480,490),new Vector2(395,490),new Vector2(400,370),5,.85f);
 curve(new Vector2(490,565),new Vector2(520,485),new Vector2(655,485),new Vector2(670,350),5,.85f);
 curve(new Vector2(900,570),new Vector2(905,485),new Vector2(1005,490),new Vector2(1000,370),5,.85f);
 // Small tapered twigs and roots give the silhouette a tree shape.
 curve(new Vector2(702,796),new Vector2(675,807),new Vector2(645,809),new Vector2(625,815),4,.45f);
 curve(new Vector2(704,796),new Vector2(735,801),new Vector2(760,812),new Vector2(785,813),4,.45f);
 curve(new Vector2(451,489),new Vector2(412,480),new Vector2(364,464),new Vector2(350,432),3,.45f);
 curve(new Vector2(588,469),new Vector2(584,437),new Vector2(570,411),new Vector2(548,400),3,.45f);
 curve(new Vector2(952,487),new Vector2(990,490),new Vector2(1050,465),new Vector2(1064,431),3,.45f);
 float[] xs={700,490,900,400,670,1000},ys={745,570,570,370,350,370};
 for(int i=0;i<6;i++){
  var r=ui.nodes[i].rectTransform;r.anchoredPosition=new Vector2(xs[i]-92,-ys[i]+45);
 }
 ui.SelectBranch(0);
 UnityEditor.PrefabUtility.SaveAsPrefabAsset(root,path);
}finally{UnityEditor.PrefabUtility.UnloadPrefabContents(root);}
return "Rooted branching tree saved; six nodes and all listeners preserved.";
