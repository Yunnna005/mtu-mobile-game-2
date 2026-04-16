using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject ball_1;
    [SerializeField] private GameObject ball_1_5;
    [SerializeField] private GameObject ball_2;
    [SerializeField] private GameObject ball_2_5;
    [SerializeField] private GameObject ball_3;
    [SerializeField] private GameObject ball_3_5;
    [SerializeField] private GameObject ball_4;

    List<GameObject> balls = new List<GameObject>(7);
    Vector2 spawnPosition = new Vector2(0, 15);

    private void Start()
    {
        balls.Add(ball_1);
        balls.Add(ball_1_5);
        balls.Add(ball_2);
        balls.Add(ball_2_5);
        balls.Add(ball_3);
        balls.Add(ball_3_5);
        balls.Add(ball_4);

        SpawnBall();


    }

    private void SpawnBall()
    {
        
    }
}
