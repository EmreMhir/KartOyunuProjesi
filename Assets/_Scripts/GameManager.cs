using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton

    [Header("Paneller")]
    public GameObject NewVictoryPanel;  // Normal Zafer Ekraný
    public GameObject NewDefeatPanel;   // Kaybetme Ekraný
    public GameObject GameFinishPanel;  // Final Bitiþ Ekraný
    public GameObject pauseMenuPanel;

    [Header("Ayarlar")]
    public int finalLevelIndex = 10;    // Oyun kaçýncý bölümde bitsin?

    [Header("Efektler")]
    public GameObject damageTextPrefab; // Hasar yazýsý prefabý
    public Transform canvasTransform;
    public Transform playerDamageSpawnPoint; // Oyuncunun hasar noktasý

    [Header("Oyun Verileri")]
    public List<CardData> gameDeck;
    public GameObject cardPrefab;
    public List<MonsterData> monsterLevels;

    [Header("Sahne Referanslarý")]
    public Player player;
    public Monster monster;
    public Transform cardSpawnParent;
    public List<Transform> cardPositions;
    public EnemyDisplay enemyDisplay;

    [Header("UI Referanslarý")]
    public TextMeshProUGUI critRateText; // Kritik oraný yazýsý
    // Not: Zafer ekranýndaki altýn yazýsýný buradan kaldýrdýk.

    private CardDisplay[] handSlots = new CardDisplay[5];
    private List<CardDisplay> selectedCards = new List<CardDisplay>();
    private bool isGameOver = false;
    private int currentLevel;

    public static bool isGamePaused = false;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        // --- MÜZÝK ---
        if (AudioManager.instance != null && AudioManager.instance.battleMusic != null)
            AudioManager.instance.PlayMusic(AudioManager.instance.battleMusic);

        // --- REFERANSLAR ---
        if (player == null) player = FindFirstObjectByType<Player>();
        if (monster == null) monster = FindFirstObjectByType<Monster>();

        // --- UI ---
        int currentCrit = SaveManager.LoadCritChance();
        if (critRateText != null) critRateText.text = "Kritik: %" + currentCrit;

        // --- SEVÝYE YÜKLEME ---
        currentLevel = SaveManager.LoadLevel();
        Debug.Log("Seviye: " + currentLevel);

        int baseMaxHealth = 100;
        int bonusMaxHealth = SaveManager.LoadMaxHealthBonus();
        int totalMaxHealth = baseMaxHealth + bonusMaxHealth;

        int loadedPlayerHealth = SaveManager.LoadBattleState().playerHealth;
        if (loadedPlayerHealth <= 0) loadedPlayerHealth = totalMaxHealth;

        player.Setup(loadedPlayerHealth, totalMaxHealth);

        // --- CANAVAR YÜKLEME ---
        int monsterIndex = currentLevel - 1;
        int monsterHealthToLoad = SaveManager.LoadBattleState().monsterHealth;

        if (monsterIndex < monsterLevels.Count)
        {
            MonsterData currentMonsterData = monsterLevels[monsterIndex];
            if (enemyDisplay != null) enemyDisplay.SetupEnemy(currentMonsterData);

            if (monsterHealthToLoad <= 0)
            {
                monster.Setup(currentMonsterData);
                player.Heal(1000);
            }
            else
            {
                monster.Setup(currentMonsterData, monsterHealthToLoad);
            }
        }
        else
        {
            MonsterData firstMonster = monsterLevels[0];
            monster.Setup(firstMonster);
            if (enemyDisplay != null) enemyDisplay.SetupEnemy(firstMonster);
        }

        // Panelleri Gizle
        if (NewVictoryPanel != null) NewVictoryPanel.SetActive(false);
        if (NewDefeatPanel != null) NewDefeatPanel.SetActive(false);
        if (GameFinishPanel != null) GameFinishPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

        DrawInitialHand();
    }

    // --- EFEKTLER ---
    public void ShowFloatingText(int damage, Vector3 position)
    {
        if (damageTextPrefab != null && canvasTransform != null)
        {
            GameObject textObj = Instantiate(damageTextPrefab, position, Quaternion.identity);
            textObj.transform.SetParent(canvasTransform, false);
            textObj.transform.position = position;
            textObj.GetComponent<DamageText>().Setup(damage);
        }
    }

    public void Saldir()
    {
        if (selectedCards.Count == 0) return;
        PlaySelectedCards();
    }

    // --- OYUN SONU YÖNETÝMÝ ---
    void HandleGameOver(bool playerWon)
    {
        isGameOver = true;
        if (AudioManager.instance != null) AudioManager.instance.musicSource.Stop();

        if (playerWon)
        {
            // --- ALTIN SÝSTEMÝ (ARKAPLANDA KAYDEDÝLÝR) ---
            int currentGold = SaveManager.LoadGold();

            // 1. Level: 100 Altýn, Her seviyede %25 artar.
            float multiplier = Mathf.Pow(1.25f, currentLevel - 1);
            int rewardGold = Mathf.RoundToInt(100f * multiplier);

            SaveManager.SaveGold(currentGold + rewardGold);
            Debug.Log("Kazanýlan Altýn (Gizli): " + rewardGold);
            // ----------------------------------------------

            // FÝNAL KONTROLÜ
            if (currentLevel >= finalLevelIndex)
            {
                Debug.Log("FÝNAL!");
                if (GameFinishPanel != null)
                {
                    GameFinishPanel.SetActive(true);
                    GameFinishPanel.transform.SetAsLastSibling();
                }
            }
            else
            {
                // NORMAL SEVÝYE ATLAMA
                currentLevel++;
                SaveManager.SaveLevel(currentLevel);
                SaveManager.SaveBattleState(currentLevel, player.currentHealth, 0);

                if (NewVictoryPanel != null)
                {
                    NewVictoryPanel.SetActive(true);
                    NewVictoryPanel.transform.SetAsLastSibling();
                    // Burada artýk yazý deðiþtirme kodu YOK.
                }
            }
        }
        else
        {
            // KAYBETME
            if (NewDefeatPanel != null)
            {
                NewDefeatPanel.SetActive(true);
                NewDefeatPanel.transform.SetAsLastSibling();
            }
        }
    }

    public void CheckForWinner()
    {
        if (isGameOver) return;
        if (monster.currentHealth <= 0) { HandleGameOver(true); }
        else if (player.currentHealth <= 0) { HandleGameOver(false); }
    }

    // --- DÝÐER FONKSÝYONLAR ---
    public void PauseGame()
    {
        if (isGameOver) return;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    public void QuitToMenuFromPause()
    {
        Time.timeScale = 1f;
        SaveManager.SaveBattleState(currentLevel, player.currentHealth, monster.currentHealth);
        GoToMainMenu();
    }

    public void ReturnToMenuAndSave()
    {
        Time.timeScale = 1f;
        SaveManager.SaveBattleState(currentLevel, player.currentHealth, monster.currentHealth);
        GoToMainMenu();
    }

    public void RestartLevel()
    {
        // 1. ZAMANI TEKRAR AKIT (Bunu unutursak oyun donuk baþlar)
        Time.timeScale = 1;

        // 2. Oyuncunun CANINI FULLE
        // Temel can (100) + Varsa bonuslar
        int maxHealth = 100 + SaveManager.LoadMaxHealthBonus();

        // 3. Canavarýn CANINI SIFIRLA (0 gönderince taze canavar gelir)
        SaveManager.SaveBattleState(currentLevel, maxHealth, 0);

        // 4. Müzik çalýyorsa durdur
        if (AudioManager.instance != null)
            AudioManager.instance.musicSource.Stop();

        // 5. Sahneyi Yeniden Yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void GoToMarket() { SceneManager.LoadScene("MarketScene"); }
    public void GoToMap() { SceneManager.LoadScene("MapScene"); }
    public void GoToMainMenu()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlayMusic(AudioManager.instance.mainMenuMusic);
        SceneManager.LoadScene("MainMenu");
    }

    #region Kart Mantýðý
    void DrawInitialHand() { for (int i = 0; i < handSlots.Length; i++) DrawOneCard(i); }

    void DrawOneCard(int slotIndex)
    {
        if (gameDeck.Count == 0 || handSlots[slotIndex] != null) return;
        int randomIndex = Random.Range(0, gameDeck.Count);
        CardData randomCard = gameDeck[randomIndex];
        GameObject newCardObject = Instantiate(cardPrefab, cardSpawnParent);
        newCardObject.transform.position = cardPositions[slotIndex].position;
        newCardObject.transform.localPosition = new Vector3(newCardObject.transform.localPosition.x, newCardObject.transform.localPosition.y, 0f);
        newCardObject.transform.localScale = Vector3.one;
        newCardObject.transform.rotation = cardPositions[slotIndex].rotation;
        CardDisplay cardDisplay = newCardObject.GetComponent<CardDisplay>();
        cardDisplay.cardData = randomCard;
        handSlots[slotIndex] = cardDisplay;
    }

    public void SelectCard(CardDisplay card)
    {
        if (isGameOver || isGamePaused) return;
        if (selectedCards.Count > 0 && selectedCards[0].cardData.name != card.cardData.name)
        {
            foreach (CardDisplay selectedCard in selectedCards) selectedCard.Deselect();
            selectedCards.Clear();
        }
        if (selectedCards.Contains(card)) { selectedCards.Remove(card); card.Deselect(); }
        else { selectedCards.Add(card); card.Select(); }
    }

    public void PlaySelectedCards()
    {
        if (selectedCards.Count == 0 || isGameOver || isGamePaused) return;

        CardData data = selectedCards[0].cardData;
        var upgrades = SaveManager.LoadUpgrades();

        // 1. Temel Hasarý Hesapla
        int damagePerCard = data.damage + upgrades.cardBonus;
        if (data.name.Contains("Okçu")) damagePerCard += SaveManager.LoadArcherBonus();
        if (data.name.Contains("Büyücü")) damagePerCard += SaveManager.LoadMageBonus();

        int totalEffectValue = damagePerCard * selectedCards.Count;

        // 2. Standart Kombo Bonuslarý
        if (selectedCards.Count == 3) totalEffectValue += 5;
        else if (selectedCards.Count == 4) totalEffectValue += 15;
        else if (selectedCards.Count >= 5) totalEffectValue += 40;

        // ---------------------------------------------------------
        // YENÝ ÖZELLÝK: CANAVAR CANINA GÖRE %5 BONUS 
        // ---------------------------------------------------------
        if (data.cardType == CardType.Attack)
        {
            if (selectedCards.Count == 3 || selectedCards.Count >= 5)
            {
                if (monster != null)
                {
                    // HATA DÜZELTME: monster.maxHealth yerine listeden çekiyoruz
                    int currentMonsterIndex = currentLevel - 1;

                    // Güvenlik: Liste hatasý olmasýn diye kontrol ediyoruz
                    if (currentMonsterIndex >= 0 && currentMonsterIndex < monsterLevels.Count)
                    {
                        int maxHealthOfTarget = monsterLevels[currentMonsterIndex].maxHealth;
                        int percentageBonus = Mathf.RoundToInt(maxHealthOfTarget * 0.05f);

                        // En az 1 vursun
                        if (percentageBonus < 1) percentageBonus = 1;

                        totalEffectValue += percentageBonus;
                        Debug.Log("%5 Dev Bonusu Eklendi: " + percentageBonus);
                    }
                }
            }
        }
        // ---------------------------------------------------------

        switch (data.cardType)
        {
            case CardType.Attack:
                // Kritik Vuruþ Hesabý
                int roll = Random.Range(0, 100);
                if (roll < SaveManager.LoadCritChance())
                {
                    totalEffectValue *= 2;
                    Debug.Log("KRÝTÝK!");
                }

                if (monster != null)
                {
                    monster.TakeDamage(totalEffectValue);

                    if (monster.monsterImage != null)
                        ShowFloatingText(totalEffectValue, monster.monsterImage.transform.position);
                    else
                        ShowFloatingText(totalEffectValue, monster.transform.position);
                }

                if (AudioManager.instance != null)
                {
                    if (data.name.Contains("Büyücü")) AudioManager.instance.PlaySound(AudioManager.instance.magicAttackSound);
                    else if (data.name.Contains("Okçu")) AudioManager.instance.PlaySound(AudioManager.instance.archerAttackSound);
                    else AudioManager.instance.PlaySound(AudioManager.instance.warriorAttackSound);
                }
                break;

            case CardType.Defense:
                player.AddBlock(totalEffectValue);
                AudioManager.instance.PlaySound(AudioManager.instance.defenseSound);
                break;

            case CardType.Heal:
                player.Heal(totalEffectValue);
                AudioManager.instance.PlaySound(AudioManager.instance.healSound);
                break;
        }

        foreach (CardDisplay c in selectedCards)
        {
            for (int i = 0; i < handSlots.Length; i++) if (handSlots[i] == c) handSlots[i] = null;
            Destroy(c.gameObject);
        }
        selectedCards.Clear();

        if (!isGameOver) EndPlayerTurn();
    }

    void EndPlayerTurn()
    {
        for (int i = 0; i < handSlots.Length; i++) if (handSlots[i] == null) DrawOneCard(i);
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(0.5f);
        if (isGameOver) yield break;
        bool isCritical = monster.IsCriticalNext();
        if (isCritical)
        {
            Transform targetVisual = monster.monsterImage.transform;
            Vector3 startPos = targetVisual.localPosition;
            Vector3 targetPos = startPos + new Vector3(0, 150f, 0);
            float elapsed = 0f;
            while (elapsed < 0.6f) { targetVisual.localPosition = Vector3.Lerp(startPos, targetPos, elapsed / 0.6f); elapsed += Time.deltaTime; yield return null; }
            yield return new WaitForSeconds(0.2f);
            elapsed = 0f;
            while (elapsed < 0.1f) { targetVisual.localPosition = Vector3.Lerp(targetPos, startPos, elapsed / 0.1f); elapsed += Time.deltaTime; yield return null; }
            targetVisual.localPosition = startPos;
            if (CameraShake.instance != null) CameraShake.instance.Salla(0.4f, 15f);
            if (AudioManager.instance != null) AudioManager.instance.PlaySound(AudioManager.instance.monsterImpactSound);
        }
        int damage = monster.CalculateAttackDamage();
        player.TakeDamage(damage);
        if (playerDamageSpawnPoint != null) ShowFloatingText(damage, playerDamageSpawnPoint.position);
        else ShowFloatingText(damage, player.transform.position);
        if (AudioManager.instance != null) AudioManager.instance.PlaySound(AudioManager.instance.monsterAttackSound);
        yield return new WaitForSeconds(0.5f);
    }
    #endregion
}