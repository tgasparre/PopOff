using System;
using System.Collections;
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
    private Coroutine _transitionCoroutine;
    
    private void Awake()
    {
        _menuRect = _menuTransition.GetComponent<RectTransform>();
        _sceneRect = _sceneTransition.GetComponent<RectTransform>();
        _miniGameRect = _miniGameTransition.GetComponent<RectTransform>();
    }

    public void Transition(TransitionType type = TransitionType.Menu, Action completed = null, float timeOverride = -1)
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
        TransitionInOut(animationTime, completed);
    }


    private void TransitionInOut(float time, Action inCompleted = null, Action outCompleted = null)
    {
        if (_transitionCoroutine != null) { Debug.LogWarning("Two transitions were run at once"); return;}
        _transitionCoroutine = StartCoroutine(InOut());
        return;

        IEnumerator InOut()
        {
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
                    _transitionCoroutine = null;
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
