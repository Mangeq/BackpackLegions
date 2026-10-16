using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject homePanel;
    [SerializeField] private GameObject chapterPanel;
    [SerializeField] private SettingsPanelUI settingsPanel;
    [SerializeField] private SkillTreePanelUI skillTreePanel;
    [SerializeField] private TMPro.TMP_Text chapterTitle;
    [SerializeField] private TMPro.TMP_Text chapterDescription;
    [SerializeField] private TMPro.TMP_Text chapterStatus;
    [SerializeField] private UnityEngine.UI.Button chapterPlayButton;
    [SerializeField] private UnityEngine.UI.Image[] chapterMarkers;
    private int selectedChapter;
    [SerializeField] private ChapterData[] chapters;

    private void Start()
    {
        Time.timeScale = 1f;
        ShowHome();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape) || SceneLoadingUI.IsLoading) return;
        if (SkillTreePanelUI.TryCloseOpen() || SettingsPanelUI.TryCloseOpen()) return;
        if (chapterPanel != null && chapterPanel.activeSelf) ShowHome();
    }

    public void StartGame()
    {
        if (SceneLoadingUI.IsLoading) return;
        homePanel.SetActive(false);
        chapterPanel.SetActive(true);
        SelectChapter(0);
    }

    public void ShowHome()
    {
        if (homePanel != null) homePanel.SetActive(true);
        if (chapterPanel != null) chapterPanel.SetActive(false);
    }

    public void OpenSkillTree() { if(skillTreePanel!=null)skillTreePanel.Open(); }

    public void OpenSettings() => settingsPanel.Open();

    public void SelectChapter(int index)
    {
        selectedChapter = Mathf.Clamp(index, 0, chapters != null && chapters.Length > 0 ? chapters.Length-1 : 4);
        var chapter = chapters != null && selectedChapter < chapters.Length ? chapters[selectedChapter] : null;
        string[] titles = { "KARAÇAM GEÇİDİ", "ESKİ TAŞ KÖPRÜ", "SİSLİ SIRTLAR", "KÜL VADİSİ", "KUZEY KALESİ" };
        chapterTitle.text = chapter != null ? chapter.chapterName : titles[selectedChapter];
        chapterDescription.text = chapter != null
            ? chapter.waves.Count + " dalga · " + chapter.TotalEnemies + " düşman\nBaşlangıç: " + chapter.startingGold + " altın\n\nKarışık ve elementli düşmanlar.\nSon dalgada bölüm sonu düşmanı.\n\nHazırlık: " + chapter.preparationSeconds + " sn\nHer dalgada +" + chapter.extraPreparationPerWave + " sn"
            : selectedChapter == 0
            ? "Orman yolunu savun. Kulelerini kur, çantanı düzenle ve geçidi koru.\n\n5 dalga · 3. dalgada uçan düşmanlar\nSon dalgada bölüm sonu düşmanı"
            : "Bu bölge henüz oynanabilir değil.\n\nYeni yollar ve düşmanlar sonraki bölümlerde açılacak.";
        chapterStatus.text = "BÖLÜM " + (selectedChapter+1).ToString("00") + (chapter != null ? " / OYNANABİLİR" : " / YAKINDA");
        chapterPlayButton.interactable = chapter != null;
        chapterPlayButton.GetComponentInChildren<TMPro.TMP_Text>().text = chapter != null ? "BÖLÜME BAŞLA" : "YAKINDA";
        for (int i = 0; i < chapterMarkers.Length; i++)
        {
            chapterMarkers[i].color = i == selectedChapter ? new Color(.75f,.59f,.32f) : new Color(.16f,.22f,.20f);
            chapterMarkers[i].GetComponentInChildren<TMPro.TMP_Text>().color = i == selectedChapter ? new Color(.065f,.10f,.09f) : new Color(.94f,.91f,.82f);
        }
    }

    public void PlaySelectedChapter()
    {
        if (SceneLoadingUI.IsLoading || chapters == null || selectedChapter >= chapters.Length || chapters[selectedChapter] == null) return;
        ChapterData.SelectedIndex = selectedChapter;
        SceneLoadingUI.LoadScene(chapters[selectedChapter].sceneName);
    }
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Oyun kapatıldı.");
    }

    public void TogglePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }
}
