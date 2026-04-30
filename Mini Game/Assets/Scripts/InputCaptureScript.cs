using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class InputCaptureScript : MonoBehaviour
{
    private float timer;
    private bool hasMoved;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        if (Touch.activeTouches.Count > 0)
        {
            Touch t = Touch.activeTouches[0];

            if (t.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                timer = 0f;
                hasMoved = false;
                GameManager.Instance.OnTouchBegan();
            }
            else if (t.phase == UnityEngine.InputSystem.TouchPhase.Stationary)
            {
                timer += Time.deltaTime;
            }
            else if (t.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                hasMoved = true;
                timer += Time.deltaTime;

                float worldDeltaX = t.delta.x / Screen.width * Camera.main.orthographicSize * Camera.main.aspect * 2f;

                float velocityX = Time.deltaTime > 0 ? worldDeltaX / Time.deltaTime : 0f;

                GameManager.Instance.OnTouchDragged(worldDeltaX, velocityX);
            }
            else if (t.phase == UnityEngine.InputSystem.TouchPhase.Ended)
            {
                GameManager.Instance.OnTouchReleased();
            }
        }
    }
}