using System;
using System.Collections;
using InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;

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
        CurrentState = _playerState;
        
        _startingPlayer.Register();
        _defaultPlayer.Register(deathCallback);

        _animator.runtimeAnimatorController = Game.Instance.GetPlayerAnimation(PlayerIndex);
        _render.sortingOrder += PlayerIndex;
        
        ActivePlayersTracker.LookForPlayerSpawn(ActivePlayer);
        DontDestroyOnLoad(gameObject);
    }
    public int PlayerIndex => _playerInput.playerIndex;
    public PlayerBase ActivePlayer => (CurrentState == PlayerState.Fighting) ? _defaultPlayer : _startingPlayer;
    public PlayerUIDisplayer PlayerUI { get; private set; } = null;
    
    [Header("Player Objects")]
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _render;
    [SerializeField] private PlayerStart _startingPlayer;
    [SerializeField] private Player _defaultPlayer;
    public AttackHurtbox PlayerHurtbox => _defaultPlayer.GetComponentInChildren<AttackHurtbox>();
    public void ApplyPowerup(Powerup p) { _defaultPlayer.powerups.ApplyPower(p); }
    public int PlayerHealth
    {
        get => _defaultPlayer.PlayerHealth;
        set => _defaultPlayer.PlayerHealth = value;
    }

    private bool _inAir => ActivePlayer.InAir;
    
    [Header("Input Handlers")]
    [SerializeField] private InputManager _startingInputManager;
    [SerializeField] private InputManager _defaultInputManager;

    private Action<InputAction.CallbackContext> OnMove;
    public Action<InputAction.CallbackContext> OnJump;
    private bool _inputEnabled = true;

    private bool _startingMiniGameFreeze = false;
    
    //===== Sound Effects =====
    private const float SFX_INTERVAL = 0.5f;
    private Coroutine _movingCoroutine;

    private void SwitchState(PlayerState state)
    {
        _defaultPlayer.gameObject.SetActive(false);
        _startingPlayer.gameObject.SetActive(false);
        _startingMiniGameFreeze = false;
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
                _defaultPlayer.ResetHealth();
                break;
            case PlayerState.StartingMiniGame:
                OnMove = null;
                //OnJump assigned by StartingMiniGame
                _startingPlayer.gameObject.SetActive(true);
                _startingMiniGameFreeze = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        _playerState = state;
        
        //initialize UI
        if (_playerState == PlayerState.Fighting && PlayerUI == null) PlayerUI = GameCanvas.Instance.CreatePlayerUI(this); 
    }

    #region Inputs
    
    public void SetInputEnabled(bool value)
    {
        _startingInputManager.SetEnabled(value);
        _defaultInputManager.SetEnabled(value);
        _inputEnabled = value;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!_inputEnabled) return;
        _movingCoroutine ??= StartCoroutine(SFXDelay());
        OnMove?.Invoke(context);
        return;

        IEnumerator SFXDelay()
        {
            while (context.started && !_startingMiniGameFreeze)
            {
                float xDir = context.ReadValue<Vector2>().x;
                if (_inAir || xDir == 0)
                {
                    yield return null;
                    continue;
                }
                AudioManager.PlaySound(AudioTrack.PlayerMove);
                yield return new WaitForSeconds(SFX_INTERVAL);
            }
            _movingCoroutine = null;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!_inputEnabled) return;
        OnJump.Invoke(context);
    }
    
    public void Pause(InputAction.CallbackContext context)
    {
        if (!_inputEnabled) return;
        if (context.performed) Game.currentState = GameStates.Pause;
    }
    
    public void DEBUG_ChangePlayer(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1, 1f)).normalized;
            _defaultPlayer.ApplyKnockback(randomDirection, CombatParameters.KNOCKBACK_FORCE);
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
    
    #endregion
}

public enum PlayerState
{
    Starting,
    StartingMiniGame,
    Fighting
}