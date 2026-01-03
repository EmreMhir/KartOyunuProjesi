using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro için gerekli
using UnityEngine.SceneManagement;

public class MarketManager : MonoBehaviour
{
    [Header("UI Referanslarý")]
    public TextMeshProUGUI goldText; // Oyuncunun parasýný gösteren yazý
    public TextMeshProUGUI feedbackText; // "Satýn Alýndý!" veya "Para Yetersiz!" yazýsý

    // FÝYAT LÝSTESÝ (Ýstersen buradan deðiþtirebilirsin)
    private int archerPrice = 40;
    private int magePrice = 60;
    private int healthPrice = 50;
    private int critPrice = 80;

    void Start()
    {
        UpdateUI();
        if (feedbackText != null) feedbackText.text = "Markete Hoþgeldin!";

        // Markete girince huzurlu müzik çalsýn
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusic(AudioManager.instance.mainMenuMusic);
        }
    }

    void UpdateUI()
    {
        // Mevcut parayý ekrana yaz
        int currentGold = SaveManager.LoadGold();
        if (goldText != null) goldText.text = "Altýn: " + currentGold;
    }

    // --- SATIN ALMA FONKSÝYONLARI ---

    // 1. OKÇU GÜÇLENDÝRME (+1 Hasar)
    public void BuyArcherUpgrade()
    {
        int gold = SaveManager.LoadGold();
        if (gold >= archerPrice)
        {
            SaveManager.SaveGold(gold - archerPrice);
            int currentBonus = SaveManager.LoadArcherBonus();
            SaveManager.SaveArcherBonus(currentBonus + 1);

            ShowFeedback("Okçular Güçlendi! (+1 Hasar)");
            UpdateUI();
        }
        else
        {
            ShowFeedback("Para Yetersiz! (" + archerPrice + " Altýn Gerek)");
        }
    }

    // 2. BÜYÜCÜ GÜÇLENDÝRME (+2 Hasar)
    public void BuyMageUpgrade()
    {
        int gold = SaveManager.LoadGold();
        if (gold >= magePrice)
        {
            SaveManager.SaveGold(gold - magePrice);
            int currentBonus = SaveManager.LoadMageBonus();
            SaveManager.SaveMageBonus(currentBonus + 2);

            ShowFeedback("Büyücüler Coþtu! (+2 Hasar)");
            UpdateUI();
        }
        else
        {
            ShowFeedback("Para Yetersiz! (" + magePrice + " Altýn Gerek)");
        }
    }

    // 3. CAN GÜÇLENDÝRME (+20 Max Can)
    public void BuyHealthUpgrade()
    {
        int gold = SaveManager.LoadGold();
        if (gold >= healthPrice)
        {
            SaveManager.SaveGold(gold - healthPrice);
            int currentBonus = SaveManager.LoadMaxHealthBonus();
            SaveManager.SaveMaxHealthBonus(currentBonus + 20);

            ShowFeedback("Max Can Arttý! (+20 HP)");
            UpdateUI();
        }
        else
        {
            ShowFeedback("Para Yetersiz! (" + healthPrice + " Altýn Gerek)");
        }
    }

    // 4. KRÝTÝK ÞANS (+%10 Ýhtimal)
    public void BuyCritUpgrade()
    {
        int gold = SaveManager.LoadGold();
        // Kritik þans %50'yi geçmesin (Oyun bozulmasýn diye sýnýr koyabiliriz)
        if (SaveManager.LoadCritChance() >= 50)
        {
            ShowFeedback("Maksimum Kritik Þansa Ulaþtýn!");
            return;
        }

        if (gold >= critPrice)
        {
            SaveManager.SaveGold(gold - critPrice);
            int currentChance = SaveManager.LoadCritChance();
            SaveManager.SaveCritChance(currentChance + 10);

            ShowFeedback("Þansýn Döndü! (+%10 Kritik)");
            UpdateUI();
        }
        else
        {
            ShowFeedback("Para Yetersiz! (" + critPrice + " Altýn Gerek)");
        }
    }

    // --- YARDIMCI FONKSÝYONLAR ---

    void ShowFeedback(string message)
    {
        if (feedbackText != null) feedbackText.text = message;
        Debug.Log(message);
    }

    public void BackToMap()
    {
        SceneManager.LoadScene("MapScene");
    }
}