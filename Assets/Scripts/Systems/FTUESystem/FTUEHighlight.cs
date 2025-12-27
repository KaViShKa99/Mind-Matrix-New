using UnityEngine;

public class FTUEHighlight : MonoBehaviour
{
    public GameObject circle;

    public void Show(Transform target)
    {
        circle.SetActive(true);
        circle.transform.position = target.position;
    }

    public void Hide()
    {
        circle.SetActive(false);
    }
}
