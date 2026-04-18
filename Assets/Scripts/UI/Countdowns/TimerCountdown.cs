using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimerCountdown : CountdownUI
{
   [SerializeField] private TextMeshProUGUI _countdownText;
   [SerializeField] private bool _makeSounds = false;
   private int _startingNumber = -1;
   
   public void InitializeCountdown(int value)
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
      int oldTimer = (int)timer;
      while (timer >= 0)
      {
         if (_forceQuit)
         {
            StopCountdownNoTrigger();
            yield break;
         }

         int flooredTimer = Mathf.RoundToInt(timer);
         _countdownText.text = flooredTimer.ToString();
         if (_makeSounds && flooredTimer != oldTimer)
         {
            AudioManager.PlaySound(AudioTrack.Countdown);
         }
         oldTimer = flooredTimer;
         
         timer -= Time.deltaTime * _timescaleSpeed;
         yield return null;
      }

      StopCountdown();
   }
}
