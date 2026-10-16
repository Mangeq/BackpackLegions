using UnityEngine;

// One reusable panel prefab for the menu and the paused game. No gameplay ownership.
public class SettingsPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject modal;
    [SerializeField] private UnityEngine.UI.Slider volume;
    [SerializeField] private UnityEngine.UI.Toggle fullscreen;
    [SerializeField] private UnityEngine.UI.Toggle vsync;
    [SerializeField] private TMPro.TMP_Text volumeText;
    [SerializeField] private TMPro.TMP_Text qualityText;
    [SerializeField] private TMPro.TMP_Text resolutionText;
    private static SettingsPanelUI openPanel;
    private static int dismissedFrame = -1;
    private int qualityIndex, resolutionIndex;
    private Vector2Int[] resolutions;
    private const string Prefix = "BL.Settings.";
    public static bool IsOpen => openPanel != null && openPanel.modal.activeInHierarchy;
    public static bool BlocksWorldInput => IsOpen || dismissedFrame == Time.frameCount;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() { openPanel = null; dismissedFrame = -1; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ApplySaved()
    {
        AudioListener.volume = Mathf.Clamp01(PlayerPrefs.GetFloat(Prefix + "Volume", 1f));
        if (PlayerPrefs.HasKey(Prefix + "Quality"))
            QualitySettings.SetQualityLevel(Mathf.Clamp(PlayerPrefs.GetInt(Prefix + "Quality"), 0, QualitySettings.names.Length - 1));
        if (PlayerPrefs.HasKey(Prefix + "VSync")) QualitySettings.vSyncCount = PlayerPrefs.GetInt(Prefix + "VSync") == 1 ? 1 : 0;
        if (PlayerPrefs.HasKey(Prefix + "Width")) Screen.SetResolution(Mathf.Max(640,PlayerPrefs.GetInt(Prefix + "Width")), Mathf.Max(480,PlayerPrefs.GetInt(Prefix + "Height")), PlayerPrefs.GetInt(Prefix + "Fullscreen") == 1);
    }

    public void Open()
    {
        if (SceneLoadingUI.IsLoading) return;
        var list = new System.Collections.Generic.List<Vector2Int> { new Vector2Int(Screen.width,Screen.height) };
        foreach (var size in new[] {new Vector2Int(1280,720),new Vector2Int(1600,900),new Vector2Int(1920,1080),new Vector2Int(2560,1440)})
            if (!list.Contains(size) && size.x <= Screen.currentResolution.width && size.y <= Screen.currentResolution.height) list.Add(size);
        resolutions = list.ToArray(); resolutionIndex = 0;
        qualityIndex = QualitySettings.GetQualityLevel();
        volume.SetValueWithoutNotify(AudioListener.volume);
        fullscreen.SetIsOnWithoutNotify(Screen.fullScreen);
        vsync.SetIsOnWithoutNotify(QualitySettings.vSyncCount > 0);
        modal.SetActive(true); openPanel = this; RefreshLabels();
        UİManager.NotifyPanelOpened();
    }

    public void ChangeQuality(int delta) { qualityIndex = (qualityIndex + delta + QualitySettings.names.Length) % QualitySettings.names.Length; RefreshLabels(); }
    public void ChangeResolution(int delta) { resolutionIndex = (resolutionIndex + delta + resolutions.Length) % resolutions.Length; RefreshLabels(); }
    public void RefreshLabels()
    {
        volumeText.text = Mathf.RoundToInt(volume.value * 100) + "%";
        string[] translated = { "PERFORMANS", "DENGELİ", "YÜKSEK" };
        qualityText.text = QualitySettings.names.Length == 3 ? translated[qualityIndex] : QualitySettings.names[qualityIndex];
        if (resolutions != null) resolutionText.text = resolutions[resolutionIndex].x + " × " + resolutions[resolutionIndex].y;
    }

    public void Apply()
    {
        if (!IsOpen || openPanel != this) return;
        AudioListener.volume = volume.value;
        QualitySettings.SetQualityLevel(qualityIndex);
        QualitySettings.vSyncCount = vsync.isOn ? 1 : 0;
        var size = resolutions[resolutionIndex];
        Screen.SetResolution(size.x, size.y, fullscreen.isOn);
        PlayerPrefs.SetFloat(Prefix + "Volume", volume.value);
        PlayerPrefs.SetInt(Prefix + "Quality", qualityIndex);
        PlayerPrefs.SetInt(Prefix + "VSync", vsync.isOn ? 1 : 0);
        PlayerPrefs.SetInt(Prefix + "Width", size.x); PlayerPrefs.SetInt(Prefix + "Height", size.y);
        PlayerPrefs.SetInt(Prefix + "Fullscreen", fullscreen.isOn ? 1 : 0);
        PlayerPrefs.Save(); Close();
    }

    public void Close()
    {
        modal.SetActive(false); if (openPanel == this) openPanel = null;
        dismissedFrame = Time.frameCount;
    }
    public static bool TryCloseOpen() { if (!IsOpen) return false; openPanel.Close(); return true; }
    private void OnDestroy() { if (openPanel == this) openPanel = null; }
}
