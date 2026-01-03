using UnityEngine;

public static class SaveManager
{
    // Kayýt anahtarlarýmýz
    private const string LevelKey = "PlayerLevel";
    private const string GoldKey = "PlayerGold";
    private const string CritChanceKey = "PlayerCritChance";
    private const string CardBonusDamageKey = "PlayerCardBonusDamage";
    private const string PlayerMaxHealthKey = "PlayerMaxHealth";

    // YENÝ EKLENEN ANAHTARLAR: Savaþ ortasý verileri için
    private const string PlayerCurrentHealthKey = "PlayerCurrentHealth";
    private const string MonsterCurrentHealthKey = "MonsterCurrentHealth";

    // --- SAVAÞ DURUMUNU KAYDETME VE YÜKLEME (YENÝ SÝSTEM) ---

    // Bu fonksiyon, "Menü" butonuna basýldýðýnda GameManager tarafýndan çaðrýlýr
    public static void SaveBattleState(int level, int playerHealth, int monsterHealth)
    {
        PlayerPrefs.SetInt(LevelKey, level);
        PlayerPrefs.SetInt(PlayerCurrentHealthKey, playerHealth);
        PlayerPrefs.SetInt(MonsterCurrentHealthKey, monsterHealth);
        PlayerPrefs.Save();
        Debug.Log("Savaþ Durumu Kaydedildi! Seviye: " + level + ", Oyuncu Caný: " + playerHealth + ", Canavar Caný: " + monsterHealth);
    }

    // Bu fonksiyon, oyun baþladýðýnda GameManager tarafýndan çaðrýlýr
    public static (int level, int playerHealth, int monsterHealth) LoadBattleState()
    {
        int level = PlayerPrefs.GetInt(LevelKey, 1);
        // Eðer daha önce can deðeri kaydedilmemiþse, -1 döndürür.
        // Bu, GameManager'ýn yeni bir savaþ mý yoksa devam eden bir savaþ mý olduðunu anlamasýný saðlar.
        int playerHealth = PlayerPrefs.GetInt(PlayerCurrentHealthKey, -1);
        int monsterHealth = PlayerPrefs.GetInt(MonsterCurrentHealthKey, -1);
        return (level, playerHealth, monsterHealth);
    }


    // --- DÝÐER KAYIT FONKSÝYONLARI (Ayný Kalýyor) ---

    // Bu fonksiyon, bir seviye KAZANILDIÐINDA çaðrýlýr
    public static void SaveLevel(int level)
    {
        PlayerPrefs.SetInt(LevelKey, level);
        // Savaþ bittiði için, savaþ ortasý can kayýtlarýný temizleyelim
        PlayerPrefs.DeleteKey(PlayerCurrentHealthKey);
        PlayerPrefs.DeleteKey(MonsterCurrentHealthKey);
        PlayerPrefs.Save();
    }
    public static int LoadLevel() { return PlayerPrefs.GetInt(LevelKey, 1); }
    public static void SaveGold(int gold) { PlayerPrefs.SetInt(GoldKey, gold); PlayerPrefs.Save(); }
    public static int LoadGold() { return PlayerPrefs.GetInt(GoldKey, 0); }
    public static void SavePlayerMaxHealth(int maxHealth) { PlayerPrefs.SetInt(PlayerMaxHealthKey, maxHealth); PlayerPrefs.Save(); }
    public static int LoadPlayerMaxHealth() { return PlayerPrefs.GetInt(PlayerMaxHealthKey, 100); }
    public static void SaveUpgrades(float critChance, int cardBonus) { PlayerPrefs.SetFloat(CritChanceKey, critChance); PlayerPrefs.SetInt(CardBonusDamageKey, cardBonus); PlayerPrefs.Save(); }
    public static (float critChance, int cardBonus) LoadUpgrades() { float critChance = PlayerPrefs.GetFloat(CritChanceKey, 0f); int cardBonus = PlayerPrefs.GetInt(CardBonusDamageKey, 0); return (critChance, cardBonus); }


    // --- GENEL YÖNETÝM ---
    public static bool HasSaveData() { return PlayerPrefs.HasKey(LevelKey); }

    public static void DeleteAllSaveData()
    {
        PlayerPrefs.DeleteAll(); // En temizi, TÜM kayýtlarý siler.
        Debug.Log("Tüm kayýt verileri silindi.");
    }
    // --- YENÝ MARKET ÖZELLÝKLERÝ ÝÇÝN KAYIT SÝSTEMÝ ---

    // 1. OKÇU BONUSU
    public static void SaveArcherBonus(int amount) { PlayerPrefs.SetInt("ArcherBonus", amount); }
    public static int LoadArcherBonus() { return PlayerPrefs.GetInt("ArcherBonus", 0); }

    // 2. BÜYÜCÜ BONUSU
    public static void SaveMageBonus(int amount) { PlayerPrefs.SetInt("MageBonus", amount); }
    public static int LoadMageBonus() { return PlayerPrefs.GetInt("MageBonus", 0); }

    // 3. MAX CAN BONUSU (Satýn alýnan ekstra can)
    public static void SaveMaxHealthBonus(int amount) { PlayerPrefs.SetInt("MaxHealthBonus", amount); }
    public static int LoadMaxHealthBonus() { return PlayerPrefs.GetInt("MaxHealthBonus", 0); }

    // 4. KRÝTÝK ÝHTÝMALÝ (0 ile 100 arasýnda)
    public static void SaveCritChance(int chance) { PlayerPrefs.SetInt("CritChance", chance); }
    public static int LoadCritChance() { return PlayerPrefs.GetInt("CritChance", 0); } // Baþlangýçta 0
}