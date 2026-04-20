using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    [SerializeField] private float timeToGameOver = 3f;

    private Dictionary<Ball, float> ballsInTrigger = new Dictionary<Ball, float>();

    private void Update()
    {
        if (ballsInTrigger.Count == 0) return;

        List<Ball> keys = new List<Ball>(ballsInTrigger.Keys);

        foreach (Ball b in keys)
        {
            if (b == null)
            {
                ballsInTrigger.Remove(b);
                continue;
            }

            ballsInTrigger[b] += Time.deltaTime;

            if (ballsInTrigger[b] >= timeToGameOver)
            {
                GameManager.Instance.TriggerGameOver();
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Ball b = other.GetComponent<Ball>();

        if (b == null || !b.hasBeenDropped) return;

        if (!ballsInTrigger.ContainsKey(b))
            ballsInTrigger.Add(b, 0f);
    }

    private void OnTriggerExit(Collider other)
    {
        Ball b = other.GetComponent<Ball>();
        if (b == null) return;

        if (ballsInTrigger.ContainsKey(b))
            ballsInTrigger.Remove(b);
    }
}