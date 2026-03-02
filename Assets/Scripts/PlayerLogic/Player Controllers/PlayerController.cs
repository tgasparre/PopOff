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
    public void Register(PlayerInput input, Action<Player> deathCallback)
    {
        _playerInput = input;
        _defaultPlayer.OnDeath = deathCallback;
        CurrentState = _playerState;
        ActivePlayersTracker.LookForPlayerSpawn(ActivePlayer);
        DontDestroyOnLoad(gameObject);
    }
    public int PlayerIndex => _playerInput.playerIndex;
    public PlayerBase ActivePlayer => (CurrentState == PlayerState.Starting) ? _startingPlayer : _defaultPlayer;
    
    [Header("Player Objects")]
    [SerializeField] private PlayerStart _startingPlayer;
    [SerializeField] private Player _defaultPlayer;
    
    [Header("Input Handlers")]
    [SerializeField] private InputManager _startingInputManager;
    [SerializeField] private InputManager _defaultInputManager;

    private Action<InputAction.CallbackContext> OnMove;
    public Action<InputAction.CallbackContext> OnJump;
    private bool _inputEnabled = true;

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
            case PlayerState.StartingMiniGame:
                OnMove = null;
                //OnJump assigned by AirFillBoard
                _startingPlayer.gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        _playerState = state;
    }

    public void AssignWeightClass(PlayerStats stat)
    {
        _defaultPlayer.AssignWeightClass(stat);
    }

    public void SetInputEnabled(bool value)
    {
        _startingInputManager.SetEnabled(value);
        _defaultInputManager.SetEnabled(value);
        _inputEnabled = value;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!_inputEnabled) return;
        OnMove?.Invoke(context);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!_inputEnabled) return;
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

    private bool DEBUG_toggle = false;
    public void DEBUG_DisableInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SetInputEnabled(!DEBUG_toggle);
            DEBUG_toggle = !DEBUG_toggle;
        }
    }
}

public enum PlayerState
{
    Starting,
    StartingMiniGame,
    Fighting
}