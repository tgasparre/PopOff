using System;
using UnityEngine;

public class PowerupCollectable : MonoBehaviour
{
    public Powerup powerup;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player p = other.gameObject.transform.parent.GetComponent<Player>();
            p.powerups.ApplyPower(powerup);
        }
    }
}
