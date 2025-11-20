using UnityEngine;
using TMPro;

public class LevelDetailsUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI moveText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI levelCount;

    private void Start()
    {
        var mgr = LevelDetailsManager.Instance;
        if (mgr == null) return;

        // subscribe
        mgr.OnMoveChanged += UpdateMoveText;
        mgr.OnTimeChanged += UpdateTimeText;
        mgr.OnLevelChanged += UpdateLevelText;

        // initialize UI with current values
        UpdateMoveText(mgr.GetMoveCount());
        UpdateTimeText(mgr.GetTimerCount());
        UpdateLevelText(mgr.GetLevel());
    }

    private void OnDestroy()
    {
        var mgr = LevelDetailsManager.Instance;
        if (mgr == null) return;

        // unsubscribe
        mgr.OnMoveChanged -= UpdateMoveText;
        mgr.OnTimeChanged -= UpdateTimeText;
        mgr.OnLevelChanged -= UpdateLevelText;
    }

    private void UpdateLevelText(int level)
    {
        if (levelCount != null)
            levelCount.text = $"{level}";
    }

    private void UpdateMoveText(int moves)
    {
        if (moveText != null)
            moveText.text = $"{moves}";
    }

    private void UpdateTimeText(float elapsedTime)
    {
        if (timeText == null) return;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timeText.text = $"{minutes:00}:{seconds:00}";
    }
}