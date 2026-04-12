using UnityEngine;

public class InvisibleWallView : MonoBehaviour
{
    public Color outline;
    public Color fill;
    private void OnDrawGizmos()
    {
        foreach (var col in GetComponents<BoxCollider>())
        {
        // Draw wire box around area collider
        Gizmos.color = outline; // Red

        //BoxCollider boxCollider = GetComponent<BoxCollider>();
        BoxCollider boxCollider = col;
        Vector3 boxSize = new Vector3(boxCollider.size.x, boxCollider.size.y, boxCollider.size.z);
        Vector3 boxPosition = new Vector3(boxCollider.bounds.center.x, boxCollider.bounds.center.y, boxCollider.bounds.center.z);

        Gizmos.DrawWireCube(boxPosition, boxSize);

        Gizmos.color = fill;; // More transparent Red

        Gizmos.DrawCube(boxPosition, boxSize);
        }
    }
}

