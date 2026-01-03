using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Monster : MonoBehaviour
{
    public MonsterData monsterData;
    public int currentHealth;

    [Header("UI Referanslarý")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI nameText;
    public Image monsterImage;
    public Slider rageSlider;

    // Kaç vuruþ yaptýðýný sayan sayaç
    private int attackCounter = 0;

    public void Setup(MonsterData data, int startingHealth = -1)
    {
        monsterData = data;

        currentHealth = (startingHealth == -1) ? monsterData.maxHealth : startingHealth;

        if (nameText != null) nameText.text = monsterData.name;
        if (monsterImage != null) monsterImage.sprite = monsterData.artwork;

        // --- ÖFKE BARINI AYARLA (5 Vuruþluk) ---
        attackCounter = 0;
        if (rageSlider != null)
        {
            rageSlider.maxValue = 5; // ARTIK 5 VURUÞTA DOLACAK
            rageSlider.value = 0;
        }

        UpdateHealthUI();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthUI();

        if (FindFirstObjectByType<GameManager>() != null)
        {
            FindFirstObjectByType<GameManager>().CheckForWinner();
        }
    }

    void UpdateHealthUI()
    {
        if (healthText != null) healthText.text = currentHealth.ToString();
    }

    // --- KRÝTÝK KONTROLÜ ---
    public bool IsCriticalNext()
    {
        // Sayaç 4 ise, sýradaki vuruþ 5. vuruþ (Kritik) olacaktýr.
        return attackCounter >= 4;
    }

    // --- SALDIRI GÜCÜ HESAPLAMA ---
    public int CalculateAttackDamage()
    {
        if (monsterData == null) return 0;

        // 1. Sayacý Arttýr
        attackCounter++;
        if (rageSlider != null) rageSlider.value = attackCounter;

        // 2. Temel Hasarý Hesapla (DOSYADAKÝ GERÇEK GÜÇ)
        // Artýk azaltma kodu YOK. Neyse o vuracak.
        int damage = Random.Range(monsterData.minDamage, monsterData.maxDamage + 1);

        // 3. KRÝTÝK VURUÞ (5. Vuruþ mu?)
        if (attackCounter >= 5)
        {
            // KRÝTÝK! Hasarý 2 ile çarp (%200 Hasar)
            // Ýstersen 1.5f yapýp %50 artýrabilirsin. Þimdilik 2 katý yaptýk sert vursun.
            damage = Mathf.RoundToInt(damage * 2.0f);

            Debug.Log("CANAVAR KRÝTÝK VURDU! (2 KAT HASAR)");

            // Barý sýfýrlamak için hazýrlýk
            StartCoroutine(ResetRageBarDelayed());
        }

        // En az 1 vursun
        if (damage < 1) damage = 1;

        return damage;
    }

    IEnumerator ResetRageBarDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        attackCounter = 0;
        if (rageSlider != null) rageSlider.value = 0;
    }
}