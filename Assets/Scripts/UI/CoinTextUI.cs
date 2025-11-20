using UnityEngine;
using TMPro;

public class CoinTextUI : MonoBehaviour
{
    public TMP_Text coinText;

    private void Start()
    {
        // When coins change, call UpdateCoinUI
        CoinManager.Instance.OnCoinsChanged += UpdateCoinUI;

        // Set initial UI
        UpdateCoinUI(CoinManager.Instance.GetCoins());
    }

    private void OnDestroy()
    {
        // Remove listener when this object is destroyed
        CoinManager.Instance.OnCoinsChanged -= UpdateCoinUI;
    }

    private void UpdateCoinUI(int newCoinAmount)
    {
        coinText.text = newCoinAmount.ToString();
    }
}
