using UnityEngine;

public class MinigamePortal : Collectable
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        PlayingState.CurrentGameplayState = GameplayStates.MiniGame;
    }
}