using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Trailer_textappear : MonoBehaviour
{
    [SerializeField] private GameObject _leftAppear;
    [SerializeField] private GameObject _rightAppear;
    [SerializeField] private CanvasGroup _leftGroup;
    [SerializeField] private CanvasGroup _rightGroup;
    
    private void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            StartCoroutine(AppearLeft());
        }
    }

    IEnumerator AppearLeft()
    {
        _leftGroup.alpha = _rightGroup.alpha = 0f;
        _leftAppear.SetActive(false);
        _rightAppear.SetActive(false);
        yield return new WaitForSeconds(1f);
        _leftAppear.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        _leftGroup.alpha = 1f;
    }

    public void StartRight()
    {
        StartCoroutine(AppearRight());
    }

    private IEnumerator AppearRight()
    {
        _rightAppear.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        _rightGroup.alpha = 1f;
    }
}
