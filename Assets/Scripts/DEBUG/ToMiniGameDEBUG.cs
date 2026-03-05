using System.Collections;
using UnityEngine;

public class ToMiniGameDEBUG : MonoBehaviour
{
    private void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        StartCoroutine(EnabledTimer());
        return;
      
        IEnumerator EnabledTimer()
        {
            yield return new WaitForSeconds(9f);
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<CircleCollider2D>().enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        PlayingState.CurrentGameplayState = GameplayStates.MiniGame;
    }
}
