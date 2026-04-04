using System;
using System.Collections;
using TMPro;
using UnityEngine;

public abstract class CountdownUI : MonoBehaviour
{
   [SerializeField] protected float _timescaleSpeed = 1f;
   
   protected bool _forceQuit = false;
   
   private Coroutine _coroutine = null;
   private Action _onFinished;
   private CanvasGroup _canvasGroup;
   
   public bool isRunning { get; protected set; } = false;
   
   protected void Awake()
   { 
      _canvasGroup = GetComponent<CanvasGroup>();
      _canvasGroup.alpha = 0;
   }
   
   public Coroutine StartCountdown(Action onFinished = null, float delay = 0f)
   {
      _forceQuit = false;
      _canvasGroup.alpha = 1f;
      _onFinished = onFinished;
      _coroutine ??= StartCoroutine(Countdown(delay));
      return _coroutine;
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

   public virtual void StopCountdownNoTrigger()
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

   public void DEBUG_StartCountdown()
   {
      StartCountdown();
   }
   protected abstract IEnumerator Countdown(float delay);
}
