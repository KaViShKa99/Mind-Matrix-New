using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Firebase;
using Firebase.Analytics;

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
        FirebaseInit.Instance.LogButtonClickedEvent("start_game");
        StartCoroutine(PlaySoundThenLoadGame());

    }

    public void LevelStage()
    {
        FirebaseInit.Instance.LogButtonClickedEvent("level_stage");
        StartCoroutine(PlaySoundThenLoad("LevelStageScene"));
    }

    public void HowToPlay()
    {
        FirebaseInit.Instance.LogButtonClickedEvent("how_to_play");
        StartCoroutine(PlaySoundThenLoad("HowToPlayScene"));
    }
    public void Achievement()
    {
        FirebaseInit.Instance.LogButtonClickedEvent("achievement");
        // StartCoroutine(PlaySoundThenLoad("AchievementScene"));
        GooglePlayManager.Instance.ShowAchievementsUI();
    }

    public void ShowCoinsShop()
    {
        FirebaseInit.Instance.LogButtonClickedEvent("show_coins_shop");
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
        // string nextScene = currentLevel < 13 ? "GameLevel3by3Scene" : "GameLevel4by4Scene";
        string nextScene = currentLevel < 13 
                ? "GameLevel3by3Scene" 
                : currentLevel < 29 
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
