using UnityEngine;
using UnityEngine.InputSystem;

public class CoinMinigameEnder : MonoBehaviour
{

    public Powerup powerup;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player otherPlayer = other.gameObject.transform.parent.GetComponent<Player>();
            Destroy(gameObject);
            PlayingState.CurrentGameplayState = GameplayStates.Combat;
            otherPlayer.powerups.ApplyPower(powerup);
        }
        
    }
}
