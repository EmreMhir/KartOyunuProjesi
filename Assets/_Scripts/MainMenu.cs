using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Ana Menü Butonlarý")]
    public Button continueButton;

    [Header("Ayarlar Paneli")]
    public GameObject settingsPanel;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        UpdateButtonState();
        settingsPanel.SetActive(false); // Baþlangýçta ayarlar kapalý olsun

        // --- SES AYARLARINI BAÞLAT ---
        if (AudioManager.instance != null)
        {
            // Slider'larý mevcut ses seviyesine eþitle (0 ile 1 arasý)
            musicSlider.value = AudioManager.instance.musicSource.volume;
            sfxSlider.value = AudioManager.instance.sfxSource.volume;

            // Slider oynatýldýðýnda çalýþacak fonksiyonlarý ekle
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    // --- SES FONKSÝYONLARI ---
    public void SetMusicVolume(float volume)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SetMusicVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SetSFXVolume(volume);
    }

    // --- DÝÐER FONKSÝYONLAR (Aynen Kalýyor) ---
    void UpdateButtonState()
    {
        if (SaveManager.HasSaveData()) continueButton.interactable = true;
        else continueButton.interactable = false;
    }

    public void StartNewGame()
    {
        SaveManager.DeleteAllSaveData();
        UpdateButtonState();
        if (AudioManager.instance != null) AudioManager.instance.PlayMusic(AudioManager.instance.battleMusic);
        SceneManager.LoadScene("GameScene");
    }

    public void ContinueGame()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlayMusic(AudioManager.instance.battleMusic);
        SceneManager.LoadScene("GameScene");
    }

    public void OpenSettings() { settingsPanel.SetActive(true); }
    public void CloseSettings() { settingsPanel.SetActive(false); }
    public void QuitGame()
    {
        Debug.Log("OYUNDAN ÇIKILIYOR..."); // Editörde çalýþtýðýný görmek için
        Application.Quit();
    }
}