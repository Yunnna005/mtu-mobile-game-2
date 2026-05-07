using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public void ShowUnityRewarded()
    {
        if (AdsUnityManager.Instance != null)
        {
            AdsUnityManager.Instance.ShowRewarded();
        }
        else
        {
            Debug.Log("Ads not initialized yet.");
        }
    }

    public void ShowUnityInterstitial()
    {
        if (AdsUnityManager.Instance != null)
        {
            AdsUnityManager.Instance.ShowInterstitial();
        }
        else
        {
            Debug.Log("Ads not initialized yet.");
        }
    }

    public void OnClick()
    {
        AdMobManager.Instance?.ShowInterstitial();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void GetRewarded()
    {
        if (AdMobManager.Instance != null)
        {
            AdMobManager.Instance.ShowRewardedAd();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(100);
            }
        }
        else
        {
            Debug.Log("Ads not initialized yet.");
        }
    }

    public void OnLeaderboardButtonPressed()
    {
        if (GooglePlayServicesManager.Instance != null)
        {
            GooglePlayServicesManager.Instance.ShowLeaderboard();
        }

    }

    public void OnAchievementsButtonPressed()
    {
        if (GooglePlayServicesManager.Instance != null)
        {
            GooglePlayServicesManager.Instance.ShowAchievements();
        }
    }
}