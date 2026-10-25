using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SfxValidation
{
    [MenuItem("Tools/Audio/Validate SFX Integration")]
    public static void Validate()
    {
        var clips = Resources.LoadAll<AudioClip>("Audio/SFX");
        if (clips.Length != 33) throw new Exception("Expected 33 SFX clips, got " + clips.Length);
        var names = new HashSet<string>();
        foreach (var clip in clips)
        {
            if (!names.Add(clip.name) || clip.length <= 0 || !clip.LoadAudioData())
                throw new Exception("Invalid/duplicate audio: " + clip.name);
        }
        var rows = new List<string> { "Item asset,Item ID,SFX" };
        var used = new HashSet<int>();
        var items = Resources.LoadAll<ItemData>("Data/Items");
        foreach (var item in items)
        {
            int index = GameAudio.ItemSoundIndex(item);
            string key = "SFX_" + index.ToString("00");
            if (!names.Contains(key)) throw new Exception("Missing sound for " + item.name);
            used.Add(index);
            rows.Add(AssetDatabase.GetAssetPath(item) + "," + item.itemID + "," + key);
        }
        if (items.Length == 0 || used.Count != 16) throw new Exception("Item coverage is incomplete.");
        var probe = ScriptableObject.CreateInstance<ItemData>();
        try
        {
            probe.itemID = "mortar_starter_bow";
            if (GameAudio.ItemSoundIndex(probe) != 6) throw new Exception("Mortar incorrectly uses bow sound.");
            probe.itemID = "v1_shortbow_fire";
            if (GameAudio.ItemSoundIndex(probe) != 1) throw new Exception("Element variant lost material family.");
            probe.soundEffectIndex = 14;
            if (GameAudio.ItemSoundIndex(probe) != 14) throw new Exception("Inspector override ignored.");
        }
        finally { UnityEngine.Object.DestroyImmediate(probe); }
        Directory.CreateDirectory("Docs/Validation/sfx");
        File.WriteAllLines("Docs/Validation/sfx/item-mapping.csv", rows);
        string result = "PASS: " + clips.Length + " clips loaded; " + items.Length + " items mapped; all " + used.Count + " item effects covered; mortar/element/override checks passed.";
        File.WriteAllText("Docs/Validation/sfx/result.txt", result);
        Debug.Log(result);
    }
}
