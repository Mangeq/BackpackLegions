using UnityEngine;

public class Base : MonoBehaviour
{
    [Header("Base Ayarları")]
    public int maxHealth = 100;
    public int currentHealth;

    private UİManager uiManager;

    void Start()
    {
        uiManager = FindObjectOfType<UİManager>();
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // Düşman içeri girdiğinde tetiklenecek fonksiyon
    private void OnTriggerEnter(Collider other)
    {
        // Gelen objenin Tag'i "Enemy" ise kodu çalıştır
        if (other.CompareTag("Enemy"))
        {
            // Üsse ulaşan düşmanın hasarını uygula.
            var enemy = other.GetComponent<Enemy>();
            if (enemy == null || !enemy.TryReachBase()) return;
            TakeDamage(enemy.enemyData != null ? enemy.enemyData.BaseDamage : enemy.damage);

            // Üsse giren düşman objesini sahneden yok et

        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        UpdateHealthUI();
        Debug.Log("Üs Hasar Aldı! Kalan Can: " + currentHealth);
        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    private void UpdateHealthUI()
    {
        float healthRatio = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;

        uiManager.Heart1.SetActive(healthRatio > 0f);
        uiManager.Heart2.SetActive(healthRatio > 1f / 3f);
        uiManager.Heart3.SetActive(healthRatio > 2f / 3f);
    }

    void GameOver()
    {
        GameManager.GameOver(false);
    }
}
