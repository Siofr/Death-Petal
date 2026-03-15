using UnityEngine;

public class Stage : MonoBehaviour
{
    public string stageName;
    public float bestTime;
    public int bestScore;
    public Bounds stageBoundary;

    private void Awake()
    {
        stageBoundary.center = transform.position;
    }

    private void OnDrawGizmos()
    {
        // Draw wire box around area collider
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f); // Green
        Gizmos.DrawWireCube(transform.position, stageBoundary.size);
    }
}
