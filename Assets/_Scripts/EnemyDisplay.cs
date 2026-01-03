using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyDisplay : MonoBehaviour
{
    [Header("UI Baðlantýlarý (Sürükle-Býrak)")]
    public TextMeshProUGUI nameText;   // Canavarýn adý
    public TextMeshProUGUI healthText; // Caný (HP)
    public Image artworkImage;         // Resmi

    [Header("Veriler (Otomatik Dolacak)")]
    public MonsterData monsterData;    // Þu anki canavarýn bilgisi
    public int currentHealth;          // Anlýk caný

    // Bu fonksiyonu GameManager çaðýracak ve "Sahneye çýk!" diyecek
    public void SetupEnemy(MonsterData data)
    {
        monsterData = data;

        // EÐER ÝSÝM KUTUSU VARSA YAZ, YOKSA GEÇ (Hata vermez)
        if (nameText != null)
        {
            // DÜZELTME: monsterName yerine name yazdýk
            nameText.text = monsterData.name;
        }

        // Resmi ve Caný ayarla
        if (artworkImage != null) artworkImage.sprite = monsterData.artwork;
        if (artworkImage != null) artworkImage.preserveAspect = true;

        currentHealth = monsterData.maxHealth;
        UpdateHealthUI();
    }

    // Can yazýsýný güncelleyen yardýmcý fonksiyon
    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + currentHealth + "/" + monsterData.maxHealth;
        }
    }

    // Ýleride oyuncu saldýrdýðýnda bu fonksiyonu kullanacaðýz
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthUI(); // Can düþünce yazýyý güncelle

        if (currentHealth <= 0)
        {
            // DÜZELTME: monsterName yerine name yazdýk
            Debug.Log(monsterData.name + " ÖLDÜ!");
            // Buraya ölüm animasyonu veya seviye atlama kodu gelecek
        }
    }
}