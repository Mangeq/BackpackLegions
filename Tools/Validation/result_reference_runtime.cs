var rows=new System.Collections.Generic.List<object>();System.Action<string,bool> check=(name,ok)=>rows.Add(new{test=name,passed=ok});
var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.enabled=false;var skin=gm.GetComponent<ResultReferenceSkin>();var waves=UnityEngine.Object.FindObjectOfType<WaveManager>();
string key=SkillTreeProgress.SaveKey;bool had=PlayerPrefs.HasKey(key);string saved=PlayerPrefs.GetString(key,"");
System.Func<GameObject,string,UnityEngine.UI.Button> button=(root,name)=>root.GetComponentsInChildren<UnityEngine.UI.Button>(true).First(b=>b.name==name);
System.Action<GameManager.GameState> state=s=>typeof(GameManager).GetProperty("CurrentState").SetValue(gm,s,null);
System.Func<GameObject,bool> raycasts=root=>{Canvas.ForceUpdateCanvases();bool ok=true;foreach(var b in root.GetComponentsInChildren<UnityEngine.UI.Button>(true)){if(!b.gameObject.activeInHierarchy||!b.interactable)continue;var rect=(RectTransform)b.transform;var ev=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){position=RectTransformUtility.WorldToScreenPoint(null,rect.TransformPoint(rect.rect.center))};var hits=new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();UnityEngine.EventSystems.EventSystem.current.RaycastAll(ev,hits);ok&=hits.Count>0&&hits[0].gameObject.GetComponentInParent<UnityEngine.UI.Button>()==b;}return ok;};
try{
 gm.TogglePause();skin.RefreshPresentation();check("pause opens approved original image",gm.IsPaused&&gm.pausePanel.activeInHierarchy&&gm.pausePanel.GetComponent<UnityEngine.UI.Image>().sprite.texture.name=="Pause");
 button(gm.pausePanel,"SkillTreeButton").onClick.Invoke();check("pause skill button opens skill tree",SkillTreePanelUI.IsOpen&&gm.IsPaused);SkillTreePanelUI.TryCloseOpen();
 button(gm.pausePanel,"SettingsButton").onClick.Invoke();check("pause settings button works",SettingsPanelUI.IsOpen&&gm.IsPaused);SettingsPanelUI.TryCloseOpen();
 button(gm.pausePanel,"CountineButton").onClick.Invoke();check("continue restores preparation without starting combat",!gm.IsPaused&&!gm.pausePanel.activeSelf&&gm.CurrentState==GameManager.GameState.Preparation&&Time.timeScale==0);
 GameManager.GameOver(false);skin.RefreshPresentation();check("defeat opens only approved defeat panel",gm.GameOverPanel.activeInHierarchy&&!gm.victoryPanel.activeSelf&&!gm.pausePanel.activeSelf&&gm.GameOverPanel.GetComponent<UnityEngine.UI.Image>().sprite.texture.name=="Defeat");
 check("defeat banner shows actual chapter",gm.GameOverPanel.transform.Find("LiveChapter").GetComponent<TMPro.TMP_Text>().text=="BÖLÜM "+waves.ActiveChapter.chapterNumber.ToString("00")+" · "+waves.ActiveChapter.chapterName);
 button(gm.GameOverPanel,"SkillTree").onClick.Invoke();check("defeat skill button opens tree",SkillTreePanelUI.IsOpen);SkillTreePanelUI.TryCloseOpen();
 state(GameManager.GameState.Preparation);gm.GameOverPanel.SetActive(false);GameManager.GameOver(true);skin.RefreshPresentation();
 check("victory opens only approved original victory image",gm.victoryPanel.activeInHierarchy&&!gm.GameOverPanel.activeSelf&&gm.victoryPanel.GetComponent<UnityEngine.UI.Image>().sprite.texture.name=="Victory");
 check("next chapter is available before final chapter",gm.nextChapterButton.interactable);
 button(gm.victoryPanel,"SkillTree").onClick.Invoke();check("victory skill button opens tree",SkillTreePanelUI.IsOpen);SkillTreePanelUI.TryCloseOpen();
 var active=waves.ActiveChapter;typeof(WaveManager).GetProperty("ActiveChapter").SetValue(waves,waves.chapters.Last(),null);state(GameManager.GameState.Preparation);GameManager.GameOver(true);skin.RefreshPresentation();
 check("final chapter disables next and changes caption",!gm.nextChapterButton.interactable&&gm.nextChapterButton.GetComponentInChildren<TMPro.TMP_Text>(true).text=="TÜM BÖLÜMLER TAMAMLANDI"&&gm.nextChapterButton.GetComponent<UnityEngine.UI.Image>().sprite.name=="VictoryNextComplete");
 typeof(WaveManager).GetProperty("ActiveChapter").SetValue(waves,active,null);state(GameManager.GameState.Preparation);gm.victoryPanel.SetActive(false);gm.GameOverPanel.SetActive(false);gm.TogglePause();skin.RefreshPresentation();
 check("legacy retry and menu identities retained for game bindings",button(gm.GameOverPanel,"NewGameButton")!=null&&button(gm.GameOverPanel,"BackMainMenuButton")!=null&&gm.pausePanel.transform.Find("CountineButton")!=null&&gm.pausePanel.transform.Find("MainMenuButton")!=null);
}finally{if(had)PlayerPrefs.SetString(key,saved);else PlayerPrefs.DeleteKey(key);PlayerPrefs.Save();}
check("player skill progress restored after validation",PlayerPrefs.GetString(key,"")==saved);
return rows;

