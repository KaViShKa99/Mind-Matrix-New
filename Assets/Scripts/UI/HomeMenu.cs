using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HomeMenu : MonoBehaviour
{
    private int currentLevel;

    private void Start()
    {
        // Start background music when menu loads
        AudioManager.Instance.PlayBackgroundMusic();
    }

    // ---------------- BUTTON FUNCTIONS ----------------

    public void StartGame()
    {
        StartCoroutine(PlaySoundThenLoadGame());
    }

    public void LevelStage()
    {
        StartCoroutine(PlaySoundThenLoad("LevelStageScene"));
    }

    public void HowToPlay()
    {
        StartCoroutine(PlaySoundThenLoad("HowToPlayScene"));
    }
    public void Achievement()
    {
        // StartCoroutine(PlaySoundThenLoad("AchievementScene"));
        GooglePlayManager.Instance.ShowAchievementsUI();
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
