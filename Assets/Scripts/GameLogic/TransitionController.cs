using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    [Header("Menu Transitions")]
    [SerializeField] private GameObject _menuTransition;
    [SerializeField] private float _defaultMenuAnimationSpeed = 0.8f;
    [SerializeField] private float _menuHiddenOffset = 2000;
    private RectTransform _menuRect;

    [Header("Scene Transitions")]
    [SerializeField] private GameObject _sceneTransition;
    [SerializeField] private float _defaultSceneAnimationSpeed = 0.7f;
    [SerializeField] private float _sceneHiddeOffset = 2000;
    private RectTransform _sceneRect;
    
    [Header("Mini Game Transitions")]
    [SerializeField] private GameObject _miniGameTransition;
    [SerializeField] private float _defaultMiniGameAnimationSpeed = 1f;
    [SerializeField] private float _miniGameHiddenOffset = 2000;
    private RectTransform _miniGameRect;

    private RectTransform _activeTransition;
    private float _activeHiddenOffset;
    
    // private delegate void TransitionDelegate(TransitionData data);
    // private struct TransitionData
    // {
    //     public TransitionType type;
    //     public Action onCompleted;
    //     public float timeOverride;
    //
    //     public TransitionData(TransitionType t, Action c, float time)
    //     {
    //         type = t;
    //         onCompleted = c;
    //         timeOverride = time;
    //     }
    // }
    //
    // private Queue<TransitionDelegate> _transitionCoroutine = new Queue<TransitionDelegate>();
    // private Coroutine _queueCoroutine; 
    
    private void Awake()
    {
        _menuRect = _menuTransition.GetComponent<RectTransform>();
        _sceneRect = _sceneTransition.GetComponent<RectTransform>();
        _miniGameRect = _miniGameTransition.GetComponent<RectTransform>();
    }

    // public void StartTransition(TransitionType type = TransitionType.Menu, Action onCompleted = null, float timeOverride = -1)
    // {
    //     TransitionData data = new TransitionData(type, onCompleted, timeOverride);
    //     _transitionCoroutine.Enqueue(Transition);
    //     _queueCoroutine ??= StartCoroutine(TransitionQueue());
    // }
    
    // private IEnumerator TransitionQueue()
    // {
    //     while (_transitionCoroutine.Count > 0)
    //     {
    //         yield return StartCoroutine(_transitionCoroutine.Dequeue().Invoke());
    //     }
    //     _queueCoroutine = null;
    // }

    public void Transition(TransitionType type = TransitionType.Menu, Action onCompleted = null, float timeOverride = -1)
    {
        float animationTime;
        switch (type)
        {
            case TransitionType.Menu:
                animationTime = Mathf.Max(timeOverride, _defaultMenuAnimationSpeed);
                _activeHiddenOffset = _menuHiddenOffset;
                _activeTransition = _menuRect;
                break;
            case TransitionType.Scene:
                animationTime = Mathf.Max(timeOverride, _defaultSceneAnimationSpeed);
                _activeHiddenOffset = _sceneHiddeOffset;
                _activeTransition = _sceneRect;
                break;
            case TransitionType.MiniGame:
                animationTime = Mathf.Max(timeOverride, _defaultMiniGameAnimationSpeed);
                _activeHiddenOffset = _miniGameHiddenOffset;
                _activeTransition = _miniGameRect;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
        TransitionInOut(animationTime, onCompleted);
    }


    private void TransitionInOut(float time, Action inCompleted = null, Action outCompleted = null)
    {
        StartCoroutine(InOut());
        return;

        IEnumerator InOut()
        {
            AudioManager.PlaySound(AudioType.Transition, 0.2f);
            yield return StartCoroutine(ApplyTransition(_activeHiddenOffset, 0, time, () =>
            {
                StartCoroutine(OutLoader());
            }));
            yield break;

            IEnumerator OutLoader()
            {
                inCompleted?.Invoke();
                yield return new WaitUntil(() => Game.CanLoadScene);
                StartCoroutine(ApplyTransition(0, -_activeHiddenOffset, time, () =>
                {
                    outCompleted?.Invoke();
                    _activeHiddenOffset = 0;
                    _activeTransition = null;
                })); 
            }
        }
    }
    private IEnumerator ApplyTransition(float start, float end, float time, Action completed = null)
    {
        float elapsed = 0;
        while (elapsed <= time)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / time);

            float x = Mathf.Lerp(start, end, t);
            if (_activeTransition == null) yield break;
            _activeTransition.anchoredPosition = new Vector2(x, _activeTransition.anchoredPosition.y);
            
            yield return null;
        }
        _activeTransition.anchoredPosition = new Vector2(end, _activeTransition.anchoredPosition.y);
        completed?.Invoke();
    }
}

public enum TransitionType
{
    Menu,
    Scene,
    MiniGame
}
