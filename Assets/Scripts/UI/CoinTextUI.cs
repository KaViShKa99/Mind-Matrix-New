// using UnityEngine;
// using TMPro;

// public class CoinTextUI : MonoBehaviour
// {
//     public TMP_Text coinText;

//     private void Start()
//     {
//         // When coins change, call UpdateCoinUI
//         CoinManager.Instance.OnCoinsChanged += UpdateCoinUI;

//         // Set initial UI
//         UpdateCoinUI(CoinManager.Instance.GetCoins());
//     }

//     private void OnDestroy()
//     {
//         // Remove listener when this object is destroyed
//         CoinManager.Instance.OnCoinsChanged -= UpdateCoinUI;
//     }

//     private void UpdateCoinUI(int newCoinAmount)
//     {
//         coinText.text = newCoinAmount.ToString();
//     }
// }
using UnityEngine;
using TMPro;

public class CoinTextUI : MonoBehaviour
{
    public TMP_Text coinText;

    private void Start()
    {
        CoinManager.Instance.OnCoinsChanged += UpdateCoinUI;
        UpdateCoinUI(CoinManager.Instance.GetCoins());
    }

    private void OnDestroy()
    {
        CoinManager.Instance.OnCoinsChanged -= UpdateCoinUI;
    }

    private void UpdateCoinUI(int newCoinAmount)
    {
        coinText.text = FormatNumber(newCoinAmount);
    }

    private string FormatNumber(long num)
    {
        if (num >= 1_000_000_000)
            return (num / 1_000_000_000f).ToString("0.#") + "B";

        if (num >= 1_000_000)
            return (num / 1_000_000f).ToString("0.#") + "M";

        if (num >= 1_000)
            return (num / 1_000f).ToString("0.#") + "K";

        return num.ToString();
    }
}

