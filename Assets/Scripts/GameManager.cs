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
    public static bool CanEditItems => instance != null && instance.CurrentState == GameState.Preparation && !instance.IsPaused;
    public static bool CanBuyTowers => instance != null && instance.CurrentState == GameState.Preparation && !instance.IsPaused;
    public static bool CanUpgradeTowers => instance != null && instance.CurrentState == GameState.Preparation && !instance.IsPaused;
    public float PreparationSecondsRemaining { get; private set; }
    private bool autoStartAttempted;

    private WaveManager waveManager;
    private float timeScaleBeforePause;
    private bool HasEnded => CurrentState == GameState.Victory || CurrentState == GameState.Defeat;

    public GameObject pausePanel;
    public GameObject GameOverPanel;
    private TMPro.TMP_Text gameOverText;

    internal Button continueButton;
    internal Button backMainMenuButton;

    internal Button restartButton;
    internal Button exitButton;
    

    private void Awake()
    {
        instance = this;
        // Hazırlık, bütün Start çağrılarından önce başlar.
        Time.timeScale = 0f;
        waveManager = FindObjectOfType<WaveManager>();
        PreparationSecondsRemaining = autoStartDelay;

        exitButton = GameOverPanel.transform.Find("BackMainMenuButton").GetComponent<Button>();
        restartButton = GameOverPanel.transform.Find("NewGameButton").GetComponent<Button>();
        gameOverText = GameOverPanel.transform.Find("Text (TMP)").GetComponent<TMPro.TMP_Text>();
        continueButton = pausePanel.transform.Find("CountineButton").GetComponent<Button>();
        backMainMenuButton = pausePanel.transform.Find("MainMenuButton").GetComponent<Button>();

        exitButton.onClick.AddListener(ExitToMainMenu);
        restartButton.onClick.AddListener(RestartGame);
        continueButton.onClick.AddListener(ContinueGame);
        backMainMenuButton.onClick.AddListener(ExitToMainMenu);

        pausePanel.SetActive(false);
        GameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
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
    }

    public void BeginPreparation()
    {
        if (CurrentState != GameState.Combat || IsPaused) return;
        CurrentState = GameState.Preparation;
        Time.timeScale = 0f;
        PreparationSecondsRemaining = autoStartDelay;
        autoStartAttempted = false;
        var shop = FindObjectOfType<ShopUIManager>();
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
        pausePanel.SetActive(true);
    }

    public void ContinueGame()
    {
        if (!IsPaused || HasEnded)
            return;

        IsPaused = false;
        Time.timeScale = timeScaleBeforePause;
        pausePanel.SetActive(false);
    }

    // Kazanma ve kaybetme aynı paneli kullanır, sadece yazı değişir.
    public static void GameOver(bool isVictory)
    {
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        // Aynı karede ikinci bir sonuç gelirse ilk sonuç korunur.
        if (gm.HasEnded)
            return;

        BackpackDraggable.CancelActiveDrag();
        gm.CurrentState = isVictory ? GameState.Victory : GameState.Defeat;
        gm.IsPaused = false;
        gm.pausePanel.SetActive(false);
        if (gm.waveManager != null)
            gm.waveManager.StopWaves();
        Time.timeScale = 0f;
        EconomyManager.Gold = 1000;
        gm.gameOverText.text = isVictory ? "BÖLÜM GEÇİLDİ !" : "KULE DÜŞTÜ ! OYUN BİTTİ !";
        gm.GameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        EconomyManager.Gold = 1000;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
