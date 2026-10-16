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

 string[][] extra={
 new[]{"Keskin Uçlar","Uzak Atış","Hızlı Kiriş","Avcı Sezgisi","Delici Oklar","Kartal Gözü"},
 new[]{"Derin Mana","Büyü Odağı","Rün Ustası","Element Dengesi","Yoğun Büyü","Kadim Bilgi"},
 new[]{"Sağlam Namlu","Uzun Menzil","Barut İkmali","İsabet Ayarı","Şarapnel","Yıkım Ustası"},
 new[]{"Sağlam Saflar","Çelik Disiplin","Hızlı İkmal","Savunma Hattı","Savaş Sancağı","Lejyon Ustası"}};
 for(int b=0;b<ui.branches.Length;b++){
  var branch=ui.branches[b];System.Array.Resize(ref branch.names,12);System.Array.Resize(ref branch.descriptions,12);System.Array.Resize(ref branch.icons,12);
  for(int i=6;i<12;i++){branch.names[i]=extra[b][i-6];branch.descriptions[i]="Yeni uzmanlık düğümü önerisi. Etkisi, maliyeti ve açılma koşulu birlikte belirlenecek. Şimdilik puan harcanmaz.";branch.icons[i]=branch.icons[i-6];}
 }
 System.Array.Resize(ref ui.nodes,12);System.Array.Resize(ref ui.nodeIcons,12);System.Array.Resize(ref ui.nodeLabels,12);
 for(int i=6;i<12;i++){
  var go=UnityEngine.Object.Instantiate(ui.nodes[0].gameObject,card);go.name="Node"+i;
  var button=go.GetComponent<UnityEngine.UI.Button>();button.onClick=new UnityEngine.UI.Button.ButtonClickedEvent();UnityEditor.Events.UnityEventTools.AddIntPersistentListener(button.onClick,ui.SelectNode,i);
  ui.nodes[i]=go.GetComponent<UnityEngine.UI.Image>();ui.nodeIcons[i]=go.transform.Find("Icon").GetComponent<UnityEngine.UI.Image>();ui.nodeLabels[i]=go.transform.Find("Name").GetComponent<TMPro.TMP_Text>();
 }
 float[] xs={700,410,700,990,410,700,990,380,540,700,860,1020};
 float[] ys={780,645,645,645,500,500,500,350,350,350,350,350};
 int[] parents={-1,0,0,0,1,2,3,4,4,5,6,6};
 for(int i=0;i<12;i++){
  var r=ui.nodes[i].rectTransform;r.anchoredPosition=new Vector2(xs[i]-73,-ys[i]+40);r.sizeDelta=new Vector2(146,80);
  var icon=ui.nodeIcons[i].rectTransform;icon.anchoredPosition=new Vector2(7,-24);icon.sizeDelta=new Vector2(32,32);
  var label=ui.nodeLabels[i];label.rectTransform.anchoredPosition=new Vector2(44,-7);label.rectTransform.sizeDelta=new Vector2(96,66);label.fontSize=15;
  if(i>0){int parent=parents[i];curve(new Vector2(xs[parent],ys[parent]),new Vector2(xs[parent],ys[parent]-90),new Vector2(xs[i],ys[i]+80),new Vector2(xs[i],ys[i]),i<=3?7:4,.85f);}
 }
 curve(new Vector2(700,825),new Vector2(697,810),new Vector2(702,800),new Vector2(700,780),10,.8f);
 curve(new Vector2(700,815),new Vector2(680,824),new Vector2(660,827),new Vector2(640,828),3,.5f);
 curve(new Vector2(700,815),new Vector2(720,824),new Vector2(740,827),new Vector2(760,828),3,.5f);
 var pointObj=card.Find("Points");
 if(pointObj==null){pointObj=UnityEngine.Object.Instantiate(card.Find("Eyebrow").gameObject,card).transform;pointObj.name="Points";}
 ui.pointsLabel=pointObj.GetComponent<TMPro.TMP_Text>();ui.pointsLabel.text="YETENEK PUANI  0";
 ui.pointsLabel.rectTransform.anchoredPosition=new Vector2(1000,-54);ui.pointsLabel.rectTransform.sizeDelta=new Vector2(390,32);ui.pointsLabel.fontSize=20;
 card.Find("PreviewNotice").GetComponent<TMPro.TMP_Text>().text="Puanlar kalıcıdır · Yetenek etkileri ve maliyetleri henüz taslak; puan harcama kapalı.";
 ui.SelectBranch(0);
 UnityEditor.PrefabUtility.SaveAsPrefabAsset(root,path);
}finally{UnityEditor.PrefabUtility.UnloadPrefabContents(root);}
return "12 nodes per type and permanent points label installed.";
