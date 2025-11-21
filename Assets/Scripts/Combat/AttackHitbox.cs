using UnityEngine;

public class AttackHitbox : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
        {
            player.TakeDamage(10);
            Debug.Log("Player " + player.name + "took damage");
        }
    }
    
    // start code by claude to visualize hitbox
    private void OnDrawGizmos()
    {
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();
        if (col != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + (Vector3)col.offset, col.bounds.size);
        }
    }
    // end code by claude
    
}
