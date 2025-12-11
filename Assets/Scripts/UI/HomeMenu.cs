using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

#if UNITY_ANDROID || UNITY_IOS
using Firebase;
using Firebase.Analytics;
#endif

public class HomeMenu : MonoBehaviour
{

    public PopupBoxUI coinsShopUI;
    public PopupBoxUI settingsUI;
    private int currentLevel;

    private void Start()
    {
        // Start background music when menu loads
        AudioManager.Instance.PlayBackgroundMusic();

    }

    // ---------------- BUTTON FUNCTIONS ----------------

    public void StartGame()
    {
#if UNITY_ANDROID || UNITY_IOS
        FirebaseInit.Instance.LogButtonClickedEvent("start_game");
#endif
        StartCoroutine(PlaySoundThenLoadGame());

    }

    public void LevelStage()
    {
#if UNITY_ANDROID || UNITY_IOS
        FirebaseInit.Instance.LogButtonClickedEvent("level_stage");
#endif
        StartCoroutine(PlaySoundThenLoad("LevelStageScene"));
    }

    public void HowToPlay()
    {
#if UNITY_ANDROID || UNITY_IOS
        FirebaseInit.Instance.LogButtonClickedEvent("how_to_play");
#endif
        StartCoroutine(PlaySoundThenLoad("HowToPlayScene"));
    }
    public void Achievement()
    {
        // StartCoroutine(PlaySoundThenLoad("AchievementScene"));
#if UNITY_ANDROID || UNITY_IOS
        FirebaseInit.Instance.LogButtonClickedEvent("achievement");
        GooglePlayManager.Instance.ShowAchievementsUI();
#endif

    }

    public void ShowCoinsShop()
    {
#if UNITY_ANDROID || UNITY_IOS
        FirebaseInit.Instance.LogButtonClickedEvent("show_coins_shop");
#endif
        AudioManager.Instance.PlayButtonClick();
        if (coinsShopUI != null) coinsShopUI.ShowPopup();
    }
    public void ClosedCoinsShop()
    {
        AudioManager.Instance.PlayButtonClick();
        if (coinsShopUI != null) coinsShopUI.ClosedPopup();
    }
    public void ShowSettings()
    {
        AudioManager.Instance.PlayButtonClick();
        if (settingsUI != null) settingsUI.ShowPopup();
    }
    public void ClosedSettings()
    {
        AudioManager.Instance.PlayButtonClick();
        if (settingsUI != null) settingsUI.ClosedPopup();
    }

    public void QuitGame()
    {
        AudioManager.Instance.PlayButtonClick();
        Debug.Log("Game Closed!");
        Application.Quit();
    }

    // ---------------- SCENE TRANSITIONS ----------------

    private IEnumerator PlaySoundThenLoadGame()
    {
        AudioManager.Instance.PlayButtonClick();
        yield return new WaitForSeconds(0.3f); // wait for sound to finish

        currentLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        string nextScene = currentLevel < 13 
                ? "GameLevel3by3Scene" 
                : currentLevel < 34 
                    ? "GameLevel4by4Scene" 
                    : "GameLevel5by5Scene";
        TransitionToScene(nextScene);
    }

    private IEnumerator PlaySoundThenLoad(string sceneName)
    {
        AudioManager.Instance.PlayButtonClick();
        yield return new WaitForSeconds(0.3f); // wait for sound to finish
        TransitionToScene(sceneName);
    }

    private void TransitionToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
