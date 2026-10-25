using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Persistent, bounded 2D effects voices. UI continues while preparation/menus pause time.</summary>
public sealed class GameAudio : MonoBehaviour
{
    static GameAudio instance;
    readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
    readonly Dictionary<string, float> lastPlayed = new Dictionary<string, float>();
    readonly List<AudioSource> uiVoices = new List<AudioSource>();
    readonly List<AudioSource> combatVoices = new List<AudioSource>();
    AudioSource resultVoice;
    float nextButtonScan;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetState() { instance = null; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        if (instance != null) return;
        var root = new GameObject("GameAudio");
        DontDestroyOnLoad(root);
        instance = root.AddComponent<GameAudio>();
        for (int i = 0; i < 6; i++) instance.uiVoices.Add(instance.CreateVoice());
        for (int i = 0; i < 12; i++) instance.combatVoices.Add(instance.CreateVoice());
        instance.resultVoice = instance.CreateVoice();
        foreach (var clip in Resources.LoadAll<AudioClip>("Audio/SFX")) instance.clips[clip.name] = clip;
        SceneManager.sceneLoaded += instance.SceneLoaded;
    }

    AudioSource CreateVoice()
    {
        var source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = SoundEffectsVolume.Gain;
        source.ignoreListenerPause = true;
        return source;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var voice in combatVoices) voice.Stop();
        resultVoice.Stop();
        lastPlayed.Clear();
        InstallButtons();
    }

    void Update()
    {
        if (Time.unscaledTime < nextButtonScan) return;
        nextButtonScan = Time.unscaledTime + .5f;
        InstallButtons();
    }

    static void InstallButtons()
    {
        foreach (var button in FindObjectsOfType<Button>(true))
            if (button.GetComponent<ButtonAudio>() == null) button.gameObject.AddComponent<ButtonAudio>();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        if (instance == this) instance = null;
    }

    public static void Play(string key, float volume = .65f, bool combat = false, float cooldown = .06f)
    {
        if (!Application.isPlaying) return;
        Initialize();
        AudioClip clip;
        if (!instance.clips.TryGetValue(key, out clip)) return;
        float last;
        if (instance.lastPlayed.TryGetValue(key, out last) && Time.unscaledTime - last < cooldown) return;
        var voices = combat ? instance.combatVoices : instance.uiVoices;
        AudioSource available = null;
        foreach (var voice in voices) if (!voice.isPlaying) { available = voice; break; }
        // Never cut an important cue to make space for another rapid attack.
        if (available == null) return;
        instance.lastPlayed[key] = Time.unscaledTime;
        available.volume = SoundEffectsVolume.Gain;
        available.PlayOneShot(clip, volume);
    }

    public static void Result(bool victory)
    {
        if (!Application.isPlaying) return;
        Initialize();
        foreach (var voice in instance.combatVoices) voice.Stop();
        AudioClip clip;
        if (!instance.clips.TryGetValue(victory ? "chapter_complete_success" : "chapter_complete_failure", out clip)) return;
        instance.resultVoice.volume = SoundEffectsVolume.Gain;
        instance.resultVoice.clip = clip;
        // Let the collapse transient speak before the defeat cue. DSP time works at timeScale zero.
        instance.resultVoice.PlayScheduled(AudioSettings.dspTime + (victory ? .02 : .55));
    }

    public static void SetPanel(GameObject panel, bool visible, bool info = false)
    {
        if (panel == null || panel.activeSelf == visible) return;
        panel.SetActive(visible);
        if (visible && Application.isPlaying) InstallButtons();
        Play((info ? "info-panel_" : "panel_") + (visible ? "open" : "close"), info ? .3f : .5f);
    }

    public static void Item(ItemData item)
    {
        if (item == null) return;
        int index = ItemSoundIndex(item);
        Play("SFX_" + index.ToString("00"), index == 13 ? .22f : .8f);
    }

    // Explicit override supports future authoring; element variants inherit their material family.
    public static int ItemSoundIndex(ItemData item)
    {
        if (item == null) return 1;
        if (item.soundEffectIndex >= 1 && item.soundEffectIndex <= 16) return item.soundEffectIndex;
        if (item.isElementCatalyst) return item.element == ElementType.Fire ? 13 : item.element == ElementType.Water ? 12 : item.element == ElementType.Air ? 16 : 6;
        string id = (item.itemID + " " + item.name).ToLowerInvariant();
        if (id.Contains("mortar"))
        {
            if (id.Contains("eldiven")) return 8;
            if (id.Contains("pelerin")) return 15;
            if (id.Contains("yuzugu") || id.Contains("tilsim")) return 10;
            if (id.Contains("basligi")) return 7;
            if (id.Contains("barut")) return 5;
            if (id.Contains("patlayici") || id.Contains("muhimmat")) return 13;
            if (id.Contains("doldurma")) return 2;
            return 6;
        }
        if (id.Contains("scroll")) return 16;
        if (id.Contains("elixir")) return 12;
        if (id.Contains("staff")) return 11;
        if (id.Contains("sword")) return 2;
        if (id.Contains("bow")) return 1;
        if (id.Contains("arrows")) return 9;
        if (id.Contains("shield")) return 6;
        if (id.Contains("leather") || id.Contains("belt")) return 3;
        if (id.Contains("armor")) return 7;
        if (id.Contains("helm") || id.Contains("crown") || id.Contains("circlet")) return 4;
        if (id.Contains("gloves")) return 8;
        if (id.Contains("boots")) return 14;
        if (id.Contains("cloak") || id.Contains("robe") || id.Contains("hood")) return 15;
        if (id.Contains("ring") || id.Contains("pendant") || id.Contains("amulet")) return 10;
        if (id.Contains("totem") || id.Contains("seal")) return 11;
        return item.Type == ItemType.Potion ? 12 : item.Type == ItemType.Parchment ? 16 : item.Type == ItemType.Armour ? 3 : item.Type == ItemType.Helmet ? 4 : 1;
    }
}
