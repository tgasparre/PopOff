using UnityEngine;

public class HitboxVisualizer : MonoBehaviour
{ 
    //script by claude to make hitbox debugging easier
    public Color gizmoColor = Color.red;
    
    void OnDrawGizmos()
    {
        // For BoxCollider2D
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null) 
        {
            Gizmos.color = gizmoColor; 
            Gizmos.DrawWireCube(transform.position + (Vector3)box.offset, box.size);
        }
    }
}
