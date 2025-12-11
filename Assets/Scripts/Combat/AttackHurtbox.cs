using UnityEngine;

public class AttackHurtbox : MonoBehaviour
{
    public Player player;
    public int HP = 200;

    public void TakeDamage(int damage)
    {
        if (player != null)
            player.TakeDamage(damage);
    }
    
}
