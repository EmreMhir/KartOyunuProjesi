using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Müzikler")]
    public AudioClip mainMenuMusic;
    public AudioClip battleMusic;

    [Header("Genel Ses Efektleri")]
    public AudioClip monsterImpactSound; // <-- YENÝ: Yere vurma (GÜM) sesi
    public AudioClip cardClickSound;

    [Header("Canavar Sesleri")]
    public AudioClip monsterAttackSound;

    [Header("Özel Kart Sesleri")]
    public AudioClip magicAttackSound;   // Büyücü
    public AudioClip archerAttackSound;  // Okçu
    public AudioClip warriorAttackSound; // Kýlýç

    [Header("Savunma/Ýyileþme Sesleri")]
    public AudioClip healSound; // Heal
    public AudioClip defenseSound; // Kalkan
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayMusic(mainMenuMusic);
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public void PlayMusic(AudioClip musicClip)
    {
        // Eðer çalan þarký zaten aynýsýysa...
        if (musicSource.clip == musicClip)
        {
            // ...ama müzik DURMUÞSA (Restart yüzünden), tekrar baþlat!
            if (!musicSource.isPlaying) musicSource.Play();
            return;
        }

        // Þarký farklýysa deðiþtir ve çal
        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip);
    }
}