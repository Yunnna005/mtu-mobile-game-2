using UnityEngine;

public class Ball : MonoBehaviour
{
    public int tier;
    public bool hasBeenDropped = false;
    private bool hasMerged = false;

    [SerializeField] private float mergeCooldown = 0.5f;
    private float spawnTime;

    private void Start()
    {
        spawnTime = Time.time;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Ball other = collision.gameObject.GetComponent<Ball>();
        if (other == null) return;

        if (hasMerged) return;
        if (Time.time - spawnTime < mergeCooldown) return;
        if (other.hasMerged) return;
        if (Time.time - other.spawnTime < other.mergeCooldown) return;
        if (other.tier != tier) return;
        if (tier >= GameManager.Instance.GetMaxTier()) return;
        if (gameObject.GetInstanceID() < other.gameObject.GetInstanceID()) return;

        hasMerged = true;
        other.hasMerged = true;

        Vector3 mergePos = (transform.position + other.transform.position) / 2f;
        GameManager.Instance.MergeBalls(tier, mergePos);

        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}