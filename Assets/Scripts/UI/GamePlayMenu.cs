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
    public PopupBoxUI gameRetryPopup;
    public PopupBoxUI hintShowPopup;
    public PopupBoxUI autoReplacePopup;
    public PopupBoxUI undoPopup;
    public PopupBoxUI CoinsNotEnoughPopup;
    public PopupBoxUI noMovesToUndoPopup;

    private int nextLevel;
    private int currentLevel;
    public HintSystem hintSystem;  
    public TileManager tileManager;  



    public void GoToHome()
    {
        // LivesManager.Instance.ConsumeLife();
        StartCoroutine(PlaySoundThenLoad("HomeScreenScene"));
    }

    public void NextLevel()
    {
        // LevelStageManager.Instance.UnlockNextLevel();
        
        LevelPlayCounter.Instance.OnLevelPlayed();

        string sceneToLoad = LevelStageManager.Instance.GetSceneName(LevelStageManager.Instance.SelectedLevel);
        StartCoroutine(PlaySoundThenLoad(sceneToLoad));
    }

    public void Retry()
    {
        // LevelPlayCounter.Instance.skipNextStartCount = true;

        // LevelPlayCounter.Instance.OnLevelPlayed();

        LevelPlayCounter.Instance.OnLevelReset();

        LevelDetailsManager.Instance.StopTimer();

        // currentLevel = LevelDetailsManager.Instance.GetLevel();
        string sceneToLoad = LevelStageManager.Instance.GetSceneName(LevelDetailsManager.Instance.GetLevel());

        
        // string sceneToLoad = currentLevel < 13
        //     ? "GameLevel3by3Scene"
        //     : currentLevel < 34
        //         ? "GameLevel4by4Scene"
        //         : "GameLevel5by5Scene";

        StartCoroutine(LoadLevelAfterAd(sceneToLoad));

        // if (AdsManager.Instance != null)
        // {
        //     AdsManager.Instance.ShowRewarded(() =>
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

        yield return new WaitForSeconds(0.8f);

        StartCoroutine(PlaySoundThenLoad(sceneName));


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
 
    //////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ///////////////////////////// Hint Popup ////////////////////////////
    /// </summary>
    public void ShowHintPopup()
    {
        LevelDetailsManager.Instance.StopTimer();
        AudioManager.Instance.PlayButtonClick();

        if (hintShowPopup != null) hintShowPopup.ShowPopup();
    }

    public void ClosedHintPopup()
    {
        AudioManager.Instance.PlayButtonClick();
        if (hintShowPopup != null) hintShowPopup.ClosedPopup();
        // StartCoroutine(StartTimerAfterHint());
        LevelDetailsManager.Instance.StartTimer();


    }
    public void AdWatchBtnInHintPopup()
    {
        LevelDetailsManager.Instance.StopTimer();
        LevelDetailsManager.Instance.AddAdWatch();
        LevelDetailsManager.Instance.AddHint();


        AudioManager.Instance.PlayButtonClick();
        if (hintSystem != null)
        {
            ClosedHintPopup();         
            hintSystem.ShowHintsAfterAd();
            StartCoroutine(StartTimerAfterHint());
        }
    }

    public void CoinSpentBtnInHintPopup()
    {
        AudioManager.Instance.PlayButtonClick();
        if (CoinManager.Instance.SpendCoins(150))
        {
            // LevelDetailsManager.Instance.AddAdWatch();
            LevelDetailsManager.Instance.AddHint();

#if UNITY_ANDROID || UNITY_IOS
            GooglePlayManager.Instance.playerData.coins = CoinManager.Instance.coins;
            GooglePlayManager.Instance.SaveGame(GooglePlayManager.Instance.playerData);
#endif

            ClosedHintPopup();         
            hintSystem.ShowHintsAfterCoinSpent();
            StartCoroutine(StartTimerAfterHint());
        }else
        {
            ClosedHintPopup(); 
            LevelDetailsManager.Instance.StopTimer();  
            StartCoroutine(StartCoinNotEnoughPopup());
        }

    }
    ///////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// /////////////////////////////  Coins Not Enough Popup ////////////////////////////
    /// </summary>


    private IEnumerator StartCoinNotEnoughPopup()
    {
        yield return new WaitForSeconds(0.5f);
        if (CoinsNotEnoughPopup != null) CoinsNotEnoughPopup.ShowPopup();
    }

    public void ClosedCoinsNotEnoughPopup()
    {
        AudioManager.Instance.PlayButtonClick();
        if (CoinsNotEnoughPopup != null) CoinsNotEnoughPopup.ClosedPopup();
        LevelDetailsManager.Instance.StartTimer();


    }

    public void AdWatchBtnInHintsCoinsNotEnoughPopup()
    {
        LevelDetailsManager.Instance.AddAdWatch();
        LevelDetailsManager.Instance.AddHint();

        AudioManager.Instance.PlayButtonClick();
        if (hintSystem != null)
        {
            ClosedCoinsNotEnoughPopup();         
            hintSystem.ShowHintsAfterAd();
            StartCoroutine(StartTimerAfterHint());
        }
    }
    public void AdWatchBtnInAutoReplaceCoinsNotEnoughPopup()
    {
        LevelDetailsManager.Instance.AddAdWatch();
        LevelDetailsManager.Instance.AddHint();

        AudioManager.Instance.PlayButtonClick();
        if (hintSystem != null)
        {
            ClosedCoinsNotEnoughPopup();         
            hintSystem.ShowAutoPlaceAfterAd(1);
            StartCoroutine(StartTimerAfterHint());
        }
    }

    ///////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////
    
    private IEnumerator StartTimerAfterHint()
    {
        yield return new WaitForSeconds(1.0f);
        LevelDetailsManager.Instance.StartTimer();
    }
    /// <summary>
    /// ///////////////////////////// auto place Buttons ////////////////////////////
    /// </summary>
    public void ShowAutoPlacePopup()
    {
        // hintSystem.OnAutoPlace1();
        LevelDetailsManager.Instance.StopTimer();
        AudioManager.Instance.PlayButtonClick();

        if (autoReplacePopup != null) autoReplacePopup.ShowPopup();
    }

    public void ClosedAutoPlacePopup()
    {
        AudioManager.Instance.PlayButtonClick();
        if (autoReplacePopup != null) autoReplacePopup.ClosedPopup();
        LevelDetailsManager.Instance.StartTimer();


    }
    public void AdWatchBtnInAutoPlacePopup()
    {
        LevelDetailsManager.Instance.StopTimer();
        LevelDetailsManager.Instance.AddAdWatch();
        LevelDetailsManager.Instance.AddHint();


        AudioManager.Instance.PlayButtonClick();
        if (hintSystem != null)
        {
            ClosedAutoPlacePopup();         
            hintSystem.ShowAutoPlaceAfterAd(1);
            StartCoroutine(StartTimerAfterHint());
        }
    }

    public void CoinSpentBtnInAutoPlacePopup()
    {
        AudioManager.Instance.PlayButtonClick();
        if (CoinManager.Instance.SpendCoins(200))
        {
            // LevelDetailsManager.Instance.AddAdWatch();
            LevelDetailsManager.Instance.AddHint();

#if UNITY_ANDROID || UNITY_IOS
            GooglePlayManager.Instance.playerData.coins = CoinManager.Instance.coins;
            GooglePlayManager.Instance.SaveGame(GooglePlayManager.Instance.playerData);
#endif

            ClosedAutoPlacePopup();         
            hintSystem.OnAutoPlace1();
            StartCoroutine(StartTimerAfterHint());
        }else
        {
            ClosedAutoPlacePopup(); 
            LevelDetailsManager.Instance.StopTimer();  
            StartCoroutine(StartCoinNotEnoughPopup());
        }

    }


    /// <summary>
    /// /////////////////////////// undo Buttons ////////////////////////////
    /// </summary>
    /// 

    public void ShowNoMovesToUndoPopup()
    {
        LevelDetailsManager.Instance.StopTimer();
        if (noMovesToUndoPopup != null) noMovesToUndoPopup.ShowPopup();

    }

    public void ClosedNoMovesToUndoPopup()
    {
        AudioManager.Instance.PlayButtonClick();
        if (noMovesToUndoPopup != null) noMovesToUndoPopup.ClosedPopup();
        LevelDetailsManager.Instance.StartTimer();
    }

    public void ShowUndoPopup()
    {
        AudioManager.Instance.PlayButtonClick();

        int moves = tileManager.GetMoveCount();

        if (moves > 0)
        {
            LevelDetailsManager.Instance.StopTimer();
            undoPopup?.ShowPopup();

            // hintSystem.ShowUndoAfterCoinSpent(1);

        }
        else
        {
            // Show small warning
            Debug.Log("No moves to undo!");
            ShowNoMovesToUndoPopup();
        }
    }

    public void ClosedUndoPopup()
    {
        AudioManager.Instance.PlayButtonClick();
        if (undoPopup != null) undoPopup.ClosedPopup();
        LevelDetailsManager.Instance.StartTimer();
    }
    public void AdWatchBtnInUndoPopup()
    {
        LevelDetailsManager.Instance.StopTimer();
        LevelDetailsManager.Instance.AddAdWatch();
        LevelDetailsManager.Instance.AddHint(); 

        AudioManager.Instance.PlayButtonClick();
        if (hintSystem != null)
        {
            ClosedUndoPopup();         
            hintSystem.ShowUndoAfterAd(1);
            StartCoroutine(StartTimerAfterHint());
        }
    }
    public void CoinSpentBtnInUndoPopup()
    {
        AudioManager.Instance.PlayButtonClick();
        if (CoinManager.Instance.SpendCoins(250))
        {
            LevelDetailsManager.Instance.AddHint();
#if UNITY_ANDROID || UNITY_IOS
            GooglePlayManager.Instance.playerData.coins = CoinManager.Instance.coins;        
            GooglePlayManager.Instance.SaveGame(GooglePlayManager.Instance.playerData);
#endif  
            ClosedUndoPopup();         
            hintSystem.OnUndo1();
            StartCoroutine(StartTimerAfterHint());
        }else
        {
            ClosedUndoPopup(); 
            LevelDetailsManager.Instance.StopTimer();  
            StartCoroutine(StartCoinNotEnoughPopup());
        }

    }


    /// <summary>
    /// ///////////////////////////// Sound and Load Scene ////////////////////////////
    /// </summary>

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
