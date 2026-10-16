using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Backpack/Chapter", fileName = "Chapter")]
public class ChapterData : ScriptableObject
{
    public int chapterNumber = 1;
    public string chapterName;
    public string sceneName = "GameScane";
    [Tooltip("Her beş bölüm bir tema. İlk beş bölüm 0.")]
    public int themeIndex;
    [Min(0)] public int startingGold = 700;
    [Min(0)] public float preparationSeconds = 60;
    [Min(0)] public float extraPreparationPerWave = 5;
    public List<WaveRound> waves = new List<WaveRound>();
    public static int SelectedIndex { get; set; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSelection() { SelectedIndex = 0; }
    public float PreparationFor(int zeroBasedWave) => Mathf.Max(0, preparationSeconds + Mathf.Max(0, zeroBasedWave) * extraPreparationPerWave);
    public int TotalEnemies
    {
        get { int count=0; foreach(var wave in waves) if(wave!=null) count+=wave.TotalEnemies; return count; }
    }
}
