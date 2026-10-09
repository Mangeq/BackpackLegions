if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Stop Play");
var bags=UnityEngine.Object.FindObjectOfType<BackpackUIManager>(true);var root=bags.combinedPanel.transform;
var storage=root.Find("Storage");var header=root.Find("TowerHeader");if(storage==null||header==null)throw new System.Exception("Inspect hierarchy first");
var font=root.Find("Help").GetComponent<UnityEngine.UI.Text>().font;
System.Func<string,UnityEngine.Transform,float,float,float,float,UnityEngine.RectTransform> make=(name,parent,x,y,w,h)=>{
    var child=parent.Find(name);var go=child!=null?child.gameObject:new UnityEngine.GameObject(name,typeof(UnityEngine.RectTransform));go.transform.SetParent(parent,false);var rt=go.GetComponent<UnityEngine.RectTransform>();rt.anchorMin=rt.anchorMax=rt.pivot=new UnityEngine.Vector2(0,1);rt.anchoredPosition=new UnityEngine.Vector2(x,-y);rt.sizeDelta=new UnityEngine.Vector2(w,h);return rt;};
System.Func<UnityEngine.RectTransform,string,int,UnityEngine.UI.Text> label=(rt,value,size)=>{var t=rt.GetComponent<UnityEngine.UI.Text>();if(t==null)t=rt.gameObject.AddComponent<UnityEngine.UI.Text>();t.text=value;t.font=font;t.fontSize=size;t.color=UnityEngine.Color.white;t.raycastTarget=false;return t;};
bags.evolutionStatus=label(make("EvolutionStatus",header,24,156,450,68),"EVRİM · Tarif eşyalarını yan yana yerleştir",15);
bags.deliveryLabel=label(make("DeliveryLabel",storage,18,557,190,32),"Teslim bekleyen yok",14);
bags.deliveryContainer=make("DeliveryItem",storage,207,551,42,38);
var next=make("NextDelivery",storage,253,554,30,32);var image=next.GetComponent<UnityEngine.UI.Image>();if(image==null)image=next.gameObject.AddComponent<UnityEngine.UI.Image>();image.color=new UnityEngine.Color(.23f,.28f,.31f);var button=next.GetComponent<UnityEngine.UI.Button>();if(button==null)button=next.gameObject.AddComponent<UnityEngine.UI.Button>();
if(button.onClick.GetPersistentEventCount()==0)UnityEditor.Events.UnityEventTools.AddPersistentListener(button.onClick,bags.NextDelivery);label(make("Label",next,5,0,25,32),">",22);
UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());return UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
