using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class AdMobManager : MonoBehaviour
{
    private BannerView bannerView;
    private RewardedAd rewardedAd;
    private InterstitialAd interstitialAd;

    private string bannerId = "ca-app-pub-8618196431920161/1688585489";
    private string interstitialId = "ca-app-pub-8618196431920161/5804639443";
    private string rewardedId = "ca-app-pub-8618196431920161/9339950965";

    public static AdMobManager Instance;
    public static event Action OnRewardEarned;
    private bool isInitialized = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static event Action OnAdMobReady;
    void Start()
    {
        MobileAds.Initialize((InitializationStatus initstatus) =>
        {
            if (initstatus == null)
            {
                Debug.LogError("Google Mobile Ads initialization failed.");
                return;
            }

            Debug.Log("Google Mobile Ads initialization complete.");
            isInitialized = true;
            LoadInterstitial();
            LoadBanner();
            LoadRewardedAd();
            OnAdMobReady?.Invoke();
        });
    }

    #region Banner
    public void LoadBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
        bannerView.LoadAd(new AdRequest());
        Debug.Log("Banner loaded.");
    }
    public void HideBanner()
    {
        bannerView?.Hide();
        Debug.Log("Banner hidden.");
    }
    public void ShowBanner()
    {
        bannerView?.Show();
        Debug.Log("Banner shown.");
    }

    #endregion

    #region Interstitial

    public void LoadInterstitial()
    {
        interstitialAd?.Destroy();

        InterstitialAd.Load(interstitialId, new AdRequest(), (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogWarning("AdMob Interstitial failed to load: " + error.GetMessage());
                return;
            }
            Debug.Log("AdMob Interstitial loaded.");
            interstitialAd = ad;

            ad.OnAdFullScreenContentClosed += () => 
            {
                Debug.Log("Interstitial closed."); 
                LoadInterstitial();
                if (isInitialized)
                {
                    GameManager.Instance?.StartInputCooldown();
                }

            };
            ad.OnAdFullScreenContentFailed += e => 
            { 
                Debug.LogWarning("Interstitial show failed: " + e.GetMessage()); 
                LoadInterstitial(); 
            
            };
        });
    }

    public void ShowInterstitial()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("Showing AdMob Interstitial.");
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("AdMob Interstitial not ready");
            LoadInterstitial();
        }
    }

    #endregion

    #region Reward
    public void LoadRewardedAd()
    {
        rewardedAd?.Destroy();

        RewardedAd.Load(rewardedId, new AdRequest(), (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogWarning("AdMob Rewarded failed to load: " + error.GetMessage());
                return;
            }
            Debug.Log("AdMob Rewarded loaded.");
            rewardedAd = ad;

            ad.OnAdFullScreenContentClosed += () => 
            { 
                Debug.Log("Rewarded closed."); 
                LoadRewardedAd();
                GameManager.Instance?.StartInputCooldown();
            };
            ad.OnAdFullScreenContentFailed += e => 
            { 
                Debug.LogWarning("Rewarded show failed: " + e.GetMessage()); 
                LoadRewardedAd(); 
            };
        });
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            Debug.Log("Showing AdMob Rewarded.");
            rewardedAd.Show(reward =>
            {
                Debug.Log($"AdMob Reward earned: {reward.Amount} {reward.Type}");
                OnRewardEarned?.Invoke();
            });
        }
        else
        {
            Debug.Log("AdMob Rewarded not ready, loading...");
            LoadRewardedAd();
        }
    }

    #endregion
}

