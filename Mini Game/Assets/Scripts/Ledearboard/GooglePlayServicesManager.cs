using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GooglePlayServicesManager : MonoBehaviour
{
    public const string LEADERBOARD_HIGH_SCORE = "CgkIotK8i6AcEAIQAQ";
    public const string ACHIEVEMENT_FIRST_SHAPE = "CgkIotK8i6AcEAIQAg";

    public static GooglePlayServicesManager Instance { get; private set; }
    public bool IsAuthenticated => PlayGamesPlatform.Instance.IsAuthenticated();

    void Awake()
    {
        if (Instance == null) 
        { 
            Instance = this; DontDestroyOnLoad(gameObject); 
        }
        else 
        { 
            Destroy(gameObject); 
        }
    }

    void Start()
    {
        PlayGamesPlatform.Activate();
        SignIn();
    }

    public void SignIn()
    {
        PlayGamesPlatform.Instance.Authenticate(status =>
        {
            if (status == SignInStatus.Success)
            {
                Debug.Log("[GPGS] Signed in: " + Social.localUser.userName);
            }
            else
            {
                Debug.LogWarning("[GPGS] Sign-in failed: " + status);
            }
        });
    }

    public void PostScore(long score)
    {
        if (!IsAuthenticated) 
        { 
            Debug.LogWarning("[GPGS] Not signed in."); 
            return; 
        }
        Social.ReportScore(score, LEADERBOARD_HIGH_SCORE, success =>
            Debug.Log("[GPGS] Score posted: " + success));
    }

    public void ShowLeaderboard()
    {
        if (!IsAuthenticated) 
        { 
            SignIn(); 
            return; 
        }
        PlayGamesPlatform.Instance.ShowLeaderboardUI(LEADERBOARD_HIGH_SCORE);
    }

    public void UnlockAchievement(string id)
    {
        if (!IsAuthenticated) return;
        Social.ReportProgress(id, 100.0, success =>
            Debug.Log("[GPGS] Achievement unlocked: " + success));
    }

    public void ShowAchievements()
    {
        if (!IsAuthenticated) { SignIn(); return; }
        Social.ShowAchievementsUI();
    }
}