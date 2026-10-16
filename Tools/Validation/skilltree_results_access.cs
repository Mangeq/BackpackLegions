if(UnityEditor.EditorApplication.isPlaying||UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)throw new System.Exception("Stop and save first");
var scene=UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/GameScane.unity");
var gm=UnityEngine.Object.FindObjectOfType<GameManager>();
foreach(var panel in new[]{gm.victoryPanel,gm.GameOverPanel}){
 var card=panel.transform.Find("Card");if(card==null)throw new System.Exception("Missing card");
 card.GetComponent<RectTransform>().sizeDelta=new Vector2(1000,760);
 var summary=card.Find("Summary").GetComponent<TMPro.TMP_Text>();summary.fontSize=23;summary.rectTransform.anchoredPosition=new Vector2(48,-258);summary.rectTransform.sizeDelta=new Vector2(904,210);
 var skill=card.Find("SkillTree");
 if(skill==null){var template=card.GetComponentsInChildren<UnityEngine.UI.Button>(true)[0];skill=UnityEngine.Object.Instantiate(template.gameObject,card).transform;skill.name="SkillTree";}
 var r=skill.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(48,-580);r.sizeDelta=new Vector2(904,62);
 var b=skill.GetComponent<UnityEngine.UI.Button>();b.onClick=new UnityEngine.UI.Button.ButtonClickedEvent();UnityEditor.Events.UnityEventTools.AddPersistentListener(b.onClick,gm.OpenSkillTree);b.interactable=true;
 b.GetComponent<UnityEngine.UI.Image>().color=new Color(.23f,.34f,.27f);var label=b.GetComponentInChildren<TMPro.TMP_Text>();label.text="YETENEK AĞACI";label.color=new Color(.94f,.91f,.82f);
 foreach(var button in card.GetComponentsInChildren<UnityEngine.UI.Button>(true)){
  if(button==b)continue;var rect=button.GetComponent<RectTransform>();
  if(button.name=="NextChapter"||button.name=="NewGameButton")rect.anchoredPosition=new Vector2(48,-490);
  else rect.anchoredPosition=new Vector2(rect.anchoredPosition.x,-666);
 }
}
UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
return "Victory and defeat skill-tree access saved.";
