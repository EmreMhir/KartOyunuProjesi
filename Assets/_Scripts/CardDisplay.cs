using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    [Header("Kart Verisi")]
    public CardData cardData;

    [Header("UI Elemanlarý")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI damageText;
    public Image artworkImage; // Kartýn ortasýndaki resim

    // Referanslar
    private GameManager gameManager;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private bool isSelected = false;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        originalScale = transform.localScale;
    }

    void Start()
    {
        // --- 1. GÖRSEL DÜZELTME (SOLUKLUK VE YAZI ÝÇÝN) ---

        // A) Kartýn Arka Planýný Beyaz Yap (Solukluðu Giderir)
        Image background = GetComponent<Image>();
        if (background != null)
        {
            background.color = Color.white; // Gri ise Beyaz yapar
            // Þeffaflýðý (Alpha) tam yap
            Color visibleColor = background.color;
            visibleColor.a = 1f;
            background.color = visibleColor;
        }

        // B) Kart Resmini Beyaz Yap
        if (artworkImage != null) artworkImage.color = Color.white;

        // C) Yazýlarý En Öne Getir (Gizlenmeyi Önler)
        if (damageText != null)
        {
            // Canvas içinde en son çizilen en üstte görünür
            damageText.transform.SetAsLastSibling();

            // Garanti olsun diye Z ekseninde biraz öne çekelim
            Vector3 textPos = damageText.transform.localPosition;
            textPos.z = -1f;
            damageText.transform.localPosition = textPos;
        }

        if (nameText != null) nameText.transform.SetAsLastSibling();


        // --- 2. VERÝLERÝ DOLDURMA ---
        if (cardData != null)
        {
            if (nameText != null) nameText.text = cardData.name;
            if (artworkImage != null) artworkImage.sprite = cardData.artwork;

            if (nameText != null)
            {
                nameText.text = cardData.name;
            }
            if (artworkImage != null) artworkImage.sprite = cardData.artwork;
            
            // --- MARKET HESAPLAMASI ---
            int finalDamage = cardData.damage;

            if (cardData.cardType == CardType.Attack)
            {
                finalDamage += SaveManager.LoadUpgrades().cardBonus;
                if (cardData.name.Contains("Okçu")) finalDamage += SaveManager.LoadArcherBonus();
                if (cardData.name.Contains("Büyücü")) finalDamage += SaveManager.LoadMageBonus();
            }

            // Hasarý Yaz
            if (damageText != null)
            {
                damageText.text = finalDamage.ToString();

                // Güçlendiyse Yeþil Yap
                if (finalDamage > cardData.damage) damageText.color = Color.green;
                else damageText.color = Color.white;
            }
        }

        // Pozisyonu kaydet
        originalPosition = transform.position;
    }

    public void OnCardClicked()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlaySound(AudioManager.instance.cardClickSound);
        if (gameManager != null) gameManager.SelectCard(this);
    }

    public void Select()
    {
        if (isSelected) return;

        // Seçilince en öne gelsin
        transform.SetAsLastSibling();

        transform.position = originalPosition + new Vector3(0, 30f, 0);
        transform.localScale = originalScale * 1.1f;
        isSelected = true;
    }

    public void Deselect()
    {
        if (!isSelected) return;

        transform.position = originalPosition;
        transform.localScale = originalScale;
        isSelected = false;
    }
}