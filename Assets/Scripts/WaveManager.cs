using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveEnemyGroup
{
    public GameObject enemyPrefab;
    [Min(1)] public int enemyCount = 1;
    [Min(0.05f)] public float spawnInterval = 1.5f;
    [Min(0)] public float startDelay;
}

[System.Serializable]
public class WaveRound
{
    [Tooltip("Gruplar doluysa eski tek-düşman alanları kullanılmaz.")]
    public List<WaveEnemyGroup> groups = new List<WaveEnemyGroup>();
    public GameObject enemyPrefab;
    public int enemyCount;
    public float spawnInterval;
    public IEnumerable<WaveEnemyGroup> GetGroups()
    {
        if (groups != null && groups.Count > 0) { foreach(var group in groups) yield return group; }
        else yield return new WaveEnemyGroup { enemyPrefab=enemyPrefab, enemyCount=enemyCount, spawnInterval=spawnInterval };
    }
    public int TotalEnemies { get { int count=0; foreach(var group in GetGroups()) if(group!=null) count+=group.enemyCount; return count; } }
}
public class WaveManager : MonoBehaviour
{
    public List<WaveRound> waves;
    public List<ChapterData> chapters = new List<ChapterData>();
    public ChapterData ActiveChapter { get; private set; }
    public int CurrentWaveIndex => currentWaveIndex;
    public int UpcomingWaveIndex => currentWaveIndex + (waveRunning ? 1 : 0);
    public WaveRound UpcomingWave => waves != null && UpcomingWaveIndex < waves.Count ? waves[UpcomingWaveIndex] : null;
    private int spawningGroups;
    private int currentWaveIndex = 0;
    private bool waveRunning;
    private GameManager gameManager;
    public bool CanStartNextWave => !waveRunning && waves != null && currentWaveIndex < waves.Count;
    public Transform spawnPoint;

    // Called explicitly by GameManager.Awake; independent of Start/Awake ordering.
    public void ConfigureSelectedChapter()
    {
        if (chapters == null || chapters.Count == 0) return;
        ActiveChapter = chapters[Mathf.Clamp(ChapterData.SelectedIndex, 0, chapters.Count-1)];
        if (ActiveChapter == null) return;
        waves = new List<WaveRound>();
        foreach(var source in ActiveChapter.waves)
        {
            if(source==null) { waves.Add(null); continue; }
            var copy=new WaveRound { enemyPrefab=source.enemyPrefab, enemyCount=source.enemyCount, spawnInterval=source.spawnInterval };
            if(source.groups!=null) foreach(var g in source.groups) copy.groups.Add(g==null?null:new WaveEnemyGroup { enemyPrefab=g.enemyPrefab, enemyCount=g.enemyCount, spawnInterval=g.spawnInterval, startDelay=g.startDelay });
            waves.Add(copy);
        }
        currentWaveIndex=0;
    }

    public float PreparationSeconds(float fallback) => ActiveChapter != null ? ActiveChapter.PreparationFor(currentWaveIndex) : fallback;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        // Sahne açılışında yalnız göstergeyi hazırla; düşman üretme.
        int waveCount = waves == null ? 0 : waves.Count;
        UİManager.UpdateWaveText(waveCount > 0 ? 1 : 0, waveCount);
    }

    public bool TryStartWaves()
    {
        if (!CanStartNextWave || gameManager == null || gameManager.CurrentState != GameManager.GameState.Combat || gameManager.IsPaused)
            return false;

        WaveRound nextWave = waves[currentWaveIndex];
        bool valid = nextWave != null;
        if(valid) foreach(var group in nextWave.GetGroups())
            if(group == null || group.enemyCount < 0 || group.spawnInterval < 0 || group.startDelay < 0 ||
                (group.enemyCount > 0 && (group.enemyPrefab == null || group.enemyPrefab.GetComponent<Enemy>() == null || group.enemyPrefab.GetComponent<Enemy>().enemyData == null || spawnPoint == null))) valid=false;
        if (!valid)
        {
            Debug.LogError("Dalga başlatılamadı: düşman, doğma noktası veya düşman sayısı geçersiz.", this);
            return false;
        }

        waveRunning = true;
        LegionUiMotion.Banner("DALGA "+(currentWaveIndex+1));
        if(spawnPoint) LegionFx.Ring(spawnPoint.position,.8f,LegionFx.Gold,.12f);
        foreach(var tower in FindObjectsOfType<Tower>()) tower.Evolution?.BeginWave(currentWaveIndex);
        StartCoroutine(SpawnWave());
        return true;
    }

    public void StopWaves()
    {
        // Sonuç ekranından sonra üretim veya hazırlığa dönüş gerçekleşmesin.
        StopAllCoroutines();
        foreach(var tower in FindObjectsOfType<Tower>()) tower.Evolution?.CancelWave();
        waveRunning = false;
        spawningGroups = 0;
    }
    IEnumerator SpawnWave()
    {
        WaveRound currentWave = waves[currentWaveIndex];
        UİManager.UpdateWaveText(currentWaveIndex + 1, waves.Count);
        var groups = new List<WaveEnemyGroup>(currentWave.GetGroups());
        spawningGroups = groups.Count;
        foreach(var group in groups) StartCoroutine(SpawnGroup(group));
        yield return new WaitUntil(() => spawningGroups == 0);
        // Üretim tamamlanmış olsa bile sahadaki son düşman temizlenmeden dalga bitmez.
        yield return new WaitUntil(() => !gameManager.IsPaused && GameObject.FindGameObjectsWithTag("Enemy").Length == 0);
        foreach(var tower in FindObjectsOfType<Tower>()) tower.Evolution?.CompleteWave(currentWaveIndex);
        LegionUiMotion.Banner("DALGA TAMAMLANDI");
        currentWaveIndex++;
        waveRunning = false;

        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("Tüm dalgalar tamamlandı!");
            GameManager.GameOver(true);
            yield break;
        }

        UİManager.UpdateWaveText(currentWaveIndex + 1, waves.Count);
        gameManager.BeginPreparation();
    }

    private IEnumerator SpawnGroup(WaveEnemyGroup group)
    {
        if(group.startDelay>0) yield return new WaitForSeconds(group.startDelay);
        for(int i=0;i<group.enemyCount;i++)
        {
            Instantiate(group.enemyPrefab,spawnPoint.position,Quaternion.identity);
            if(i+1<group.enemyCount) yield return new WaitForSeconds(Mathf.Max(.05f,group.spawnInterval));
        }
        spawningGroups--;
    }
}
