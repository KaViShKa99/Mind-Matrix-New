using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("UI Buttons")]
    public Button btn100;
    public Button btn500;
    public Button btn1000;

    [Header("Button Texts")]
    public TMP_Text txt100;
    public TMP_Text txt500;
    public TMP_Text txt1000;

    private void Start()
    {
        ResetDailyIfNeeded();
        UpdateAllUI();
    }

    // -------------------------------------------------------------
    // DAILY RESET
    // -------------------------------------------------------------
    void ResetDailyIfNeeded()
    {
        int today = System.DateTime.Now.DayOfYear;
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
        }
    }

    // -------------------------------------------------------------
    // MAIN REWARD LOGIC
    // -------------------------------------------------------------
    void EarnCoins(int amount)
    {
        CoinManager.Instance.AddCoins(amount);

        GooglePlayManager.Instance.playerData.coins = CoinManager.Instance.coins;
        GooglePlayManager.Instance.SaveGame(GooglePlayManager.Instance.playerData);
    }

    // -------------------------------------------------------------
    // BUTTON LOGIC — REQUIRES MULTIPLE ADS
    // -------------------------------------------------------------
    public void OnReward100()
    {
        if (PlayerPrefs.GetInt(KEY_100) == 1) return;

        int watched = PlayerPrefs.GetInt(ADS100, 0);

        ShowAd(() =>
        {
            watched++;
            PlayerPrefs.SetInt(ADS100, watched);

            if (watched >= 1)
            {
                EarnCoins(100);
                PlayerPrefs.SetInt(KEY_100, 1);
            }

            UpdateAllUI();
        });
    }

    public void OnReward500()
    {
        if (PlayerPrefs.GetInt(KEY_500) == 1) return;

        int watched = PlayerPrefs.GetInt(ADS500, 0);

        ShowAd(() =>
        {
            watched++;
            PlayerPrefs.SetInt(ADS500, watched);

            if (watched >= 2)
            {
                EarnCoins(500);
                PlayerPrefs.SetInt(KEY_500, 1);
            }

            UpdateAllUI();
        });
    }

    public void OnReward1000()
    {
        if (PlayerPrefs.GetInt(KEY_1000) == 1) return;

        int watched = PlayerPrefs.GetInt(ADS1000, 0);

        ShowAd(() =>
        {
            watched++;
            PlayerPrefs.SetInt(ADS1000, watched);

            if (watched >= 3)
            {
                EarnCoins(1000);
                PlayerPrefs.SetInt(KEY_1000, 1);
            }

            UpdateAllUI();
        });
    }

    // -------------------------------------------------------------
    // ADS
    // -------------------------------------------------------------
    void ShowAd(System.Action onComplete)
    {
        AdsManager.Instance.ShowRewarded(onComplete);
    }

    // -------------------------------------------------------------
    // UI UPDATE SYSTEM (shows progress)
    // -------------------------------------------------------------
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
            btn.interactable = false;
            txt.text = "Claimed";
            btn.image.color = Color.gray;
        }
        else
        {
            btn.interactable = true;
            txt.text = rewardText + $" ({watched}/{neededAds})";
            // txt.text = "watch" + $" ({watched}/{neededAds})";
            btn.image.color = Color.white;
        }
    }

    // -------------------------------------------------------------
    // DEBUG RESET (for testing)
    // -------------------------------------------------------------
    [ContextMenu("Reset Rewards Now")]
    public void DebugResetRewards()
    {
        PlayerPrefs.SetInt(KEY_DAY, -1);
        ResetDailyIfNeeded();
        UpdateAllUI();
        Debug.Log("Reward system reset manually.");
    }
}
