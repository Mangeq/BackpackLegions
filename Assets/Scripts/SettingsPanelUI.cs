using UnityEngine;

// One reusable panel prefab for the menu and the paused game. No gameplay ownership.
public class SettingsPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject modal;
    [SerializeField] private UnityEngine.UI.Slider volume;
    [SerializeField] private UnityEngine.UI.Slider effects;
    [SerializeField] private UnityEngine.UI.Toggle fullscreen;
    [SerializeField] private UnityEngine.UI.Toggle vsync;
    [SerializeField] private TMPro.TMP_Text volumeText;
    [SerializeField] private TMPro.TMP_Text effectsText;
    [SerializeField] private TMPro.TMP_Text fpsText;
    [SerializeField] private UnityEngine.UI.Button fpsPrevious, fpsNext;
    [SerializeField] private TMPro.TMP_Text qualityText;
    [SerializeField] private TMPro.TMP_Text resolutionText;
    private static SettingsPanelUI openPanel;
    private static int dismissedFrame = -1;
    private int qualityIndex, resolutionIndex, fpsIndex;
    private static readonly int[] FrameRates = { 30, 60, 90, 120, 144, 165, 240, -1 };
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
        SoundEffectsVolume.SetGain(PlayerPrefs.GetFloat(Prefix + "SFX", 1f));
        if (PlayerPrefs.HasKey(Prefix + "Quality"))
            QualitySettings.SetQualityLevel(Mathf.Clamp(PlayerPrefs.GetInt(Prefix + "Quality"), 0, QualitySettings.names.Length - 1));
        if (PlayerPrefs.HasKey(Prefix + "VSync")) QualitySettings.vSyncCount = PlayerPrefs.GetInt(Prefix + "VSync") == 1 ? 1 : 0;
        int fps=PlayerPrefs.GetInt(Prefix + "FPS",60);
        Application.targetFrameRate=QualitySettings.vSyncCount>0?-1:(System.Array.IndexOf(FrameRates,fps)>=0?fps:60);
        if (PlayerPrefs.HasKey(Prefix + "Width")) SetDisplay(Mathf.Max(640,PlayerPrefs.GetInt(Prefix + "Width")), Mathf.Max(480,PlayerPrefs.GetInt(Prefix + "Height")), PlayerPrefs.GetInt(Prefix + "Fullscreen") == 1);
    }

    public void Open()
    {
        if (SceneLoadingUI.IsLoading) return;
        if(IsOpen&&openPanel!=this)openPanel.Close();
        var list = new System.Collections.Generic.List<Vector2Int> { new Vector2Int(Screen.width,Screen.height) };
        foreach(var resolution in Screen.resolutions){var size=new Vector2Int(resolution.width,resolution.height);if(size.x>=640&&size.y>=480&&!list.Contains(size))list.Add(size);}
        if(list.Count==1)foreach(var size in new[]{new Vector2Int(1280,720),new Vector2Int(1600,900),new Vector2Int(1920,1080)})if(!list.Contains(size)&&size.x<=Screen.currentResolution.width&&size.y<=Screen.currentResolution.height)list.Add(size);
        resolutions = list.ToArray(); resolutionIndex = 0;
        qualityIndex = QualitySettings.GetQualityLevel();
        fpsIndex=System.Array.IndexOf(FrameRates,PlayerPrefs.GetInt(Prefix+"FPS",Application.targetFrameRate>0?Application.targetFrameRate:60));if(fpsIndex<0)fpsIndex=1;
        volume.SetValueWithoutNotify(AudioListener.volume);
        if(effects!=null)effects.SetValueWithoutNotify(SoundEffectsVolume.Gain);
        fullscreen.SetIsOnWithoutNotify(Screen.fullScreen);
        vsync.SetIsOnWithoutNotify(QualitySettings.vSyncCount > 0);
        GameAudio.SetPanel(modal, true); openPanel = this; RefreshLabels();
        UİManager.NotifyPanelOpened();
    }

    public void ChangeQuality(int delta) { qualityIndex = (qualityIndex + delta + QualitySettings.names.Length) % QualitySettings.names.Length; RefreshLabels(); }
    public void ChangeResolution(int delta) { if(resolutions==null||resolutions.Length==0)return;resolutionIndex = (resolutionIndex + delta + resolutions.Length) % resolutions.Length; RefreshLabels(); }
    public void ChangeFPS(int delta) { if(vsync.isOn)return;fpsIndex=(fpsIndex+delta+FrameRates.Length)%FrameRates.Length;RefreshLabels(); }
    public void RefreshLabels()
    {
        volumeText.text = Mathf.RoundToInt(volume.value * 100) + "%";
        if(effectsText!=null&&effects!=null)effectsText.text=Mathf.RoundToInt(effects.value*100)+"%";
        if(fpsText!=null)fpsText.text=vsync.isOn?"VSYNC":FrameRates[fpsIndex]<0?"SINIRSIZ":FrameRates[fpsIndex].ToString();
        if(fpsPrevious!=null)fpsPrevious.interactable=!vsync.isOn;if(fpsNext!=null)fpsNext.interactable=!vsync.isOn;
        string[] translated = { "PERFORMANS", "DENGELİ", "YÜKSEK" };
        qualityText.text = QualitySettings.names.Length == 3 ? translated[qualityIndex] : QualitySettings.names[qualityIndex];
        if (resolutions != null) resolutionText.text = resolutions[resolutionIndex].x + " × " + resolutions[resolutionIndex].y;
    }

    public void Apply()
    {
        if (!IsOpen || openPanel != this) return;
        AudioListener.volume = volume.value;
        if(effects!=null)SoundEffectsVolume.SetGain(effects.value);
        QualitySettings.SetQualityLevel(qualityIndex);
        QualitySettings.vSyncCount = vsync.isOn ? 1 : 0;
        Application.targetFrameRate=vsync.isOn?-1:FrameRates[fpsIndex];
        var size = resolutions[resolutionIndex];
        SetDisplay(size.x, size.y, fullscreen.isOn);
        PlayerPrefs.SetFloat(Prefix + "Volume", volume.value);
        if(effects!=null)PlayerPrefs.SetFloat(Prefix+"SFX",effects.value);
        PlayerPrefs.SetInt(Prefix+"FPS",FrameRates[fpsIndex]);
        PlayerPrefs.SetInt(Prefix + "Quality", qualityIndex);
        PlayerPrefs.SetInt(Prefix + "VSync", vsync.isOn ? 1 : 0);
        PlayerPrefs.SetInt(Prefix + "Width", size.x); PlayerPrefs.SetInt(Prefix + "Height", size.y);
        PlayerPrefs.SetInt(Prefix + "Fullscreen", fullscreen.isOn ? 1 : 0);
        PlayerPrefs.Save(); Close();
    }

    private static void SetDisplay(int width,int height,bool full)
    {
        var mode=full?(Application.platform==RuntimePlatform.WindowsPlayer?FullScreenMode.ExclusiveFullScreen:FullScreenMode.FullScreenWindow):FullScreenMode.Windowed;
        Screen.SetResolution(width,height,mode);
    }

    public void Close()
    {
        GameAudio.SetPanel(modal, false); if (openPanel == this) openPanel = null;
        dismissedFrame = Time.frameCount;
    }
    public static bool TryCloseOpen() { if (!IsOpen) return false; openPanel.Close(); return true; }
    private void OnDestroy() { if (openPanel == this) openPanel = null; }
}
