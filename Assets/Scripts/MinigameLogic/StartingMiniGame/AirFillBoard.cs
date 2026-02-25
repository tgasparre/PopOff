using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AirFillBoard : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    private Slider _progressSlider;

    private void Awake()
    {
        _progressSlider = GetComponentInChildren<Slider>();
        Game.Instance.GetPlayers()[playerIndex].OnJump = Fill;
    }

    public void Fill(InputAction.CallbackContext context)
    {
        //TODO
        Debug.Log("Pressing the fill button oh my god");
    }

}
