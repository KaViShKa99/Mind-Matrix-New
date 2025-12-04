// using UnityEngine;
// using GoogleMobileAds.Api;
// using System;
// using UnityEngine.SceneManagement;

// public class AdsManager : MonoBehaviour
// {
//     public static AdsManager Instance;

//     private BannerView bannerView;
//     private InterstitialAd interstitialAd;
//     private RewardedAd rewardedAd;

//     private bool isInitialized = false;

//     public event Action OnInterstitialClosed;
//     public event Action OnRewardedAdEarned;

//     // ✅ Test ad unit IDs (replace with yours later)
//     // private readonly string bannerAdUnitId = "ca-app-pub-4782366325141566/4822005892";
//     // private readonly string interstitialAdUnitId = "ca-app-pub-4782366325141566/8262557459";
//     // private readonly string rewardedAdUnitId = "ca-app-pub-4782366325141566/6100973132";
//     // real app id = ca-app-pub-4782366325141566~4553863840

//     // fake app id ca-app-pub-3940256099942544~3347511713 
//     private readonly string bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
//     private readonly string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
//     private readonly string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject); // ✅ Keeps AdsManager alive across scenes
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }


//     private void Start()
//     {
//         InitializeAds();
//     }

//     private void InitializeAds()
//     {
//         if (isInitialized) return;

//         MobileAds.Initialize(initStatus =>
//         {
//             Debug.Log("✅ Google Mobile Ads initialized");
//             LoadBanner();
//             LoadInterstitial();
//             LoadRewarded();
//         });

//         isInitialized = true;
//     }

//     // ============================================================
//     // 📱 BANNER AD
//     // ============================================================
//     public void LoadBanner()
//     {
//         bannerView?.Destroy(); // cleanup before reload

//         bannerView = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);
//         AdRequest adRequest = new AdRequest();

//         bannerView.LoadAd(adRequest);
//         bannerView.OnBannerAdLoaded += () => Debug.Log("📢 Banner loaded successfully.");
//         bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
//             Debug.LogError("❌ Banner failed to load: " + error);
//     }




//     public void HideBanner() => bannerView?.Hide();

//     // ============================================================
//     // 🧱 INTERSTITIAL AD (Full Screen)
//     // ============================================================
//     private void LoadInterstitial()
//     {
//         interstitialAd?.Destroy();
//         interstitialAd = null;

//         AdRequest adRequest = new AdRequest();
//         InterstitialAd.Load(interstitialAdUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
//         {
//             if (error != null || ad == null)
//             {
//                 Debug.LogWarning("⚠️ Interstitial failed to load: " + error);
//                 return;
//             }

//             interstitialAd = ad;
//             Debug.Log("✅ Interstitial loaded.");

//             interstitialAd.OnAdFullScreenContentClosed += () =>
//             {
//                 Debug.Log("🔁 Interstitial closed, reloading...");
//                 OnInterstitialClosed?.Invoke();
//                 OnInterstitialClosed = null;
//                 LoadInterstitial();
//             };

//             interstitialAd.OnAdFullScreenContentFailed += (AdError adError) =>
//             {
//                 Debug.LogError("⚠️ Interstitial failed to show: " + adError);
//                 LoadInterstitial();
//             };
//         });
//     }

//     public void ShowInterstitial(Action onAdClosed = null)
//     {
//         if (interstitialAd != null && interstitialAd.CanShowAd())
//         {
//             if (onAdClosed != null)
//                 OnInterstitialClosed += onAdClosed;

//             interstitialAd.Show();
//         }
//         else
//         {
//             Debug.LogWarning("⚠️ Interstitial not ready — reloading...");
//             LoadInterstitial();
//             onAdClosed?.Invoke();
//         }
//     }

//     // ============================================================
//     // 🎁 REWARDED AD
//     // ============================================================
//     private void LoadRewarded()
//     {
//         rewardedAd = null;
//         AdRequest adRequest = new AdRequest();

//         RewardedAd.Load(rewardedAdUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
//         {
//             if (error != null || ad == null)
//             {
//                 Debug.LogWarning("⚠️ Rewarded failed to load: " + error);
//                 return;
//             }

//             rewardedAd = ad;
//             Debug.Log("✅ Rewarded ad loaded.");

//             rewardedAd.OnAdFullScreenContentClosed += () =>
//             {
//                 Debug.Log("🔁 Rewarded closed, reloading...");
//                 LoadRewarded();
//             };

//             rewardedAd.OnAdFullScreenContentFailed += (AdError adError) =>
//             {
//                 Debug.LogError("⚠️ Rewarded failed to show: " + adError);
//                 LoadRewarded();
//             };
//         });
//     }

//     public void ShowRewarded(Action onRewardEarned = null)
//     {
//         if (rewardedAd != null && rewardedAd.CanShowAd())
//         {
//             rewardedAd.Show((Reward reward) =>
//             {
//                 Debug.Log($"🎁 Reward earned: {reward.Type} x {reward.Amount}");
//                 onRewardEarned?.Invoke();
//                 OnRewardedAdEarned?.Invoke();
//             });
//         }
//         else
//         {
//             Debug.LogWarning("⚠️ Rewarded ad not ready — reloading...");
//             LoadRewarded();
//         }
//     }

