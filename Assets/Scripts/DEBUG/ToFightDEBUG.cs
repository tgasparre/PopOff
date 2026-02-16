using System;
using System.Collections;
using UnityEngine;

public class ToFightDEBUG : MonoBehaviour
{
   private void Start()
   {
      StartCoroutine(EnabledTimer());
      return;
      
      IEnumerator EnabledTimer()
      {
         yield return new WaitForSeconds(1f);
         GetComponent<CircleCollider2D>().enabled = true;
      }
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (!other.gameObject.CompareTag("Player")) return;
      PlayingState.CurrentGameplayState = GameplayStates.Combat;
   }
}
