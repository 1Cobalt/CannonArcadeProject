using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("UI Настройки")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Источники Звука")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Аудио Клипы (Музыка)")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    [Header("Аудио Клипы (Звуки)")]
    public AudioClip rockBreakSound;
    public AudioClip buttonClickSound;
    public AudioClip coinSound;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- МАГИЯ СМЕНЫ СЦЕН ---
    // Эти два метода (OnEnable и OnDisable) подписывают нас на событие загрузки сцены

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Этот метод Unity вызывает САМА каждый раз, когда загружается новая сцена
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckMusic(scene.name);

        // Также нужно заново найти слайдеры, если мы вернулись в меню
        FindSliders();
    }

    // -------------------------

    void Start()
    {
        if (musicSlider != null)
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);

        if (sfxSlider != null)
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        LoadVolume();
        // Первая проверка при запуске игры
        CheckMusic(SceneManager.GetActiveScene().name);
    }

    void CheckMusic(string sceneName)
    {
        AudioClip clipToPlay = null;

        // Логика выбора трека
        // Если сцена называется "Menu" или "MainMenu" - играем музыку меню
        if (sceneName == "MainMenu" || sceneName == "Menu")
        {
            clipToPlay = menuMusic;
        }
        else
        {
            // Во всех остальных сценах (Level1, Game, BossLevel) - играем боевую музыку
            clipToPlay = gameMusic;
        }

        // Если музыка та же самая (например, рестарт уровня), не прерываем её
        if (musicSource.clip != clipToPlay)
        {
            musicSource.clip = clipToPlay;
            musicSource.Play();
        }
    }

    // Этот метод ищет слайдеры на новой сцене, чтобы они работали
    void FindSliders()
    {
 
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    // --- УПРАВЛЕНИЕ ГРОМКОСТЬЮ ---

    public void OnMusicSliderChange(float value)
    {
        musicSource.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    public void OnSFXSliderChange(float value)
    {
        sfxSource.volume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    void LoadVolume()
    {
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    // --- ЗВУКИ ---
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip);
    }

    public void PlayRockBreak() { PlaySFX(rockBreakSound); }
    public void PlayCoinSound() { PlaySFX(coinSound); }
    public void PlayClickSound() { PlaySFX(buttonClickSound); }
}