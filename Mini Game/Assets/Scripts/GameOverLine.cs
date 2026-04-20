using UnityEngine;

public class GameOverLine : MonoBehaviour
{
    [SerializeField] private float lineWidth = 0.05f;
    [SerializeField] private Color lineColor = new Color(1f, 0.2f, 0.2f, 0.6f);
    [SerializeField] private float lineHalfWidth = 6f;

    private void Start()
    {
        LineRenderer lr = gameObject.AddComponent<LineRenderer>();

        lr.positionCount = 2;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.useWorldSpace = true;
        lr.sortingOrder = 1;

        float y = transform.position.y;
        float z = transform.position.z;

        lr.SetPosition(0, new Vector3(-lineHalfWidth, y, z));
        lr.SetPosition(1, new Vector3(lineHalfWidth, y, z));
    }
}