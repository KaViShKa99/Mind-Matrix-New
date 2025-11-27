using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;



public class GamePlayMenu : MonoBehaviour
{
    // Optional Pause Menu UI

    public Button pauseButton;      // Button component
    public Sprite pauseSprite;      // Image to show when game is running
    public Sprite playSprite;       // Image to show when game is paused
    public Image pauseButtonImage;  
    public PopupBoxUI pauseMenuUI;
    public PopupBoxUI rertyLoadingUI;
    public PopupBoxUI gameOverUI;
    public PopupBoxUI gameQuitUI;
    private int nextLevel;
    private int currentLevel;
    public HintSystem hintSystem;    // public LevelDetailsManager levelDetailsManager;



    public void GoToHome()
    {
        // LivesManager.Instance.ConsumeLife();

        StartCoroutine(PlaySoundThenLoad("HomeScreenScene"));
    }

    public void NextLevel()
    {
        // LevelStageManager.Instance.UnlockNextLevel();
        string sceneToLoad = LevelStageManager.Instance.GetSceneName(LevelStageManager.Instance.SelectedLevel);
        StartCoroutine(PlaySoundThenLoad(sceneToLoad));
    }

    public void Retry()
    {
        LevelDetailsManager.Instance.StopTimer();
        
        currentLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        PlayerPrefs.SetInt("SelectedLevel", currentLevel);

        string sceneToLoad = currentLevel < 13
            ? "GameLevel3by3Scene"
            : currentLevel < 29
                ? "GameLevel4by4Scene"
                : "GameLevel5by5Scene";

        // StartCoroutine(LoadLevelAfterAd(sceneToLoad));

        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.ShowInterstitial(() =>
            {
                StartCoroutine(LoadLevelAfterAd(sceneToLoad));
            });
        }
        else
        {
            Debug.LogWarning("⚠️ AdsManager not found — loading level directly.");
            StartCoroutine(LoadLevelAfterAd(sceneToLoad));
        }

        // if (GamePixAdsManager.Instance != null)
        // {
        //     GamePixAdsManager.Instance.ShowInterstitial(() =>
        //     {
        //         StartCoroutine(LoadLevelAfterAd(sceneToLoad));
        //     });
        // }
        // else
        // {
        //     Debug.LogWarning("⚠️ AdsManager not found — loading level directly.");
        //     StartCoroutine(LoadLevelAfterAd(sceneToLoad));
        // }
    }

    private IEnumerator LoadLevelAfterAd(string sceneName)
    {
        if (gameOverUI != null && gameOverUI.gameObject.activeSelf) gameOverUI.ClosedPopup();

        yield return new WaitForSeconds(0.5f);

        rertyLoadingUI?.ShowPopup();

        yield return new WaitForSeconds(1.2f);

        StartCoroutine(PlaySoundThenLoad(sceneName));


    }

    public void ShowHint()
    {
        LevelDetailsManager.Instance.StopTimer();
        AudioManager.Instance.PlayButtonClick();

        if (hintSystem != null) hintSystem.OnHintBtnClick();
    }

    public void GameQuitUI()
    {
        LevelDetailsManager.Instance.StopTimer();
        AudioManager.Instance.PlayButtonClick();

        if (gameQuitUI != null) gameQuitUI.ShowPopup();
    }

    public void ClosedGameQuitUI()
    {
        LevelDetailsManager.Instance.StartTimer();
        AudioManager.Instance.PlayButtonClick();

        if (gameQuitUI != null) gameQuitUI.ClosedPopup();
    }
    public void GamePause()
    {
        LevelDetailsManager.Instance.StopTimer();
        AudioManager.Instance.PlayButtonClick();

        if (pauseButtonImage != null)
        {
            if (pauseMenuUI != null)
                pauseButtonImage.sprite = playSprite;
            else
                pauseButtonImage.sprite = pauseSprite;
        }

        if (pauseMenuUI != null) pauseMenuUI.ShowPopup();
    }
    public void GamePlay()
    {
        LevelDetailsManager.Instance.StartTimer();
        AudioManager.Instance.PlayButtonClick();

        if (pauseButtonImage != null)
        {
            if (pauseMenuUI != null)
                pauseButtonImage.sprite = pauseSprite;
            else
                pauseButtonImage.sprite = playSprite;
        }

        if (pauseMenuUI != null) pauseMenuUI.ClosedPopup();
    }
    public void ClosedGamePauseUI()
    {
        LevelDetailsManager.Instance.StartTimer();
        AudioManager.Instance.PlayButtonClick();

        if (pauseButtonImage != null)
        {
            if (pauseMenuUI != null)
                pauseButtonImage.sprite = pauseSprite;
            else
                pauseButtonImage.sprite = playSprite;
        }

        if (pauseMenuUI != null) pauseMenuUI.ClosedPopup();
    }

    public void AdWatchBtn()
    {
        AudioManager.Instance.PlayButtonClick();
        if (hintSystem != null)
        {
            hintSystem.ClosedAdWatchPopup();
            hintSystem.ShowHintSteps();
            StartCoroutine(StartTimerAfterHint());
        }

    }
    public void ClosedAdWatchPopup()
    {
        AudioManager.Instance.PlayButtonClick();
        if (hintSystem != null) hintSystem.ClosedAdWatchPopup();
        StartCoroutine(StartTimerAfterHint());

    }

    private IEnumerator StartTimerAfterHint()
    {
        yield return new WaitForSeconds(1.0f);
        LevelDetailsManager.Instance.StartTimer();
    }

    private IEnumerator PlaySoundThenLoad(string sceneName)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
        else
        {
            Debug.LogWarning("AudioManager not found in scene!");
        }
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(sceneName);
    }

}
