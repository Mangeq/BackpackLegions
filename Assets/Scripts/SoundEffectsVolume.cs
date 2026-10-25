using System.Collections.Generic;
using UnityEngine;

/// <summary>Separate effects gain without changing each source's authored loudness.</summary>
[DefaultExecutionOrder(-10000)]
public sealed class SoundEffectsVolume : MonoBehaviour
{
    public static float Gain { get; private set; } = 1f;
    static SoundEffectsVolume instance;
    sealed class Voice { public float original,last; }
    readonly Dictionary<AudioSource,Voice> voices=new Dictionary<AudioSource,Voice>();
    readonly List<AudioSource> expired=new List<AudioSource>();
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetState() { Gain=1;instance=null; }
    public static void SetGain(float value)
    {
        Gain=Mathf.Clamp01(value);
        if(!Application.isPlaying)return;
        if(instance==null){var root=new GameObject("SoundEffectsVolume");DontDestroyOnLoad(root);instance=root.AddComponent<SoundEffectsVolume>();}
        instance.Refresh();
    }
    void LateUpdate() { Refresh(); }
    void Refresh()
    {
        // The current project has only effects sources. A source explicitly tagged as
        // music by its mixer group keeps its own gain and is affected by master only.
        foreach(var source in FindObjectsOfType<AudioSource>())
        {
            if(source.GetComponent<GameAudio>()!=null){source.volume=Gain;continue;}
            if(source.outputAudioMixerGroup!=null&&source.outputAudioMixerGroup.name.IndexOf("Music",System.StringComparison.OrdinalIgnoreCase)>=0)continue;
            Voice voice;if(!voices.TryGetValue(source,out voice)){voice=new Voice{original=source.volume,last=source.volume};voices.Add(source,voice);}
            if(!Mathf.Approximately(source.volume,voice.last))voice.original=source.volume;
            voice.last=Mathf.Clamp01(voice.original*Gain);source.volume=voice.last;
        }
        expired.Clear();foreach(var pair in voices)if(pair.Key==null)expired.Add(pair.Key);foreach(var source in expired)voices.Remove(source);
    }
    void OnDestroy(){if(instance==this)instance=null;}
}
