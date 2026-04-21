using System;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.InputSystem;

public class AdsUnityManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Header("Game IDs")]
    [SerializeField] private string _androidGameId = "6096032";
    [SerializeField] private bool _testMode = true;

    [SerializeField] private string _androidBannerId = "Banner_Android";
    [SerializeField] private string _androidInterstitialId = "Interstitial_Android";
    [SerializeField] private string _androidRewardedId = "Rewarded_Android";

    private string _bannerAdUnitId;
    private string _interstitialAdUnitId;
    private string _rewardedAdUnitId;

    private bool _interstitialLoaded = false;
    private bool _rewardedLoaded = false;

    public static event Action OnRewardEarned;


    public static AdsUnityManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); return;
        }

        _bannerAdUnitId = _androidBannerId;
        _interstitialAdUnitId = _androidInterstitialId;
        _rewardedAdUnitId = _androidRewardedId;

        if (!Advertisement.isInitialized && Advertisement.isSupported)
            Advertisement.Initialize(_androidGameId, _testMode, this);
    }

    void Update()
    {
        // for testing
        if (_testMode)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                ShowInterstitial();
            }
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Ads Initialized");
        ShowBanner();
        LoadInterstitial();
        LoadRewarded();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Initialization Failed: {error} - {message}");
    }

    #region Banner Ad
    public void ShowBanner()
    {
        Debug.Log("Showing Banner Ad");

        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_LEFT);

        BannerLoadOptions loadOptions = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(_bannerAdUnitId, loadOptions);
    }

    void OnBannerLoaded()
    {
        Debug.Log("Banner Loaded");

        BannerOptions showOptions = new BannerOptions
        {
            showCallback = OnBannerShown,
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden
        };

        Advertisement.Banner.Show(_bannerAdUnitId, showOptions);
    }

    void OnBannerError(string message)
    {
        Debug.Log("Banner Error: " + message);
    }

    void OnBannerShown()
    {
        Debug.Log("Banner Shown");
    }

    void OnBannerClicked()
    {
        Debug.Log("Banner Clicked");
    }

    void OnBannerHidden()
    {
        Debug.Log("Banner Hidden");
    }
    public void HideBanner()
    {
        Advertisement.Banner.Hide();
        Debug.Log("Banner Hidden");
    }
    #endregion

    #region Interstitial Ad
    public void LoadInterstitial()
    {
        Debug.Log("Loading Interstitial: " + _interstitialAdUnitId);
        Advertisement.Load(_interstitialAdUnitId, this);
    }

    public void ShowInterstitial()
    {
        if (_interstitialLoaded)
        {
            Debug.Log("Showing Interstitial: " + _interstitialAdUnitId);
            Advertisement.Show(_interstitialAdUnitId, this);
        }
        else
        {
            Debug.Log("Interstitial not ready");
            LoadInterstitial();
        }
    }

    #endregion

    #region Reward

    public void LoadRewarded()
    {
        Debug.Log("Loading Rewarded: " + _rewardedAdUnitId);
        Advertisement.Load(_rewardedAdUnitId, this);
    }

    public void ShowRewarded()
    {
        if (_rewardedLoaded)
        {
            Debug.Log("Showing Rewarded: " + _rewardedAdUnitId);
            Advertisement.Show(_rewardedAdUnitId, this);
        }
        else
        {
            Debug.Log("Rewarded not ready");
            LoadRewarded();
        }
    }

    #endregion

    #region IUnityAdsLoadListener (load)
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId == _interstitialAdUnitId)
        {
            _interstitialLoaded = true;
        }
        if (adUnitId == _rewardedAdUnitId)
        {
            _rewardedLoaded = true;
        }
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Failed to load {adUnitId}: {error} - {message}");

        if (adUnitId == _interstitialAdUnitId)
        {
            _interstitialLoaded = false;
        }
        if (adUnitId == _rewardedAdUnitId)
        {
            _rewardedLoaded = false;
        }
    }
    #endregion

    #region IUnityAdsShowListener (show)

    public void OnUnityAdsShowStart(string adUnitId)
    {
        Debug.Log("Ad Started: " + adUnitId);

        if (adUnitId == _interstitialAdUnitId)
        {
            _interstitialLoaded = false;
        }
        if (adUnitId == _rewardedAdUnitId)
        {
            _rewardedLoaded = false;
        }
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
        Debug.Log("Ad Clicked: " + adUnitId);
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState state)
    {
        Debug.Log($"Ad Complete: {adUnitId} - {state}");

        if (adUnitId == _interstitialAdUnitId)
        {
            LoadInterstitial();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartInputCooldown();
            }
               
        }
        if (adUnitId == _rewardedAdUnitId)
        {
            LoadRewarded();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartInputCooldown();
            }
                
        }
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Ad Show Failed {adUnitId}: {error} - {message}");

        if (adUnitId == _interstitialAdUnitId)
        {
            LoadInterstitial();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartInputCooldown();
            }

        }
        if (adUnitId == _rewardedAdUnitId)
        {
            LoadRewarded();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartInputCooldown();
            }
        }
    }

    #endregion
}