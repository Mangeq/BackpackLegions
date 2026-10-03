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
        // Hazırlık, bütün Start çağrılarından önce başlar.
        Time.timeScale = 0f;
        waveManager = FindObjectOfType<WaveManager>();

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

        CurrentState = GameState.Combat;
        Time.timeScale = 1f;

        if (!waveManager.TryStartWaves())
        {
            CurrentState = GameState.Preparation;
            Time.timeScale = 0f;
        }
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
