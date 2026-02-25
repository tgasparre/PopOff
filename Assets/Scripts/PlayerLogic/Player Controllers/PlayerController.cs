using System;
using InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerState _playerState;
    public PlayerState CurrentState
    {
        get => _playerState;
        set => SwitchState(value);
    }

    private PlayerInput _playerInput;
    public void Register(PlayerInput input)
    {
        _playerInput = input;
        CurrentState = _playerState;
        ActivePlayersTracker.LookForPlayerSpawn(ActivePlayer);
        DontDestroyOnLoad(gameObject);
    }
    public int PlayerIndex => _playerInput.playerIndex;
    public PlayerBase ActivePlayer => (CurrentState == PlayerState.Starting) ? _startingPlayer : _defaultPlayer;
    
    [Header("Player Objects")]
    [SerializeField] private PlayerBase _startingPlayer;
    [SerializeField] private PlayerBase _defaultPlayer;
    
    [Header("Input Handlers")]
    [SerializeField] private InputManager _startingInputManager;
    [SerializeField] private InputManager _defaultInputManager;

    private Action<InputAction.CallbackContext> OnMove;
    public Action<InputAction.CallbackContext> OnJump;

    private void SwitchState(PlayerState state)
    {
        _defaultPlayer.gameObject.SetActive(false);
        _startingPlayer.gameObject.SetActive(false);
        switch (state)
        {
            case PlayerState.Starting:
                OnMove = _startingInputManager.Move;
                OnJump = _startingInputManager.Jump;
                _startingPlayer.gameObject.SetActive(true);
                break;
            case PlayerState.Fighting:
                OnMove = _defaultInputManager.Move;
                OnJump = _defaultInputManager.Jump;
                _defaultPlayer.gameObject.SetActive(true);
                break;
            case PlayerState.CharacterMiniGame:
                OnMove = null;
                //OnJump assigned by AirFillBoard
                _startingPlayer.gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        _playerState = state;
    }

    public void Move(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(context);
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
    CharacterMiniGame,
    Fighting
}