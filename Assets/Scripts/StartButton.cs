using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _startGameText;
    [SerializeField] private int _defaultStartTime = 3;
    private float _timer = 0;
    private Coroutine _runningTimer = null;

    private void Awake()
    {
        _startGameText.gameObject.SetActive(false);
        _timer = _defaultStartTime;
    }

    private void OnDestroy()
    {
        ResetCoroutine();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_runningTimer != null || (Game.PlayerCount <= 1 || !Game.Instance.bypassOnePlayerBlock)) return;
        _startGameText.gameObject.SetActive(true);
        _runningTimer = StartCoroutine(StartGameTimer());
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        ResetCoroutine();
    }

    private IEnumerator StartGameTimer()
    {
        while (_timer >= 0)
        {
            _timer -= Time.deltaTime;
            _startGameText.text = ((int)_timer + 1).ToString();
            yield return null;
        }
        Game.currentState = GameStates.Playing;
        _runningTimer = null;
    }

    private void ResetCoroutine()
    {
        if (_runningTimer == null) return;
        _timer = _defaultStartTime;
        _startGameText.gameObject.SetActive(false);
        StopCoroutine(_runningTimer);
        _runningTimer = null;
    }
}
