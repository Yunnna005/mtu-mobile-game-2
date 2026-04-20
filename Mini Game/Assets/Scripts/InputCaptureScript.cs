using UnityEngine;

public class InputCaptureScript : MonoBehaviour
{
    private float timer;
    private bool hasMoved;
    private float tapThreshold = 0.5f;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.touches[0];

            switch (t.phase)
            {
                case TouchPhase.Began:
                    timer = 0f;
                    hasMoved = false;
                    GameManager.Instance.OnTouchBegan();
                    break;

                case TouchPhase.Stationary:
                    timer += Time.deltaTime;
                    break;

                case TouchPhase.Moved:
                    hasMoved = true;
                    timer += Time.deltaTime;

                    float worldDeltaX = t.deltaPosition.x / Screen.width * Camera.main.orthographicSize * Camera.main.aspect * 2f;
                    float velocityX = Time.deltaTime > 0 ? worldDeltaX / Time.deltaTime: 0f;

                    GameManager.Instance.OnTouchDragged(worldDeltaX, velocityX);
                    break;

                case TouchPhase.Ended:
                    GameManager.Instance.OnTouchReleased();
                    break;
            }
        }
    }
}