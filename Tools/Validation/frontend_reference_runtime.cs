var checks=new System.Collections.Generic.List<object>();
System.Action<string,bool> check=(name,ok)=>checks.Add(new{test=name,passed=ok});
var menu=UnityEngine.Object.FindObjectOfType<MainMenuManager>();var ui=UnityEngine.Object.FindObjectOfType<SkillTreePanelUI>(true);
string original=UnityEngine.PlayerPrefs.GetString(SkillTreeProgress.SaveKey,""); SkillTreePanelUI.TryCloseOpen();SettingsPanelUI.TryCloseOpen();menu.ShowHome();Canvas.ForceUpdateCanvases();
var home=GameObject.Find("FrontendCanvas").transform.Find("SafeFrame/HomePanel");
check("main menu uses original supplied image",home.GetComponent<UnityEngine.UI.Image>().sprite.texture.name=="MainMenu");
check("old main menu artwork hidden",home.GetComponentsInChildren<UnityEngine.UI.RawImage>(true).All(x=>!x.enabled));
var buttons=new[]{"Play","SkillTree","Settings","Quit"}.Select(n=>home.Find(n).GetComponent<UnityEngine.UI.Button>()).ToArray();
check("four menu callbacks preserved",buttons.All(b=>b.onClick.GetPersistentEventCount()>0));
var pointer=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
bool hits=true;foreach(var b in buttons){pointer.position=RectTransformUtility.WorldToScreenPoint(null,((RectTransform)b.transform).TransformPoint(((RectTransform)b.transform).rect.center));var results=new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointer,results);hits&=results.Count>0&&results[0].gameObject.GetComponentInParent<UnityEngine.UI.Button>()==b;}
check("click targets align with all four illustrated buttons",hits);
buttons[1].onClick.Invoke();check("menu opens new skill screen",SkillTreePanelUI.IsOpen&&ui.GetComponent<SkillTreeReferenceSkin>()!=null);
var card=(RectTransform)ui.nodes[0].transform.parent;
check("skill background is persistent native sprite",card.GetComponent<UnityEngine.UI.Image>().sprite.texture.name=="SkillBackground"&&UnityEditor.EditorUtility.IsPersistent(card.GetComponent<UnityEngine.UI.Image>().sprite));
for(int branch=0;branch<ui.trees.Length;branch++)
{
 ui.tabs[branch].GetComponent<UnityEngine.UI.Button>().onClick.Invoke();var tree=ui.trees[branch];
 check(tree.title+" retains all data nodes",ui.branchTitle.text==tree.title&&ui.nodes.Count(n=>n.gameObject.activeSelf)==tree.nodes.Count);
 check(tree.title+" selected tab follows selection",ui.tabs[branch].sprite.name=="SelectedTab"&&ui.tabs.Where((x,i)=>i!=branch).All(x=>x.sprite.name=="Tab"));
 bool labels=true;for(int i=0;i<tree.nodes.Count;i++)labels&=ui.nodeLabels[i].text==tree.nodes[i].skill.displayName&&ui.nodeIcons[i].sprite==tree.nodes[i].skill.icon;
 check(tree.title+" shows live labels and icons",labels);
 int index=tree.nodes.Count-1;ui.nodes[index].GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
 check(tree.title+" node click updates details and selection",ui.detailTitle.text==tree.nodes[index].skill.displayName&&ui.detailBody.text==tree.nodes[index].skill.description&&ui.nodes[index].sprite.name=="SelectedNodeTransparent");
 check(tree.title+" keeps unlock conditions",ui.unlockButton.interactable==(new SkillTreeProgress().UnlockError(tree,tree.nodes[index].skill)==null));
}
ui.tabs[0].GetComponent<UnityEngine.UI.Button>().onClick.Invoke();ui.SelectNode(0);
card.Find("Close").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();check("close button returns to home",!SkillTreePanelUI.IsOpen&&home.gameObject.activeInHierarchy);
buttons[2].onClick.Invoke();check("settings menu still opens",SettingsPanelUI.TryCloseOpen());
buttons[0].onClick.Invoke();check("play opens existing chapter selection",!home.gameObject.activeSelf);menu.ShowHome();check("return from chapter selection restores reference home",home.gameObject.activeSelf);
check("skill save unchanged by presentation and navigation",original==UnityEngine.PlayerPrefs.GetString(SkillTreeProgress.SaveKey,""));
return checks;


