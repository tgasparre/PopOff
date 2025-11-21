using UnityEngine;

public class BasicAttackCommand : Command
{
    public override void Execute(GameObject target)
    {
        // set hitbox to active
        // deactivate after wait time
        if (target)
        {
            Attack(target);
        }
    }

    private void Attack(GameObject target)
    {
        Debug.Log("Attack command called");
        Player player = target.GetComponent<Player>();

        if (player != null)
        {
            player.TakeDamage(10);
            Debug.Log("Player " + player.name + "took damage");
        }

    }
    
}
