using System.Collections;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField] private GameObject _joinText;
    [SerializeField] private CountdownUI _startGameCountdown;
    [SerializeField] private int _defaultStartTime = 3;
    
    [Space]
    [SerializeField] private GameObject _buttonTop;
    private Vector2 _startingPos;
    private Coroutine _buttonPressDown;
    
    private GameObject _touchingObject;

    private void Awake()
    {
        _startGameCountdown.gameObject.SetActive(false);
        _startingPos = _buttonTop.transform.position;
        ActivePlayersTracker.Joined += CheckCollision;
        
        ResetButton();
    }

    private void OnDestroy()
    {
        ActivePlayersTracker.Joined -= CheckCollision;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player") && _touchingObject != null) return;
        _touchingObject = other.gameObject;
        CheckCollision(null);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == _touchingObject)
        {
            ResetButton();
            _touchingObject = null;
        }
    }

    private void CheckCollision(PlayerController _)
    {
        if (_touchingObject == null) return;
        if (_startGameCountdown.isRunning || (Game.PlayerCount <= 1 && !Game.Instance.bypassOnePlayerBlock)) return;

        _buttonPressDown ??= StartCoroutine(PressButton());
        
        _joinText.SetActive(false);
        _startGameCountdown.gameObject.SetActive(true);
        _startGameCountdown.StartCountdown(StartGame, 0.5f);
    }
    
    private void ResetButton()
    {
        if (_startGameCountdown == null) return;
        _joinText.SetActive(true);
        _startGameCountdown.StopCountdownNoTrigger();
        _startGameCountdown.InitializeCountdown(_defaultStartTime);
        _startGameCountdown.gameObject.SetActive(false);

        if (_buttonPressDown != null)
        {
            StopCoroutine(_buttonPressDown);
            _buttonPressDown = null;
        }
        _buttonTop.transform.position = _startingPos;
    }

    private IEnumerator PressButton()
    {
        float elapsed = 0;
        float target = _buttonTop.transform.position.y - 0.32f;
        while (elapsed <= _defaultStartTime)
        {
            elapsed += Time.deltaTime;
            Vector2 temp = _buttonTop.transform.position;
            temp.y = Mathf.Lerp(_startingPos.y, target, elapsed / _defaultStartTime);
            _buttonTop.transform.position = temp;
            yield return null;
        }
        _buttonPressDown = null;
    }

    private void StartGame()
    {
        Game.currentState = GameStates.Playing;
    }
}