//     // ============================================================
//     // 🧹 CLEANUP
//     // ============================================================
//     private void OnDestroy()
//     {
//         bannerView?.Destroy();
//         interstitialAd?.Destroy();
//         rewardedAd = null;
//     }
// }

using System;
using UnityEngine;
using GoogleMobileAds.Api;
// using GoogleMobileAds.Mediation; // optional, depending on package


public class AdsManager : MonoBehaviour
{
    // Singleton instance
    public static AdsManager Instance { get; private set; }

    [Header("Ad Unit IDs")]
    // public string bannerAdUnitId;
    // public string interstitialAdUnitId;
    // public string rewardedAdUnitId;

    // private readonly string bannerAdUnitId = "ca-app-pub-4782366325141566/4822005892";
    // private readonly string interstitialAdUnitId = "ca-app-pub-4782366325141566/8262557459";
    // private readonly string rewardedAdUnitId = "ca-app-pub-4782366325141566/6100973132";

    // real app id = ca-app-pub-4782366325141566~4553863840

    // fake app id ca-app-pub-3940256099942544~3347511713 

    private  string bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
    private  string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
    private  string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    private bool isInitialized = false;

    // Events
    public event Action OnInterstitialClosed;
    public event Action OnRewardedAdEarned;

    // ===============================
    // 🟢 SINGLETON & INITIALIZE ADS
    // ===============================
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAds();
    }

    private void InitializeAds()
    {
        if (isInitialized) return;

        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("✅ Google Mobile Ads initialized (mediation enabled)");
            LoadBanner();
            LoadInterstitial();
            LoadRewarded();
        });

        isInitialized = true;
    }

    // ===============================
    // 📱 BANNER AD
    // ===============================
    public void LoadBanner()
    {
        bannerView?.Destroy();

        // AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        // bannerView = new BannerView(bannerAdUnitId, adaptiveSize, AdPosition.Bottom);


        bannerView = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);
        AdRequest request = new AdRequest();

        bannerView.LoadAd(request);

        bannerView.OnBannerAdLoaded += () => Debug.Log("📢 Banner loaded successfully");
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            Debug.LogError($"❌ Banner failed to load: {error}");
    }

    public void ShowBanner() => bannerView?.Show();
    public void HideBanner() => bannerView?.Hide();

    // ===============================
    // 🧱 INTERSTITIAL AD
    // ===============================
    private void LoadInterstitial()
    {
        interstitialAd?.Destroy();
        interstitialAd = null;

        AdRequest request = new AdRequest();
        InterstitialAd.Load(interstitialAdUnitId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogWarning($"⚠️ Interstitial failed to load: {error}");
                return;
            }

            interstitialAd = ad;
            Debug.Log("✅ Interstitial loaded");

            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("🔁 Interstitial closed, reloading...");
                OnInterstitialClosed?.Invoke();
                OnInterstitialClosed = null;
                LoadInterstitial();
            };

            interstitialAd.OnAdFullScreenContentFailed += (AdError adError) =>
            {
                Debug.LogError($"⚠️ Interstitial failed to show: {adError}");
                LoadInterstitial();
            };

            interstitialAd.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log($"💰 Interstitial paid: {adValue.Value} {adValue.CurrencyCode}, Precision: {adValue.Precision}");
            };
        });
    }

    public void ShowInterstitial(Action onAdClosed = null)
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            if (onAdClosed != null) OnInterstitialClosed += onAdClosed;
            interstitialAd.Show();
        }
        else
        {
            Debug.LogWarning("⚠️ Interstitial not ready — reloading...");
            LoadInterstitial();
            onAdClosed?.Invoke();
        }
    }

    // ===============================
    // 🎁 REWARDED AD
    // ===============================
    private void LoadRewarded()
    {
        rewardedAd = null;

        AdRequest request = new AdRequest();
        RewardedAd.Load(rewardedAdUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogWarning($"⚠️ Rewarded failed to load: {error}");
                return;
            }

            rewardedAd = ad;
            Debug.Log("✅ Rewarded ad loaded");

            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("🔁 Rewarded closed, reloading...");
                LoadRewarded();
            };

            rewardedAd.OnAdFullScreenContentFailed += (AdError adError) =>
            {
                Debug.LogError($"⚠️ Rewarded failed to show: {adError}");
                LoadRewarded();
            };

            rewardedAd.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log($"💰 Rewarded paid: {adValue.Value} {adValue.CurrencyCode}, Precision: {adValue.Precision}");
            };
        });
    }

    public void ShowRewarded(Action onRewardEarned = null)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"🎁 Reward earned: {reward.Type} x {reward.Amount}");
                onRewardEarned?.Invoke();
                OnRewardedAdEarned?.Invoke();
            });
        }
        else
        {
            Debug.LogWarning("⚠️ Rewarded ad not ready — reloading...");
            LoadRewarded();
        }
    }

    // ===============================
    // 🧹 CLEANUP
    // ===============================
    private void OnDestroy()
    {
        bannerView?.Destroy();
        interstitialAd?.Destroy();
        rewardedAd = null;
    }

    // ===============================
    // 🔹 MEDIATION TEST SUITE
    // ===============================
    // public void ShowMediationTestSuite()
    // {
    //     MediationTestSuite.Show();
    // }
}
