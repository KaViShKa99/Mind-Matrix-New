using UnityEngine;
using System.Collections;

public class PopupBoxController : MonoBehaviour
{
    public Animator star1Animator;
    public Animator star2Animator;
    public Animator star3Animator;

    public void OnPopupOpenComplete() // Called by Animation Event
    {
        // StartCoroutine(PlayStarsSequentially());
    }

    private IEnumerator PlayStarsSequentially()
    {
        // Wait 0.8s after popup appears
        yield return new WaitForSeconds(10f);

        // Play each star with sound delay
        star1Animator.SetTrigger("StarPop");
        PlayStarSound(1);

        yield return new WaitForSeconds(5f);
        star2Animator.SetTrigger("StarPop");
        PlayStarSound(2);

        yield return new WaitForSeconds(5f);
        star3Animator.SetTrigger("StarPop");
        PlayStarSound(3);
    }

    private void PlayStarSound(int index)
    {
        // Optional – play sound for each star pop
        Debug.Log("✨ Star " + index + " sound!");
        // Example:
        // AudioManager.Instance.Play("StarPop");
    }
}
