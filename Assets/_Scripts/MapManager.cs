using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    [Header("Seviye Butonlarý")]
    public Button[] levelButtons; 
    void Start()
    {
        // HARÝTAYA GÝRÝNCE MENÜ MÜZÝÐÝ ÇALSIN
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusic(AudioManager.instance.mainMenuMusic);
        }

        UpdateMap();
    }

    void UpdateMap()
    {
        // Kayýtlý seviyeyi öðren (Eðer hiç kayýt yoksa 1 döner)
        int currentLevel = SaveManager.LoadLevel();

        // Tüm butonlarý kontrol et
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNumber = i + 1; // Dizi 0'dan baþlar, seviye 1'den.

            if (levelNumber <= currentLevel)
            {
                // Oyuncu bu seviyeye gelmiþse butonu aç
                levelButtons[i].interactable = true;
            }
            else
            {
                // Henüz gelmediði seviyeleri kilitle
                levelButtons[i].interactable = false;
            }
        }
    }

    // Butonlara týklandýðýnda çalýþacak fonksiyon
    public void SelectLevel(int level)
    {
        // Buradan direkt sahneyi yüklüyoruz.
        // Müzik çalma iþini artýk GameManager'ýn Start fonksiyonu yapacak.
        SceneManager.LoadScene("GameScene");
    }

    public void BackToMenu()
    {
        // Ana menüye dönerken zaten müzik çalýyor ama garanti olsun diye býrakabiliriz
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusic(AudioManager.instance.mainMenuMusic);
        }
        SceneManager.LoadScene("MainMenu");
    }
}