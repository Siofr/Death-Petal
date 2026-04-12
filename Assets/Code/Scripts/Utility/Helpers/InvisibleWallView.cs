using UnityEngine;

public class InvisibleWallView : MonoBehaviour
{
    
    private void OnDrawGizmos()
    {
        foreach (var col in GetComponents<BoxCollider>())
        {
        // Draw wire box around area collider
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f); // Red

        //BoxCollider boxCollider = GetComponent<BoxCollider>();
        BoxCollider boxCollider = col;
        Vector3 boxSize = new Vector3(boxCollider.size.x, boxCollider.size.y, boxCollider.size.z);
        Vector3 boxPosition = new Vector3(boxCollider.bounds.center.x, boxCollider.bounds.center.y, boxCollider.bounds.center.z);

        Gizmos.DrawWireCube(boxPosition, boxSize);

        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);; // More transparent Red

        Gizmos.DrawCube(boxPosition, boxSize);
        }
    }
}

