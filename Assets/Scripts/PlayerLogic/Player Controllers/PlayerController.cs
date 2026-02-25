using System;
using InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerState _playerState;
    public PlayerState CurrentState
    {
        get => _playerState;
        set => SwitchState(value);
    }
    
    [Header("Player Objects")]
    [SerializeField] private GameObject _startingPlayer;
    [SerializeField] private GameObject _defaultPlayer;
    
    [Header("Input Handlers")]
    [SerializeField] private InputManager _startingInputManager;
    [SerializeField] private InputManager _defaultInputManager;

    private Action<InputAction.CallbackContext> OnMove;
    private Action<InputAction.CallbackContext> OnJump;

    private void Awake()
    {
        CurrentState = _playerState;
    }

    private void SwitchState(PlayerState state)
    {
        _defaultPlayer.SetActive(false);
        _startingPlayer.SetActive(false);
        switch (state)
        {
            case PlayerState.Starting:
                OnMove = _startingInputManager.Move;
                OnJump = _startingInputManager.Jump;
                _startingPlayer.SetActive(true);
                break;
            case PlayerState.Fighting:
                OnMove = _defaultInputManager.Move;
                OnJump = _defaultInputManager.Jump;
                _defaultPlayer.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        _playerState = state;
    }

    public void Move(InputAction.CallbackContext context)
    {
        OnMove.Invoke(context);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        OnJump.Invoke(context);
    }
    
    public void DEBUG_ChangePlayer(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CurrentState == PlayerState.Starting) CurrentState = PlayerState.Fighting;
            else if (CurrentState == PlayerState.Fighting) CurrentState = PlayerState.Starting;
        }
    }
    
    
}

public enum PlayerState
{
    Starting,
    Fighting
}