using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public GameObject lockIcon; // assign lock icon in prefab
    private int levelNumber;
    private Button button;
    private Color normalColor;



    public void Setup(int number, bool isUnlocked)
    {
        levelNumber = number;
        levelText.text = number.ToString();

        if (button == null)
            button = GetComponent<Button>();

        // Save the button’s normal color
        normalColor = button.colors.normalColor;

        // Keep button looking normal, even if not interactable
        var colors = button.colors;
        colors.disabledColor = normalColor;
        button.colors = colors;

        // Apply unlock state
        button.interactable = isUnlocked;
        if (lockIcon != null)
            lockIcon.SetActive(!isUnlocked);
    }

    public void OnClick()
    {
        if (!GetComponent<Button>().interactable)
            return;

        LevelStageManager.Instance.SetSelectedLevel(levelNumber);
    }
}
