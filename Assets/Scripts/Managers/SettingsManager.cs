using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Status Text")]
    public TMP_Text musicStatusText;
    public TMP_Text sfxStatusText;

    private void Start()
    {
        // Load saved states (default to ON if not saved)
        int musicState = PlayerPrefs.GetInt("MusicState", 1);
        int sfxState = PlayerPrefs.GetInt("SfxState", 1);

        // Set sliders to saved state
        musicSlider.value = musicState;
        sfxSlider.value = sfxState;

        // Apply states to AudioManager
        ApplyMusicState(musicState);
        ApplySfxState(sfxState);

        // Update status text
        UpdateMusicText(musicState);
        UpdateSfxText(sfxState);
    }

    // ---------------- MUSIC TOGGLE ----------------

    public void OnMusicToggle(float _)
    {
        if (!Application.isPlaying) return;

        int newValue = (int)musicSlider.value;

        ApplyMusicState(newValue);
        UpdateMusicText(newValue);

        // Save state
        PlayerPrefs.SetInt("MusicState", newValue);
        PlayerPrefs.Save();
    }

    private void ApplyMusicState(int state)
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.musicSource.mute = (state == 0);
    }

    private void UpdateMusicText(int state)
    {
        musicStatusText.text = state == 1 ? "MUSIC : ON" : "MUSIC : OFF";
    }

    // ---------------- SFX TOGGLE ----------------

    public void OnSfxToggle(float _)
    {
        if (!Application.isPlaying) return;

        int newValue = (int)sfxSlider.value;

        ApplySfxState(newValue);
        UpdateSfxText(newValue);

        // Save state
        PlayerPrefs.SetInt("SfxState", newValue);
        PlayerPrefs.Save();
    }

    private void ApplySfxState(int state)
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.sfxSource.mute = (state == 0);
    }

    private void UpdateSfxText(int state)
    {
        sfxStatusText.text = state == 1 ? "SFX : ON" : "SFX : OFF";
    }
}
