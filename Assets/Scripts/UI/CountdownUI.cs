using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CountdownUI : MonoBehaviour
{
   [SerializeField] private float _timescaleSpeed = 1f;
   
   private int _startingNumber = -1;
   private TextMeshProUGUI _countdown;
   private Coroutine _coroutine = null;
   private Action _onFinished;

   public bool isRunning { get; private set; } = false;

   private void Awake()
   {
      _countdown = GetComponent<TextMeshProUGUI>();
   }

   public void InitializeCountdown(int value)
   {
      if (value == -1)
      {
         _startingNumber = -1;
         _countdown.text = "";
         return;
      }
      
      _startingNumber = value;
      _countdown.text = value.ToString();
   }

   public Coroutine StartCountdown(Action onFinished = null, float delay = 0f)
   {
      if (_startingNumber == -1)
      {
         Debug.LogError("Starting Number not assigned");
         return null;
      }
      
      _onFinished = onFinished;
      _coroutine ??= StartCoroutine(Timer());
      return _coroutine;
      
      IEnumerator Timer()
      {
         yield return new WaitForSeconds(delay);
         
         isRunning = true;
         float timer = _startingNumber;
         while (timer >= 0)
         {
            _countdown.text = Mathf.RoundToInt(timer).ToString();
            timer -= Time.deltaTime * _timescaleSpeed;
            yield return null;
         }

         isRunning = false;
         _coroutine = null; 
         onFinished?.Invoke();
      }
   }

   public void StopCountdown()
   {
      if (_coroutine != null) StopCoroutine(_coroutine);
      _coroutine = null;
      isRunning = false;
      
      _onFinished?.Invoke();
      _onFinished = null;
   }

   public void StopCountdownNoTrigger()
   {
      if (_coroutine != null) StopCoroutine(_coroutine);
      _coroutine = null;
      isRunning = false;
      
      _onFinished = null;
   }

   private void OnDestroy()
   {
      StopCountdownNoTrigger();
   }
}
