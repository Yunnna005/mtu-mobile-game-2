using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Ball Prefabs (index 0 = smallest)")]
    [SerializeField] private GameObject ball_1;
    [SerializeField] private GameObject ball_1_5;
    [SerializeField] private GameObject ball_2;
    [SerializeField] private GameObject ball_2_5;
    [SerializeField] private GameObject ball_3;
    [SerializeField] private GameObject ball_3_5;
    [SerializeField] private GameObject ball_4;

    [Header("Settings")]
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

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text scoreText;

    private List<GameObject> ballPrefabs;
    private GameObject pendingBall;
    private Rigidbody pendingRb;

    private bool isHolding = false;
    private bool canSpawn = true;
    private bool isGameOver = false;
    private int score = 0;

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
        SpawnPendingBall();
    }

    private void Update()
    {
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
        if (isGameOver || pendingBall == null) return;
        isHolding = true;
        lastDragVelocityX = 0f;
    }

    public void OnTouchDragged(float worldDeltaX, float velocityX)
    {
        if (isGameOver || !isHolding || pendingBall == null) return;

        Vector3 pos = pendingBall.transform.position;
        float newX = Mathf.Clamp(pos.x + worldDeltaX, horizontalMin, horizontalMax);
        pendingBall.transform.position = new Vector3(newX, spawnY, spawnZ);

        lastDragVelocityX = Mathf.Lerp(lastDragVelocityX, velocityX, 0.3f);

        DropLine dl = pendingBall.GetComponent<DropLine>();
        if (dl != null) dl.UpdateLine(floorY);
    }


    public void OnTouchReleased()
    {
        if (isGameOver || !isHolding || pendingBall == null) return;
        isHolding = false;
        DropBall();
    }

    private void SpawnPendingBall()
    {
        if (!canSpawn) return;

        int tier = Random.Range(0, SpawnableTiers);
        pendingBall = Instantiate(
            ballPrefabs[tier],
            new Vector3(0f, spawnY, spawnZ),
            Quaternion.identity
        );

        Ball b = pendingBall.GetComponent<Ball>();
        b.tier = tier;

        pendingRb = pendingBall.GetComponent<Rigidbody>();
        if (pendingRb != null)
            pendingRb.isKinematic = true;

        lastDragVelocityX = 0f;

        DropLine dl = pendingBall.GetComponent<DropLine>();
        if (dl != null) dl.ShowLine(floorY);
    }

    private void DropBall()
    {
        if (pendingBall == null) return;
        DropLine dl = pendingBall.GetComponent<DropLine>();
        if (dl != null) dl.HideLine();

        Ball b = pendingBall.GetComponent<Ball>();
        if (b != null) b.hasBeenDropped = true;

        if (pendingRb != null)
        {
            pendingRb.isKinematic = false;
            pendingRb.constraints =
                RigidbodyConstraints.FreezePositionZ |
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationY;

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
        if (pendingBall != null) Destroy(pendingBall);
        gameOverPanel.SetActive(true);
    }

    public void MergeBalls(int tier, Vector3 position)
    {
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
                rb.constraints =
                    RigidbodyConstraints.FreezePositionZ |
                    RigidbodyConstraints.FreezeRotationX |
                    RigidbodyConstraints.FreezeRotationY;
            }
        }

        AddScore(scorePerMerge * (tier + 1));
    }

    public int GetMaxTier()
    {
        return ballPrefabs.Count - 1;
    }

    private void AddScore(int amount)
    {
        score += amount;
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}