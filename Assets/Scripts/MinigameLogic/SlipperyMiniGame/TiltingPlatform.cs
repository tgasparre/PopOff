using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TiltingPlatform : MonoBehaviour
{
    private int numPlayersOnLeft = 0;
    private int numPlayersOnRight = 0;

    [SerializeField] private TiltingPlatformSensor leftSide;
    [SerializeField] private TiltingPlatformSensor rightSide;
    
    private Coroutine tiltCoroutine = null;
    private Coroutine gameCoroutine = null;
    
    //rotation parameters
    private float rotation;
    private float startValue;
    private float rightEndValue = -4f;
    private float leftEndValue = 4f;
    
    void Start()
    {
        if (leftSide != null)
        {
            leftSide.PlayerEntered += OnPlayerEnteredSide;
            leftSide.PlayerExited += OnPlayerExitSide;
        }
        if (rightSide != null)
        {
            rightSide.PlayerEntered += OnPlayerEnteredSide;
            rightSide.PlayerExited += OnPlayerExitSide;
        }
    }

    public void StartTiltingPlatform(float minigameDuration)
    {
        gameCoroutine = StartCoroutine(TiltPlatform(minigameDuration));
    }

    public void EndGame()
    {
        StopAllCoroutines();
    }

    //when num players on one side is greater than the other, tilt the platform
    IEnumerator TiltPlatform(float minigameDuration)
    {
        float timeElapsed = 0;
        while (timeElapsed < minigameDuration)
        {
            if (tiltCoroutine != null) StopCoroutine(tiltCoroutine);

            if (numPlayersOnLeft > numPlayersOnRight)
            {
                tiltCoroutine = StartCoroutine(Tilt(leftEndValue));
            }
            else if (numPlayersOnLeft < numPlayersOnRight)
            {
                tiltCoroutine = StartCoroutine(Tilt(rightEndValue));
            }
            else
            {
                tiltCoroutine = StartCoroutine(Tilt(0));
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

    }

    IEnumerator Tilt(float endValue)
    {
        //code from claude - platform was spinning out of control when trying to pass in -2
        startValue = Mathf.DeltaAngle(0, transform.eulerAngles.z);
        //end code from claude
        
        float timeElapsed = 0;
        
        while (timeElapsed < 5)
        {
            rotation = Mathf.Lerp(startValue, endValue, timeElapsed / 3);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, rotation);
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        tiltCoroutine = null; 

        rotation = endValue;
    }

    private void OnPlayerEnteredSide(Side side)
    {
        switch (side)
        {
            case Side.Left:
                ++numPlayersOnLeft;
                break;
            case Side.Right:
                ++numPlayersOnRight;
                break;
        }
    }

    private void OnPlayerExitSide(Side side)
    {
        switch (side)
        {
            case Side.Left:
                --numPlayersOnLeft;
                break;
            case Side.Right:
                --numPlayersOnRight;
                break;
        }
    }

    private void OnDestroy()
    {
        if (leftSide != null)
        {
            leftSide.PlayerEntered -= OnPlayerEnteredSide;
            leftSide.PlayerExited -= OnPlayerExitSide;
        }

        if (rightSide != null)
        {
            rightSide.PlayerEntered -= OnPlayerEnteredSide;
            rightSide.PlayerExited -= OnPlayerExitSide;
        }
    }
}
