using UnityEngine;

public class MinigamePortal : Collectable
{
    private const float AMP = 0.1f;
    private const float FREQ = 2f;
    private Vector3 _startingPos;

    private void Awake()
    {
        _startingPos = transform.position;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        PlayingState.CurrentGameplayState = GameplayStates.MiniGame;
    }

    private void Update()
    {
        float xPos = AMP * Mathf.Sin(FREQ * Time.time);
        float yPos = AMP * Mathf.Sin(FREQ * Time.time);
        transform.position = _startingPos + new Vector3(xPos, yPos);
    }
}