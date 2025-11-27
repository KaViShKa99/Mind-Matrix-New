// using UnityEngine;
// using TMPro;
// using System;

// public class LivesTextUI : MonoBehaviour
// {
//     public TMP_Text livesText;
//     public TMP_Text refillTimerText; // shows next life refill countdown

//     private void Start()
//     {
//         // Subscribe to lives changes
//         LivesManager.Instance.OnLivesChanged += UpdateLivesUI;

//         // Start updating the refill timer every second
//         InvokeRepeating(nameof(UpdateRefillTimerUI), 0f, 1f);

//         // Initial UI update
//         UpdateLivesUI(LivesManager.Instance.GetLives());
//     }

//     private void OnDestroy()
//     {
//         if (LivesManager.Instance != null)
//             LivesManager.Instance.OnLivesChanged -= UpdateLivesUI;

//         CancelInvoke(nameof(UpdateRefillTimerUI));
//     }

//     private void UpdateLivesUI(int newLives)
//     {
//         livesText.text = newLives.ToString();

//         // Hide timer if lives full
//         if (newLives >= LivesManager.Instance.maxLives)
//         {
//             refillTimerText.text = "Full";
//         }
//     }

//     private void UpdateRefillTimerUI()
//     {
//         int currentLives = LivesManager.Instance.GetLives();

//         if (currentLives >= LivesManager.Instance.maxLives)
//         {
//             refillTimerText.text = "Full";
//             return;
//         }

//         long secondsLeft = LivesManager.Instance.GetSecondsUntilNextLife();

//         TimeSpan time = TimeSpan.FromSeconds(secondsLeft);
//         refillTimerText.text = $"{time.Minutes:D2}:{time.Seconds:D2}";
//     }
// }
