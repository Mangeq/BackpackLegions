if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Play kapalı olmalı");
var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
if(scene.path!="Assets/Scenes/GameScane.unity")throw new System.Exception("Yanlış sahne");
UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene,"Backups/2026-10-08-xp/BeforeXP.unity",true);
var ui=UnityEngine.Object.FindObjectOfType<UİManager>();
var header=ui.towerPanelTitle.transform.parent;
if(header.Find("TowerExperience")!=null)throw new System.Exception("XP UI zaten var");
var font=ui.towerPanelStats.font;
System.Func<string,UnityEngine.Transform,float,float,float,float,UnityEngine.RectTransform> rect=(n,p,x,y,w,h)=>{var g=new UnityEngine.GameObject(n,typeof(UnityEngine.RectTransform));g.transform.SetParent(p,false);var r=g.GetComponent<UnityEngine.RectTransform>();r.anchorMin=r.anchorMax=r.pivot=new UnityEngine.Vector2(0,1);r.anchoredPosition=new UnityEngine.Vector2(x,-y);r.sizeDelta=new UnityEngine.Vector2(w,h);return r;};
System.Func<string,UnityEngine.Transform,float,float,float,float,int,UnityEngine.UI.Text> label=(n,p,x,y,w,h,f)=>{var r=rect(n,p,x,y,w,h);var t=r.gameObject.AddComponent<UnityEngine.UI.Text>();t.font=font;t.fontSize=f;t.color=new UnityEngine.Color(.9f,.91f,.88f);t.raycastTarget=false;t.horizontalOverflow=UnityEngine.HorizontalWrapMode.Wrap;t.verticalOverflow=UnityEngine.VerticalWrapMode.Truncate;return t;};
ui.towerPanelTitle.rectTransform.sizeDelta=new UnityEngine.Vector2(460,36);ui.towerPanelTitle.fontSize=27;
ui.towerPanelStats.rectTransform.sizeDelta=new UnityEngine.Vector2(460,90);ui.towerPanelStats.fontSize=16;
var xpRoot=rect("TowerExperience",header,500,16,478,125);
ui.towerExperienceText=label("XPLabel",xpRoot,0,0,478,26,20);ui.towerExperienceText.text="XP 0 / 20 · Normal";
var bg=rect("XPBar",xpRoot,0,32,454,8);bg.gameObject.AddComponent<UnityEngine.UI.Image>().color=new UnityEngine.Color(.08f,.12f,.15f);
var fill=rect("Fill",bg,0,0,0,0);fill.anchorMin=UnityEngine.Vector2.zero;fill.anchorMax=UnityEngine.Vector2.one;fill.offsetMin=fill.offsetMax=UnityEngine.Vector2.zero;
ui.towerExperienceFill=fill.gameObject.AddComponent<UnityEngine.UI.Image>();ui.towerExperienceFill.color=new UnityEngine.Color(.38f,.64f,.7f);ui.towerExperienceFill.raycastTarget=false;
ui.towerElementStatus=label("ElementStatus",xpRoot,0,48,470,25,16);ui.towerElementStatus.text="Element seçimi 5. seviyede açılır";
ui.elementButtons=new UnityEngine.UI.Button[4];
for(int i=0;i<4;i++){var r=rect("Element"+(i+1),xpRoot,i*116,82,106,33);var im=r.gameObject.AddComponent<UnityEngine.UI.Image>();im.color=new UnityEngine.Color(.16f,.22f,.24f);var button=r.gameObject.AddComponent<UnityEngine.UI.Button>();button.targetGraphic=im;UnityEditor.Events.UnityEventTools.AddIntPersistentListener(button.onClick,ui.SelectTowerElement,i+1);var t=label("Label",r,0,0,106,33,18);t.alignment=UnityEngine.TextAnchor.MiddleCenter;t.text=UİManager.ElementLabel((ElementType)(i+1));ui.elementButtons[i]=button;r.gameObject.SetActive(false);}
var rewards=new System.Collections.Generic.Dictionary<string,float>{{"Scout",20},{"Raider",25},{"Guard",30},{"SkyWisp",30},{"Elite",40},{"Warlord",150}};
foreach(var entry in rewards){var p="Assets/Resources/Data/EnemyData/Chapter01/"+entry.Key+".asset";var d=UnityEditor.AssetDatabase.LoadAssetAtPath<EnemyData>(p);d.experienceReward=entry.Value;UnityEditor.EditorUtility.SetDirty(d);}
UnityEditor.EditorUtility.SetDirty(ui);UnityEditor.AssetDatabase.SaveAssets();UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
return new{uiBound=ui.elementButtons.Length,xpRewardTypes=rewards.Count};
