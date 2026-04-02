using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimerCountdown : CountdownUI
{
   [SerializeField] private TextMeshProUGUI _countdownText;
   private int _startingNumber = -1;
   
   public override void InitializeCountdown(int value)
   {
      if (value == -1)
      {
         _startingNumber = -1;
         _countdownText.text = "";
         return;
      }
      
      _startingNumber = value;
      _countdownText.text = value.ToString();
   }

   protected override IEnumerator Countdown(float delay)
   {
      if (_startingNumber == -1) throw new Exception("Starting number not initialized");
      yield return new WaitForSeconds(delay);

      isRunning = true;
      float timer = _startingNumber;
      while (timer >= 0)
      {
         if (_forceQuit)
         {
            StopCountdownNoTrigger();
            yield break;
         }

         _countdownText.text = Mathf.RoundToInt(timer).ToString();
         timer -= Time.deltaTime * _timescaleSpeed;
         yield return null;
      }

      StopCountdown();
   }
}
