using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RewardButtonsManager : MonoBehaviour
{
    private const string KEY_DAY = "reward_day";

    // Claim flags
    private const string KEY_100 = "reward_100_used";
    private const string KEY_500 = "reward_500_used";
    private const string KEY_1000 = "reward_1000_used";

    // Ad watch counters
    private const string ADS100 = "ads100";
    private const string ADS500 = "ads500";
    private const string ADS1000 = "ads1000";

    // Last refresh timestamp key
    private const string KEY_LAST_REFRESH = "last_refresh_timestamp";

    private float refreshTime = 150f; // refresh every 150 seconds
    private DateTime lastRefreshTime;

    [Header("UI Buttons")]
    public Button btn100;
    public Button btn500;
    public Button btn1000;

    [Header("Button Texts")]
    public TMP_Text txt100;
    public TMP_Text txt500;
    public TMP_Text txt1000;
    public TMP_Text timerText;

    private void Start()
    {
        ResetDailyIfNeeded();
        LoadLastRefreshTime();
        UpdateAllUI();
    }

    private void Update()
    {
        // Calculate remaining time using DateTime.Now
        TimeSpan elapsed = DateTime.Now - lastRefreshTime;
        float remaining = refreshTime - (float)elapsed.TotalSeconds;

        if (remaining <= 0f)
        {
            // Refresh buttons and reset last refresh time
            RefreshAllButtons();
            lastRefreshTime = DateTime.Now;
        }
        else
        {
            // Update timer text in MM:SS format
            int minutes = Mathf.FloorToInt(remaining / 60f);
            int seconds = Mathf.FloorToInt(remaining % 60f);
            timerText.text = $"Next Refresh: {minutes:00}:{seconds:00}";
        }
    }

    void LoadLastRefreshTime()
    {
        if (PlayerPrefs.HasKey(KEY_LAST_REFRESH))
        {
            string savedTime = PlayerPrefs.GetString(KEY_LAST_REFRESH);
            lastRefreshTime = DateTime.Parse(savedTime);
        }
        else
        {
            lastRefreshTime = DateTime.Now;
            PlayerPrefs.SetString(KEY_LAST_REFRESH, lastRefreshTime.ToString());
        }
    }

    void ResetDailyIfNeeded()
    {
        int today = DateTime.Now.DayOfYear;
        int last = PlayerPrefs.GetInt(KEY_DAY, -1);

        if (today != last)
        {
            PlayerPrefs.SetInt(KEY_DAY, today);

            PlayerPrefs.SetInt(KEY_100, 0);
            PlayerPrefs.SetInt(KEY_500, 0);
            PlayerPrefs.SetInt(KEY_1000, 0);

            PlayerPrefs.SetInt(ADS100, 0);
            PlayerPrefs.SetInt(ADS500, 0);
            PlayerPrefs.SetInt(ADS1000, 0);

            lastRefreshTime = DateTime.Now;
            PlayerPrefs.SetString(KEY_LAST_REFRESH, lastRefreshTime.ToString());
        }
    }

    // -------------------------------------------------------------
    // BUTTON LOGIC & ADS
    // -------------------------------------------------------------
    public void OnReward100() => WatchAdReward(KEY_100, ADS100, 1, 100, "reward_100");
    public void OnReward500() => WatchAdReward(KEY_500, ADS500, 2, 500, "reward_500");
    public void OnReward1000() => WatchAdReward(KEY_1000, ADS1000, 3, 1000, "reward_1000");

    void WatchAdReward(string claimKey, string adsKey, int neededAds, int reward, string eventName)
    {
#if UNITY_ANDROID || UNITY_IOS
        FirebaseInit.Instance.LogButtonClickedEvent(eventName);
#endif

        if (PlayerPrefs.GetInt(claimKey, 0) == 1) return;

        int watched = PlayerPrefs.GetInt(adsKey, 0);

        ShowAd(() =>
        {
            watched++;
            PlayerPrefs.SetInt(adsKey, watched);

            if (watched >= neededAds)
            {
                EarnCoins(reward);
                PlayerPrefs.SetInt(claimKey, 1);
            }

            UpdateAllUI();
        });
    }

    void ShowAd(System.Action onComplete)
    {
        AdsManager.Instance.ShowRewarded(onComplete);
    }

    void EarnCoins(int amount)
    {
        CoinManager.Instance.AddCoins(amount);

#if UNITY_ANDROID || UNITY_IOS        
        GooglePlayManager.Instance.playerData.coins = CoinManager.Instance.coins;
        GooglePlayManager.Instance.SaveGame(GooglePlayManager.Instance.playerData);
#endif

    }

    void UpdateAllUI()
    {
        UpdateButton(btn100, txt100, KEY_100, ADS100, 1, "100 Coins");
        UpdateButton(btn500, txt500, KEY_500, ADS500, 2, "500 Coins");
        UpdateButton(btn1000, txt1000, KEY_1000, ADS1000, 3, "1000 Coins");
    }

    void UpdateButton(Button btn, TMP_Text txt, string claimKey, string adsKey, int neededAds, string rewardText)
    {
        bool claimed = PlayerPrefs.GetInt(claimKey, 0) == 1;
        int watched = PlayerPrefs.GetInt(adsKey, 0);

        if (claimed)
        {
            txt.text = "Claimed";
            btn.interactable = false;
            btn.image.color = Color.gray;
        }
        else
        {
            txt.text = rewardText + $" ({watched}/{neededAds})";
            btn.interactable = true;
            btn.image.color = Color.white;
        }
    }

    void RefreshAllButtons()
    {
        PlayerPrefs.SetInt(KEY_100, 0);
        PlayerPrefs.SetInt(KEY_500, 0);
        PlayerPrefs.SetInt(KEY_1000, 0);

        PlayerPrefs.SetInt(ADS100, 0);
        PlayerPrefs.SetInt(ADS500, 0);
        PlayerPrefs.SetInt(ADS1000, 0);

        UpdateAllUI();
        Debug.Log("Reward buttons refreshed!");
    }
}
