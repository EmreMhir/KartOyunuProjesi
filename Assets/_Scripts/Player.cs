using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Can Deðerleri")]
    public int maxHealth = 100;
    public int currentHealth;
    public int currentBlock;

    [Header("UI Baðlantýlarý")]
    public TextMeshProUGUI healthText;
    public Slider healthSlider; // Unity'den Can Barýný buraya sürükle!

    public void Setup(int health, int newMaxHealth)
    {
        // 1. Yeni kapasiteyi ayarla (Örn: 100 yerine 120 yap)
        maxHealth = newMaxHealth;

        // 2. Mevcut caný ayarla
        currentHealth = health;

        // Hata önleme: Can max'tan büyükse eþitle, 0'dan küçükse 0 yap
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (currentHealth < 0) currentHealth = 0;

        currentBlock = 0; // Her savaþa sýfýr zýrhla baþla

        // 3. Slider (Çubuk) ayarýný güncelle
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        UpdateHealthUI();
    }

    // Kalkan ekleme fonksiyonu
    public void AddBlock(int amount)
    {
        currentBlock += amount;
        UpdateHealthUI();
    }

    // Ýyileþme fonksiyonu
    public void Heal(int amount)
    {
        currentHealth += amount;

        // Kapasiteyi (maxHealth) geçmesine izin verme
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateHealthUI();
    }

    public void TakeDamage(int amount)
    {
        // Önce zýrhtan düþ
        if (currentBlock > 0)
        {
            int damageToBlock = Mathf.Min(amount, currentBlock);
            currentBlock -= damageToBlock;
            amount -= damageToBlock;
        }

        // Kalan hasar varsa candan düþ
        if (amount > 0)
        {
            currentHealth -= amount;
            if (currentHealth < 0) currentHealth = 0;
        }

        UpdateHealthUI();

        // GameManager'a oyun bitti mi diye sor
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null) gm.CheckForWinner();
    }

    void UpdateHealthUI()
    {
        // 1. Slider'ý güncelle
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        // 2. Yazýyý güncelle
        if (healthText != null)
        {
            // Zýrh varsa mavi renkte göster: 100 (+10) / 120
            if (currentBlock > 0)
                healthText.text = $"{currentHealth} <color=blue>(+{currentBlock})</color> / {maxHealth}";
            else
                healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    // Tur baþýnda zýrhý sýfýrlamak istersen bu fonksiyonu kullanabilirsin
    public void ResetBlock()
    {
        currentBlock = 0;
        UpdateHealthUI();
    }
}