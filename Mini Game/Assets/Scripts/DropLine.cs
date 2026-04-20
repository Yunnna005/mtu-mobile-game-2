using UnityEngine;

public class DropLine : MonoBehaviour
{
    [SerializeField] private float lineWidth = 0.05f;
    [SerializeField] private Color lineColor = new Color(1f, 1f, 1f, 0.4f);

    private LineRenderer lr;

    private void Awake()
    {
        lr = gameObject.AddComponent<LineRenderer>();

        lr.positionCount = 2;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lineColor;
        lr.endColor = lineColor;

        lr.useWorldSpace = true;
        lr.enabled = false;
    }

    public void ShowLine(float floorY)
    {
        if (lr == null) return;
        lr.enabled = true;
        UpdateLine(floorY);
    }

    public void HideLine()
    {
        if (lr == null) return;
        lr.enabled = false;
    }

    public void UpdateLine(float floorY)
    {
        if (lr == null || !lr.enabled) return;

        Vector3 top = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 bottom = new Vector3(transform.position.x, floorY, transform.position.z);

        lr.SetPosition(0, top);
        lr.SetPosition(1, bottom);
    }
}