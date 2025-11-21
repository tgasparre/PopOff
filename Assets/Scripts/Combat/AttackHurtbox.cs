using UnityEngine;

public class AttackHurtbox : MonoBehaviour
{
    public Player player;

    public void TakeDamage(int damage)
    {
        if (player != null)
            player.TakeDamage(damage);
    }
    
}
