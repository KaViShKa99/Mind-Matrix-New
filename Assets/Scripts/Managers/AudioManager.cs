using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource; // For background music
    public AudioSource sfxSource;   // For SFX (clicks, moves, etc.)

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip buttonClickSound;
    public AudioClip tileMoveSound;
    public AudioClip gameWonSound;
    public AudioClip gameOverSound;

    private void Awake()
    {
        // Ensure a single persistent instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    // 🎵 Play looping background music
    public void PlayBackgroundMusic()
    {
        if (musicSource == null)
        {
            Debug.LogWarning("❌ MusicSource not assigned!");
            return;
        }

        if (backgroundMusic == null)
        {
            Debug.LogWarning("❌ BackgroundMusic clip not assigned!");
            return;
        }

        if (!musicSource.gameObject.activeInHierarchy)
        {
            // Debug.Log("🎵 MusicSource was disabled — enabling now...");
            musicSource.gameObject.SetActive(true);
        }

        if (!musicSource.isPlaying)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = 0.5f;
            musicSource.Play();
            // Debug.Log("🎶 Background music started");
        }
    }


    // 🔊 General method to play SFX
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("❌ Tried to play a null SFX clip!");
            return;
        }

        if (sfxSource == null)
        {
            Debug.LogWarning("❌ SFXSource not assigned!");
            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    // 🖱️ UI button click
    public void PlayButtonClick()
    {
        // Debug.Log("🔘 Button click sound: " + (buttonClickSound != null ? buttonClickSound.name : "Missing Clip"));
        PlaySFX(buttonClickSound);
    }

    // 🧩 Tile move
    public void PlayTileSlide()
    {
        // Debug.Log("🧱 Tile move sound: " + (tileMoveSound != null ? tileMoveSound.name : "Missing Clip"));
        PlaySFX(tileMoveSound);
    }

    // ☠️ Game over
    public void PlayGameOver()
    {
        // Debug.Log("💀 Game over sound: " + (gameOverSound != null ? gameOverSound.name : "Missing Clip"));
        PlaySFX(gameOverSound);
    }

    // 🏆 Level complete / win
    public void PlayLevelComplete()
    {
        // Debug.Log("🏆 Level complete sound: " + (gameWonSound != null ? gameWonSound.name : "Missing Clip"));
        PlaySFX(gameWonSound);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 🔁 If music stopped (e.g., new scene), restart it
        if (!musicSource.isPlaying)
        {
            PlayBackgroundMusic();
            // Debug.Log($"🎵 Restarted music after scene load: {scene.name}");
        }
    }

    // 🔇 Turn OFF background music (SFX still works)
    public void MuteBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();        // stops music
            musicSource.mute = true;   // ensures it stays silent
        }
    }

    // 🔊 Turn ON background music again
    public void UnmuteBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.mute = false;
            PlayBackgroundMusic();
        }
    }


}
