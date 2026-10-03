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
    private bool hasStarted;
    public Transform spawnPoint;
    public float timeBetweenWaves = 10f;

    private void Start()
    {
        // Sahne açılışında yalnız göstergeyi hazırla; düşman üretme.
        int waveCount = waves == null ? 0 : waves.Count;
        UİManager.UpdateWaveText(waveCount > 0 ? 1 : 0, waveCount);
    }

    public bool TryStartWaves()
    {
        if (hasStarted)
            return false;

        if (waves == null || waves.Count == 0)
        {
            Debug.LogError("Dalga başlatılamadı: dalga listesi boş.", this);
            return false;
        }

        hasStarted = true;
        StartCoroutine(SpawnWave());
        return true;
    }

    public void StopWaves()
    {
        // Oyun bitince bekleyen spawn ve sonraki dalga coroutine'lerini de durdur.
        StopAllCoroutines();
    }
    IEnumerator SpawnWave()
    {
        WaveRound currentWave = waves[currentWaveIndex];
        UİManager.UpdateWaveText(currentWaveIndex + 1, waves.Count);
        for (int i = 0; i < currentWave.enemyCount; i++)
        {
            Instantiate(currentWave.enemyPrefab, spawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(currentWave.spawnInterval);
        }
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Count)
        {
            // Son dalga spawn oldu - bölüm ancak sahadaki tüm düşmanlar ölünce geçilir.
            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);
            Debug.Log("Tüm dalgalar tamamlandı!");
            GameManager.GameOver(true);
            yield break;
        }

        yield return new WaitForSeconds(timeBetweenWaves);
        StartCoroutine(SpawnWave());
    }
}
