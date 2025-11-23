using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LevelStageManager : MonoBehaviour
{
    public static LevelStageManager Instance;

    public int UnlockedLevel { get; private set; } = 1;
    public int SelectedLevel { get; private set; } = 1;
    private int currentLevel;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadProgress();
        CheckForAppUpdate();
    }

    private void LoadProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        // Debug.Log("All PlayerPrefs reset on Start!");

        UnlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        SelectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
    }

    public void UnlockNextLevel()
    {
        // ✅ Unlock only if not already unlocked
        if (SelectedLevel >= UnlockedLevel)
        {
            UnlockedLevel = SelectedLevel + 1;
            PlayerPrefs.SetInt("UnlockedLevel", UnlockedLevel);
        }

        // ✅ Move current level forward
        SelectedLevel++;
        PlayerPrefs.SetInt("SelectedLevel", SelectedLevel);
        PlayerPrefs.Save();

        Debug.Log($"Current Level: {SelectedLevel - 1} -> Next: {SelectedLevel}");
        Debug.Log($"Unlocked Level: {UnlockedLevel}");
    }

    public void SetSelectedLevel(int level)
    {
        SelectedLevel = level;
        PlayerPrefs.SetInt("SelectedLevel", level);
        PlayerPrefs.Save();
        StartCoroutine(PlaySoundThenLoadScene(level));

    }

    private IEnumerator PlaySoundThenLoadScene(int levelNumber)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        yield return new WaitForSeconds(0.3f);

        string sceneName = GetSceneName(levelNumber);
        SceneManager.LoadScene(sceneName);
    }

    public string GetSceneName(int level)
    {
        if (level < 13) return "GameLevel3by3Scene";
        if (level < 29) return "GameLevel4by4Scene";
        return "GameLevel5by5Scene";
    }

    private void CheckForAppUpdate()
    {
        string currentVersion = Application.version; // version from Player Settings
        string savedVersion = PlayerPrefs.GetString("AppVersion", "");

        if (!string.IsNullOrEmpty(savedVersion) && savedVersion != currentVersion)
        {
            UpdatePlayerLevelsAfterUpdate();
        }

        PlayerPrefs.SetString("AppVersion", currentVersion);
        PlayerPrefs.Save();
    }

    private void UpdatePlayerLevelsAfterUpdate()
    {
        int savedSelected = PlayerPrefs.GetInt("SelectedLevel", 1);
        int savedUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (savedSelected > savedUnlocked)
        {
            savedSelected = savedUnlocked;
            PlayerPrefs.SetInt("SelectedLevel", savedSelected);
        }

        PlayerPrefs.SetInt("UnlockedLevel", savedUnlocked);
        PlayerPrefs.Save();

        Debug.Log($"Levels updated after app update: Selected={savedSelected}, Unlocked={savedUnlocked}");
    }


}
