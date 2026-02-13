using UnityEngine;

public class AttackHurtbox : MonoBehaviour
{
    public Player player;
    public float HP = 200;

    public void TakeDamage(float damage)
    {
        if (player != null)
            player.TakeDamage(damage);
    }
    
}
