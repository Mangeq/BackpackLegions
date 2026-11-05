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
            LegionFx.Ring(other.transform.position,.7f,new Color(1,.32f,.10f),.14f,true);
            TakeDamage(enemy.enemyData != null ? enemy.enemyData.BaseDamage : enemy.damage);

            // Üsse giren düşman objesini sahneden yok et

        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0 || damageAmount <= 0) return;
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        GameAudio.Play(currentHealth == 0 ? "base_fall" : "base_damage", .8f, false, .12f);
        UpdateHealthUI();
        Debug.Log("Üs Hasar Aldı! Kalan Can: " + currentHealth);
        if(uiManager && uiManager.Heart1) LegionUiMotion.Pulse(uiManager.Heart1.transform.parent,true);
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
