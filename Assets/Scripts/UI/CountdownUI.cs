using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CountdownUI : MonoBehaviour
{
   [SerializeField] private CountdownType type = CountdownType.Number;
   [SerializeField] private float _timescaleSpeed = 1f;
   
   private int _startingNumber = -1;
   private TextMeshProUGUI _countdown;
   private Coroutine _coroutine = null;
   private Action _onFinished;
   
   [Header("Ready Go Settings")]
   [SerializeField] private float _readyWaitTime = 1.8f;
   [SerializeField] private float _goWaitTime = 0.8f;

   private CanvasGroup _canvasGroup;

   private bool _forceQuit = false;
   public bool isRunning { get; private set; } = false;

   private void Awake()
   {
      _countdown = GetComponent<TextMeshProUGUI>();
      _canvasGroup = GetComponent<CanvasGroup>();
      _canvasGroup.alpha = 0;
   }

   public void InitializeCountdown(int value)
   {
      if (value == -1)
      {
         _startingNumber = -1;
         _countdown.text = "";
         return;
      }

      if (type == CountdownType.ReadyGo)
      {
         _startingNumber = 1;
         _countdown.text = "Ready";
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

      _canvasGroup.alpha = 1f;
      _onFinished = onFinished;
      _coroutine ??= StartCoroutine(type == CountdownType.Number ? Timer() : ReadyGo());
      return _coroutine;

      IEnumerator Timer()
      {
         yield return new WaitForSeconds(delay);

         isRunning = true;
         float timer = _startingNumber;
         while (timer >= 0)
         {
            if (_forceQuit)
            {
               _forceQuit = false;
               yield break;
            }
            _countdown.text = Mathf.RoundToInt(timer).ToString();
            timer -= Time.deltaTime * _timescaleSpeed;
            yield return null;
         }

         StopCountdown();
      }

      IEnumerator ReadyGo()
      {
         yield return new WaitForSeconds(delay);
         if (_forceQuit) 
         {
            _forceQuit = false;
            yield break;
         }

         isRunning = true;
         _countdown.text = "Ready";
         yield return new WaitForSeconds(_readyWaitTime);
         if (_forceQuit) 
         {
            _forceQuit = false;
            yield break;
         }
         _countdown.text = "Go";
         yield return new WaitForSeconds(_goWaitTime);
         if (_forceQuit) 
         {
            _forceQuit = false;
            yield break;
         }

         StopCountdown();
      }
   }

   public void StopCountdown()
   {
      if (_coroutine != null) _forceQuit = true;
      _coroutine = null;
      isRunning = false;
      _canvasGroup.alpha = 0f;
      
      _onFinished?.Invoke();
      _onFinished = null;
   }

   public void StopCountdownNoTrigger()
   {
      if (_coroutine != null) _forceQuit = true;
      _coroutine = null;
      isRunning = false;
      _canvasGroup.alpha = 0f;
      
      _onFinished = null;
   }

   private void OnDestroy()
   {
      StopCountdownNoTrigger();
   }

   public enum CountdownType
   {
      Number,
      ReadyGo
   }
}
