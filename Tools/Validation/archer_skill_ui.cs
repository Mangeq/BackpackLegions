if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Stop Play first");
var path="Assets/Prefabs/UI/SkillTreeUI.prefab";var root=UnityEditor.PrefabUtility.LoadPrefabContents(path);
try{
var ui=root.GetComponent<SkillTreePanelUI>();var card=root.transform.Find("Modal/Card");
ui.previewConnections=card.Find("TreeBranches").gameObject;
var oldBranches=card.Find("ArcherBranches");if(oldBranches!=null)UnityEngine.Object.DestroyImmediate(oldBranches.gameObject);
var tree=new GameObject("ArcherBranches",typeof(RectTransform));tree.transform.SetParent(card,false);
var tr=tree.GetComponent<RectTransform>();tr.anchorMin=tr.anchorMax=tr.pivot=new Vector2(0,1);tr.anchoredPosition=Vector2.zero;tr.sizeDelta=new Vector2(1120,840);tree.transform.SetSiblingIndex(ui.nodes[0].transform.GetSiblingIndex());
ui.archerConnections=tree;
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

float[] xs={700,410,700,990,410,700,990,410,700,990,990};
float[] ys={790,675,675,675,550,550,550,400,400,440,320};
ui.previewPositions=ui.nodes.Select(n=>n.rectTransform.anchoredPosition).ToArray();ui.archerPositions=new Vector2[11];
for(int i=0;i<11;i++){
 ui.archerPositions[i]=new Vector2(xs[i]-73,-ys[i]+40);int parent=SkillTreeProgress.ArcherSkills[i].parent;
 if(parent>=0)curve(new Vector2(xs[parent],ys[parent]),new Vector2(xs[parent],ys[parent]-75),new Vector2(xs[i],ys[i]+65),new Vector2(xs[i],ys[i]),i<=3?7:4,.85f);
}
curve(new Vector2(700,835),new Vector2(697,820),new Vector2(702,800),new Vector2(700,790),9,.8f);
var detail=card.Find("Details");ui.unlockButton=detail.Find("PreviewOnly").GetComponent<UnityEngine.UI.Button>();ui.unlockLabel=ui.unlockButton.GetComponentInChildren<TMPro.TMP_Text>();ui.unlockButton.onClick=new UnityEngine.UI.Button.ButtonClickedEvent();UnityEditor.Events.UnityEventTools.AddPersistentListener(ui.unlockButton.onClick,ui.UnlockSelected);
ui.rulesLabel=detail.Find("RulesNote").GetComponent<TMPro.TMP_Text>();ui.rulesLabel.fontSize=14;ui.rulesLabel.rectTransform.sizeDelta=new Vector2(370,38);
ui.detailBody.fontSize=20;ui.detailBody.rectTransform.sizeDelta=new Vector2(370,164);
var reset=card.Find("ResetArcher");if(reset==null){reset=UnityEngine.Object.Instantiate(ui.unlockButton.gameObject,card).transform;reset.name="ResetArcher";}
ui.resetButton=reset.GetComponent<UnityEngine.UI.Button>();ui.resetButton.interactable=true;ui.resetButton.onClick=new UnityEngine.UI.Button.ButtonClickedEvent();UnityEditor.Events.UnityEventTools.AddPersistentListener(ui.resetButton.onClick,ui.ResetSelectedTree);
var rr=reset.GetComponent<RectTransform>();rr.anchoredPosition=new Vector2(40,-770);rr.sizeDelta=new Vector2(212,40);
var rt=reset.GetComponentInChildren<TMPro.TMP_Text>();rt.text="ÜCRETSİZ SIFIRLA";rt.fontSize=15;rt.rectTransform.anchorMin=Vector2.zero;rt.rectTransform.anchorMax=Vector2.one;rt.rectTransform.offsetMin=new Vector2(8,0);rt.rectTransform.offsetMax=new Vector2(-8,0);
card.Find("SharedHint").GetComponent<TMPro.TMP_Text>().text="Puanlar bütün kule türleri için ortaktır.";
card.Find("SharedHint").GetComponent<RectTransform>().anchoredPosition=new Vector2(44,-670);
card.Find("PreviewNotice").GetComponent<TMPro.TMP_Text>().text="Okçu yetenekleri etkin · Diğer türler taslak · Açılan yetenekler kalıcıdır.";
ui.branches[0].subtitle="Hız · Menzil · Kritik · Kalıcı yetenekler";
ui.SelectBranch(0);ui.modal.SetActive(false);UnityEditor.PrefabUtility.SaveAsPrefabAsset(root,path);
}finally{UnityEditor.PrefabUtility.UnloadPrefabContents(root);}return "Archer unlock, refund and 11-node paths installed.";

