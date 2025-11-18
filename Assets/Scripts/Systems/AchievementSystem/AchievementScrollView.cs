using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class AchievementScrollView : MonoBehaviour
{
    public RectTransform content;
    public GameObject achievementPrefab;
    public ScrollRect scrollRect;
    public AchievementPopupUI popup;

    public List<AchievementData> achievements = new List<AchievementData>();

    void Start()
    {
        Populate();
    }

    void Populate()
    {
        foreach (var a in achievements)
        {
            GameObject item = Instantiate(achievementPrefab, content);

            item.GetComponent<AchievementPrefabUI>()
                .Init(a, ShowPopup);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        scrollRect.verticalNormalizedPosition = 1;
    }

    public void ShowPopup(AchievementData data)
    {            
        AudioManager.Instance.PlayButtonClick();
        popup.ShowPopup(data);
    }

    public void ClosedPopup()
    {
        AudioManager.Instance.PlayButtonClick();
        popup.ClosedPopup();
    }

    public void GoToHome()
    {
        StartCoroutine(PlaySoundThenLoad("HomeScreenScene"));
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
