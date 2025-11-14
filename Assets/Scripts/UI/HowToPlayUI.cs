using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class HowToPlayUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BackToHome()
    {
        StartCoroutine(PlaySoundThenLoad("HomeScreenScene"));

    }


    private IEnumerator PlaySoundThenLoad(string sceneName)
    {
        // 🟢 Safety check before using AudioManager
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
        else
        {
            Debug.LogWarning("AudioManager not found in scene!");
        }

        // ⏳ Small delay to allow sound to play before changing scene
        yield return new WaitForSeconds(0.3f);

        // 🔁 Now load next scene
        SceneManager.LoadScene(sceneName);
    }
    
    
}
