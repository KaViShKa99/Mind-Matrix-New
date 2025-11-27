using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;


public class LevelStageUI : MonoBehaviour
{
    public TextMeshProUGUI stageName;
    public GameObject stage1Container;
    public GameObject stage2Container;
    public GameObject stage3Container;
    public Button levelButtonPrefab; // assign prefab
    public GameObject leftBtn;
    public GameObject rightBtn;

    private int currentStageIndex = 1;
    private List<LevelButton> levelButtons = new List<LevelButton>();

    private void Start()
    {
        CreateLevelButtons();
        ShowStage(currentStageIndex);
    }
    private void OnEnable()
    {
        RefreshLevelButtons();
    }

    private void CreateLevelButtons()
    {
        GameObject[] stageContainers = { stage1Container, stage2Container, stage3Container };
        int[,] stageRanges = { {1, 12}, {13, 28}, {29, 58} }; // stage min and max levels

        int unlocked = LevelStageManager.Instance.UnlockedLevel;

        for (int stage = 0; stage < stageContainers.Length; stage++)
        {
            GameObject container = stageContainers[stage];
            int start = stageRanges[stage, 0];
            int end = stageRanges[stage, 1];

            for (int i = start; i <= end; i++)
            {
                Button btn = Instantiate(levelButtonPrefab, container.transform);
                int levelNumber = i; // capture for lambda

                LevelButton levelBtn = btn.GetComponent<LevelButton>();
                levelButtons.Add(levelBtn);

                bool isUnlocked = levelNumber <= unlocked;
                levelBtn.Setup(levelNumber, isUnlocked);

                btn.onClick.AddListener(() =>
                {
                    LevelStageManager.Instance.SetSelectedLevel(levelNumber);
                    // Optionally: LevelStageManager.Instance.LoadSelectedLevel();
                });
            }
        }
    }
    
    public void RefreshLevelButtons()
    {

        PlayerPrefs.SetInt("UnlockedLevel", 50);
        PlayerPrefs.Save();

        int unlocked = LevelStageManager.Instance.UnlockedLevel;
        Debug.Log("refresh " + unlocked);

        foreach (LevelButton btn in levelButtons)
        {
            int levelNum = int.Parse(btn.levelText.text);
            bool isUnlocked = levelNum <= unlocked;

            btn.gameObject.SetActive(isUnlocked); // hide locked buttons
            btn.GetComponent<Button>().interactable = isUnlocked;

            if (btn.lockIcon != null)
                btn.lockIcon.SetActive(!isUnlocked);
        }
    }

    public void LeftMove()
    {
        AudioManager.Instance.PlayButtonClick();

        if (currentStageIndex > 1)
        {
            currentStageIndex--;
            ShowStage(currentStageIndex);
        }
    }

    public void RightMove()
    {
        AudioManager.Instance.PlayButtonClick();

        if (currentStageIndex < 3)
        {
            currentStageIndex++;
            ShowStage(currentStageIndex);
        }
    }

    private void ShowStage(int index)
    {
        stage1Container.SetActive(false);
        stage2Container.SetActive(false);
        stage3Container.SetActive(false);

        switch (index)
        {
            case 1:
                stage1Container.SetActive(true);
                stageName.text = "3x3 Puzzles";
                leftBtn.SetActive(false);
                rightBtn.SetActive(true);
                break;
            case 2:
                stage2Container.SetActive(true);
                stageName.text = "4x4 Puzzles";
                leftBtn.SetActive(true);
                rightBtn.SetActive(true);
                break;
            case 3:
                stage3Container.SetActive(true);
                stageName.text = "5x5 Puzzles";
                leftBtn.SetActive(true);
                rightBtn.SetActive(false);
                break;
        }
    }

    public void GoToHome()
    {
        StartCoroutine(PlaySoundThenLoad("HomeScreenScene"));
    }

    private IEnumerator PlaySoundThenLoad(string sceneName)
    {
        AudioManager.Instance.PlayButtonClick();
        yield return new WaitForSeconds(0.3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    


}
