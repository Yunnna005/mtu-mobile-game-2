using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject ball_1;
    [SerializeField] private GameObject ball_1_5;
    [SerializeField] private GameObject ball_2;
    [SerializeField] private GameObject ball_2_5;
    [SerializeField] private GameObject ball_3;
    [SerializeField] private GameObject ball_3_5;
    [SerializeField] private GameObject ball_4;

    [SerializeField] private float spawnY = 14f;
    [SerializeField] private float spawnZ = 0f;
    [SerializeField] private float gameOverY = 13f;
    [SerializeField] private float dropDelay = 0.8f;
    [SerializeField] private float horizontalMin = -8.5f;
    [SerializeField] private float horizontalMax = 8.5f;
    [SerializeField] private int scorePerMerge = 10;
    [SerializeField] private float maxReleaseVelocityX = 3f;
    [SerializeField] private float floorY = -8f;
    [SerializeField] private float maxUpwardVelocity = 3f;
    [SerializeField] private float mergeCooldown = 0.2f;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text scoreText2;

    private List<GameObject> ballPrefabs;
    private GameObject pendingBall;
    private Rigidbody pendingRb;

    private bool isHolding = false;
    private bool canSpawn = true;
    public bool isGameOver = false;
    private int score = 0;
    private bool inputCooldown = false;

    private float lastDragVelocityX = 0f;

    private const int SpawnableTiers = 3;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ballPrefabs = new List<GameObject>
        {
            ball_1, ball_1_5, ball_2,
            ball_2_5, ball_3, ball_3_5, ball_4
        };

        gameOverPanel.SetActive(false);
        gamePanel.SetActive(true);

        AdMobManager.OnAdMobReady += OnAdMobReady;
    }

    private void Update()
    {
        if (isGameOver) return;

        Ball[] allBalls = FindObjectsByType<Ball>(FindObjectsSortMode.None);
        foreach (Ball b in allBalls)
        {
            if (!b.hasBeenDropped) continue;

            Rigidbody rb = b.GetComponent<Rigidbody>();
            if (rb == null) continue;

            if (rb.linearVelocity.y > maxUpwardVelocity)
            {
                rb.linearVelocity = new Vector3(
                    rb.linearVelocity.x,
                    maxUpwardVelocity,
                    0f
                );
            }
        }
    }

    public void OnTouchBegan()
    {
        if (isGameOver || inputCooldown || pendingBall == null) return;
        isHolding = true;
    }

    public void OnTouchDragged(float worldDeltaX, float velocityX)
    {
        if (isGameOver || inputCooldown || !isHolding || pendingBall == null) return;

        Vector3 pos = pendingBall.transform.position;
        float newX = Mathf.Clamp(pos.x + worldDeltaX, horizontalMin, horizontalMax);
        pendingBall.transform.position = new Vector3(newX, spawnY, spawnZ);

        lastDragVelocityX = Mathf.Lerp(lastDragVelocityX, velocityX, 0.3f);

        DropLine dl = pendingBall.GetComponent<DropLine>();
        if (dl != null) dl.UpdateLine(floorY);
    }

    public void OnTouchReleased()
    {
        if (isGameOver || inputCooldown || !isHolding || pendingBall == null) return;
        isHolding = false;
        DropBall();
    }

    private void SpawnPendingBall()
    {
        if (!canSpawn) return;

        int tier = Random.Range(0, SpawnableTiers);
        pendingBall = Instantiate(ballPrefabs[tier], new Vector3(0f, spawnY, spawnZ), Quaternion.identity);

        Ball b = pendingBall.GetComponent<Ball>();
        b.tier = tier;

        pendingRb = pendingBall.GetComponent<Rigidbody>();
        if (pendingRb != null)
        {
            pendingRb.isKinematic = true;
        }

        lastDragVelocityX = 0f;

        DropLine dl = pendingBall.GetComponent<DropLine>();
        if (dl != null)
        {
            dl.ShowLine(floorY);
        }
    }

    private void DropBall()
    {
        if (pendingBall == null) return;
        DropLine dl = pendingBall.GetComponent<DropLine>();
        if (dl != null)
        {
            dl.HideLine();
        }

        Ball b = pendingBall.GetComponent<Ball>();
        if (b != null)
        {
            b.hasBeenDropped = true;
        }

        if (pendingRb != null)
        {
            pendingRb.isKinematic = false;
            pendingRb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            pendingRb.linearVelocity = Vector3.zero;
        }

        pendingBall = null;
        pendingRb = null;
        canSpawn = false;
        lastDragVelocityX = 0f;

        StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator SpawnAfterDelay()
    {
        yield return new WaitForSeconds(dropDelay);
        canSpawn = true;
        SpawnPendingBall();
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        canSpawn = false;
        isHolding = false;

        StopAllCoroutines();

        if (pendingBall != null)
        {
            Destroy(pendingBall);
            pendingBall = null;
            pendingRb = null;
        }

        if (GooglePlayServicesManager.Instance != null)
        {
            GooglePlayServicesManager.Instance.PostScore(score);
        }

        if (gamePanel != null)
        {
            gamePanel.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void ResetGame()
    {
        StopAllCoroutines();

        Ball[] allBalls = FindObjectsByType<Ball>(FindObjectsSortMode.None);
        foreach (Ball b in allBalls)
        {
            if (b != null && b.gameObject != null) 
            {
                Destroy(b.gameObject);
            }
        }

        if (pendingBall != null)
        {
            Destroy(pendingBall);
            pendingBall = null;
            pendingRb = null;
        }

        GameOverTrigger trigger = FindFirstObjectByType<GameOverTrigger>();
        if (trigger != null)
        {
            trigger.ClearBalls();
        }

        isHolding = false;
        canSpawn = true;
        isGameOver = false;
        score = 0;
        lastDragVelocityX = 0f;

        if (scoreText != null)
        {
            scoreText.text = "Score: 0";
        }
        if (scoreText2 != null)
        {
            scoreText2.text = "Score: 0";
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (gamePanel != null)
        {
            gamePanel.SetActive(true);
        }

        SpawnPendingBall();
    }

    public void MergeBalls(int tier, Vector3 position)
    {
        if (GooglePlayServicesManager.Instance != null)
        {
            GooglePlayServicesManager.Instance.UnlockAchievement(GooglePlayServicesManager.ACHIEVEMENT_FIRST_SHAPE);
        }


        int nextTier = tier + 1;
        if (nextTier < ballPrefabs.Count)
        {
            GameObject merged = Instantiate(ballPrefabs[nextTier], position, Quaternion.identity);

            Ball b = merged.GetComponent<Ball>();
            b.tier = nextTier;
            b.hasBeenDropped = true;

            Rigidbody rb = merged.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            }
        }

        AddScore(scorePerMerge * (tier + 1));
    }

    public int GetMaxTier()
    {
        return ballPrefabs.Count - 1;
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        if (scoreText2 != null)
        {
            scoreText2.text = "Score: " + score;
        }
    }

    public void StartInputCooldown(float duration = 0.5f)
    {
        StartCoroutine(InputCooldownRoutine(duration));
    }

    private IEnumerator InputCooldownRoutine(float duration)
    {
        inputCooldown = true;
        yield return new WaitForSeconds(duration);
        inputCooldown = false;
    }
    private void OnAdMobReady()
    {
        AdMobManager.OnAdMobReady -= OnAdMobReady;
        Debug.Log("AdMob ready — spawning ball");
        isGameOver = false; 
        isHolding = false;
        canSpawn = true;
        SpawnPendingBall();
    }

    private void OnDestroy()
    {
        AdMobManager.OnAdMobReady -= OnAdMobReady;
    }
}