using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Preparation,
        Combat,
        Victory,
        Defeat
    }

    public GameState CurrentState { get; private set; } = GameState.Preparation;
    public bool IsPaused { get; private set; }
    public int SelectedSpeed { get; private set; } = 1;
    public bool CanChangeSpeed => CurrentState == GameState.Combat && !IsPaused;
    [Min(0f)] public float autoStartDelay = 60f;
    private static GameManager instance;
    // Finish transient battle visuals when preparation or results stop the combat clock.
    internal static float EffectDelta => instance && !instance.IsPaused && instance.CurrentState!=GameState.Combat
        ? Time.unscaledDeltaTime : Time.deltaTime;
    private UİManager uiManager;
    public bool itemPlacementEnabled = true;
    public static bool ItemPlacementEnabled => instance != null && instance.itemPlacementEnabled;
    public static bool CanEditItems => ItemPlacementEnabled && instance != null && instance.CurrentState == GameState.Preparation && !instance.IsPaused;
    public static bool CanBuyTowers => instance != null && !instance.HasEnded && !instance.IsPaused;
    public static bool CanLevelUpTowers => CanBuyTowers;
    public static bool CanUpgradeTowers => instance != null && instance.CurrentState == GameState.Preparation && !instance.IsPaused;
    public float PreparationSecondsRemaining { get; private set; }
    private bool autoStartAttempted;

    private WaveManager waveManager;
    private float timeScaleBeforePause;
    public bool HasEnded => CurrentState == GameState.Victory || CurrentState == GameState.Defeat;

    public GameObject pausePanel;
    public SettingsPanelUI settingsPanel;
    public GameObject GameOverPanel;
    public GameObject victoryPanel;
    public SkillTreePanelUI skillTreePanel;
    public TMPro.TMP_Text victorySummary, defeatSummary;
    public Button nextChapterButton;
    private TMPro.TMP_Text gameOverText;

    internal Button continueButton;
    internal Button backMainMenuButton;

    internal Button restartButton;
    internal Button exitButton;
    

    private void Awake()
    {
        instance = this;
        uiManager = FindObjectOfType<UİManager>();
        // Hazırlık, bütün Start çağrılarından önce başlar.
        Time.timeScale = 0f;
        waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null) waveManager.ConfigureSelectedChapter();
        // Each entry/restart starts a fresh chapter budget; no static gold carry-over.
        EconomyManager.Gold = waveManager != null && waveManager.ActiveChapter != null
            ? Mathf.Max(0, waveManager.ActiveChapter.startingGold) : 700;
        PreparationSecondsRemaining = waveManager != null ? waveManager.PreparationSeconds(autoStartDelay) : autoStartDelay;

        exitButton = System.Array.Find(GameOverPanel.GetComponentsInChildren<Button>(true), b => b.name == "BackMainMenuButton");
        restartButton = System.Array.Find(GameOverPanel.GetComponentsInChildren<Button>(true), b => b.name == "NewGameButton");
        gameOverText = System.Array.Find(GameOverPanel.GetComponentsInChildren<TMPro.TMP_Text>(true), t => t.name == "Text (TMP)");
        continueButton = pausePanel.transform.Find("CountineButton").GetComponent<Button>();
        backMainMenuButton = pausePanel.transform.Find("MainMenuButton").GetComponent<Button>();

        exitButton.onClick.AddListener(ExitToMainMenu);
        restartButton.onClick.AddListener(RestartGame);
        continueButton.onClick.AddListener(ContinueGame);
        backMainMenuButton.onClick.AddListener(ExitToMainMenu);

        GameAudio.SetPanel(pausePanel, false);
        GameOverPanel.SetActive(false);
        if(victoryPanel!=null)victoryPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            HandleEscape();
        TickPreparation(Time.unscaledDeltaTime);
    }

    private void TickPreparation(float elapsed)
    {
        if (CurrentState != GameState.Preparation || IsPaused || autoStartAttempted) return;
        PreparationSecondsRemaining = Mathf.Max(0f, PreparationSecondsRemaining - elapsed);
        if (PreparationSecondsRemaining > 0f) return;
        autoStartAttempted = true;
        StartGame();
    }

    public void StartGame()
    {
        if (CurrentState != GameState.Preparation || IsPaused)
            return;

        if (waveManager == null)
        {
            Debug.LogError("Dalga başlatılamadı: sahnede WaveManager yok.", this);
            return;
        }

        if (!waveManager.CanStartNextWave) return;
        BackpackDraggable.CancelActiveDrag();
        CurrentState = GameState.Combat;
        Time.timeScale = SelectedSpeed;

        if (!waveManager.TryStartWaves())
        {
            CurrentState = GameState.Preparation;
            Time.timeScale = 0f;
        }
        else if (uiManager != null) uiManager.CloseTowerPanel();
    }

    public void BeginPreparation()
    {
        if (CurrentState != GameState.Combat || IsPaused) return;
        CurrentState = GameState.Preparation;
        Time.timeScale = 0f;
        PreparationSecondsRemaining = waveManager != null ? waveManager.PreparationSeconds(autoStartDelay) : autoStartDelay;
        autoStartAttempted = false;
        var shop = FindObjectOfType<ShopUIManager>(true);
        if (shop != null) shop.RefreshShopsForPreparation();
    }

    public void SetGameSpeed(int speed)
    {
        if (!CanChangeSpeed || speed < 1 || speed > 3) return;
        SelectedSpeed = speed;
        Time.timeScale = SelectedSpeed;
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    public void TogglePause()
    {
        if (HasEnded)
            return;

        if (IsPaused)
        {
            ContinueGame();
            return;
        }

        // Hazırlıkta 0, savaşta mevcut hız saklanır; devam etmek fazı değiştirmez.
        timeScaleBeforePause = Time.timeScale;
        BackpackDraggable.CancelActiveDrag();
        IsPaused = true;
        Time.timeScale = 0f;
        GameAudio.SetPanel(pausePanel, true);
        UİManager.NotifyPanelOpened();
    }

    public void HandleEscape()
    {
        if (SceneLoadingUI.IsLoading || SkillTreePanelUI.TryCloseOpen() || SettingsPanelUI.TryCloseOpen()) return;
        if (HasEnded) return;
        if (IsPaused) { ContinueGame(); return; }
        if (uiManager != null && uiManager.CloseTopPanel()) return;
        TogglePause();
    }

    public void ContinueGame()
    {
        if (SettingsPanelUI.IsOpen || SkillTreePanelUI.IsOpen) return;
        if (!IsPaused || HasEnded)
            return;

        IsPaused = false;
        Time.timeScale = timeScaleBeforePause;
        GameAudio.SetPanel(pausePanel, false);
    }

    // Separate result panels share the same authoritative game state.
    public static void GameOver(bool isVictory)
    {
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        // Aynı karede ikinci bir sonuç gelirse ilk sonuç korunur.
        if (gm == null || gm.HasEnded)
            return;

        BackpackDraggable.CancelActiveDrag();
        GameAudio.Result(isVictory);
        gm.CurrentState = isVictory ? GameState.Victory : GameState.Defeat;
        gm.IsPaused = false;
        gm.pausePanel.SetActive(false);
        if (gm.waveManager != null)
            gm.waveManager.StopWaves();
        Time.timeScale = 0f;
        SettingsPanelUI.TryCloseOpen();SkillTreePanelUI.TryCloseOpen();
        var chapter=gm.waveManager!=null?gm.waveManager.ActiveChapter:null;
        int total=gm.waveManager!=null?gm.waveManager.waves.Count:0;
        int completed=isVictory?total:gm.waveManager!=null?gm.waveManager.CurrentWaveIndex:0;
        string summary=(chapter!=null?"BÖLÜM "+chapter.chapterNumber.ToString("00")+" · "+chapter.chapterName:"SAVUNMA RAPORU")
            + "\n\nTamamlanan dalga   "+completed+" / "+total+"\nKalan altın   "+EconomyManager.Gold;
        var baseObject = GameObject.FindGameObjectWithTag("Base");
        var castle = baseObject != null ? baseObject.GetComponent<Base>() : null;
        int hp = castle != null ? castle.currentHealth : 0;
        int maxHp = castle != null ? castle.maxHealth : 0;
        var progress = new SkillTreeProgress();
        int gained = chapter != null ? progress.RecordResult(chapter.chapterNumber, isVictory, hp, maxHp) : 0;
        summary += "\nKalan can   " + hp + " / " + maxHp
            + "\nYetenek puanı   +" + gained + "  ·  Toplam " + progress.AvailablePoints;
        if(gm.victorySummary!=null)gm.victorySummary.text=summary;
        if(gm.defeatSummary!=null)gm.defeatSummary.text=summary;
        gm.gameOverText.text=isVictory&&gm.victoryPanel==null?"BÖLÜM GEÇİLDİ":"KULE DÜŞTÜ";
        gm.GameOverPanel.SetActive(!isVictory || gm.victoryPanel==null);
        if(gm.victoryPanel!=null)gm.victoryPanel.SetActive(isVictory);
        gm.StartCoroutine(LegionUiMotion.Reveal(isVictory&&gm.victoryPanel?gm.victoryPanel:gm.GameOverPanel,isVictory?gm.victorySummary:gm.defeatSummary,isVictory,gained));
        if(gm.nextChapterButton!=null)
        {
            gm.nextChapterButton.interactable=gm.NextChapterIndex>=0;
            gm.nextChapterButton.GetComponentInChildren<TMPro.TMP_Text>().text=gm.NextChapterIndex>=0?"SONRAKİ BÖLÜM  ›":"TÜM BÖLÜMLER TAMAMLANDI";
        }
    }

    public void RestartGame()
    {
        if(SceneLoadingUI.IsLoading)return;
        SceneLoadingUI.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneLoadingUI.LoadScene("MainMenu");
    }

    private int NextChapterIndex
    {
        get
        {
            if(waveManager==null||waveManager.ActiveChapter==null)return -1;
            int current=waveManager.chapters.IndexOf(waveManager.ActiveChapter);
            int next=current+1;
            return current>=0&&next<waveManager.chapters.Count&&waveManager.chapters[next]!=null?next:-1;
        }
    }
    public void NextChapter()
    {
        if(CurrentState!=GameState.Victory||SceneLoadingUI.IsLoading)return;
        int next=NextChapterIndex;if(next<0)return;
        ChapterData.SelectedIndex=next;
        SceneLoadingUI.LoadScene(waveManager.chapters[next].sceneName);
    }
    public void OpenSkillTree() { if((IsPaused||HasEnded)&&skillTreePanel!=null)skillTreePanel.Open(); }
    public void OpenSettings() { if (IsPaused && settingsPanel != null) settingsPanel.Open(); }
}
