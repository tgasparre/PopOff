using UnityEngine;

public class MapBoundry : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponentInChildren<AttackHurtbox>().TakeDamage(9999);
        }
    }
}
