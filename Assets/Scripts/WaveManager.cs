using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveRound
{
    public GameObject enemyPrefab;
    public int enemyCount;
    public float spawnInterval;
}
public class WaveManager : MonoBehaviour
{
    public List<WaveRound> waves;
    private int currentWaveIndex = 0;
    private bool waveRunning;
    private GameManager gameManager;
    public bool CanStartNextWave => !waveRunning && waves != null && currentWaveIndex < waves.Count;
    public Transform spawnPoint;

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
        if (nextWave == null || nextWave.enemyCount < 0 || (nextWave.enemyCount > 0 && (nextWave.enemyPrefab == null || spawnPoint == null)))
        {
            Debug.LogError("Dalga başlatılamadı: düşman, doğma noktası veya düşman sayısı geçersiz.", this);
            return false;
        }

        waveRunning = true;
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
    }
    IEnumerator SpawnWave()
    {
        WaveRound currentWave = waves[currentWaveIndex];
        UİManager.UpdateWaveText(currentWaveIndex + 1, waves.Count);
        for (int i = 0; i < currentWave.enemyCount; i++)
        {
            Instantiate(currentWave.enemyPrefab, spawnPoint.position, Quaternion.identity);
            if (i + 1 < currentWave.enemyCount)
                yield return new WaitForSeconds(Mathf.Max(0f, currentWave.spawnInterval));
        }
        // Üretim tamamlanmış olsa bile sahadaki son düşman temizlenmeden dalga bitmez.
        yield return new WaitUntil(() => !gameManager.IsPaused && GameObject.FindGameObjectsWithTag("Enemy").Length == 0);
        foreach(var tower in FindObjectsOfType<Tower>()) tower.Evolution?.CompleteWave(currentWaveIndex);
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
}
