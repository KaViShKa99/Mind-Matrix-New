// using UnityEngine;
// using System;
// using GamePix;

// public class GamePixAdsManager : MonoBehaviour
// {
//     public static GamePixAdsManager Instance { get; private set; }

//     // Events
//     public event Action InterstitialClosed;
//     public event Action RewardEarned;

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     private void Start()
//     {
// #if UNITY_WEBGL && !UNITY_EDITOR
//         InitializeAds();
// #endif
//     }

//     #region Initialization
//     private void InitializeAds()
//     {
//         Debug.Log("Initializing GamePix GPX Ads...");

//         LoadBanner();
//         LoadInterstitial();
//         LoadRewarded();
//     }
//     #endregion

//     #region Banner
//     public void LoadBanner()
//     {
//         Debug.Log("📢 Showing GamePix Sticky Banner");
//     }

//     public void HideBanner()
//     {
//         Debug.Log("📢 Hiding GamePix Sticky Banner");
//     }
//     #endregion

//     #region Interstitial
//     public void LoadInterstitial()
//     {
//         // GPX auto-handles interstitial loading
//         Debug.Log("🎬 GamePix Interstitial ready (auto-managed)");
//     }

//     public void ShowInterstitial(Action onAdClosed = null)
//     {
//         Debug.Log("🎬 Requesting Interstitial...");
//         InterstitialClosed += onAdClosed;

//         Gpx.Ads.InterstitialAd(() =>
//         {
//             Debug.Log("📢 Interstitial closed!");
//             InterstitialClosed?.Invoke();
//             InterstitialClosed = null; // Clear subscribers
//         });
//     }
//     #endregion

//     #region Rewarded Ads
//     public void LoadRewarded()
//     {
//         // GPX auto-handles rewarded ad loading
//         Debug.Log("🎁 GamePix Rewarded ready (auto-managed)");
//     }

//     public void ShowRewardedAd(Action onSuccess)
//     {
//         RewardEarned += onSuccess;
//         Gpx.Ads.RewardAd(OnRewardAdSuccess, OnRewardAdFail);
//     }

//     private void OnRewardAdSuccess()
//     {
//         Debug.Log("🎉 Reward received!");
//         RewardEarned?.Invoke();
//         RewardEarned = null; // Clear subscribers
//     }

//     private void OnRewardAdFail()
//     {
//         Debug.LogWarning("❌ Reward not available. Try again later.");
//     }
//     #endregion
// }
