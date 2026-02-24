using System;
using InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputManager), typeof(PlayerInput))]
public class PlayerBase : MonoBehaviour
{
    protected PlayerInput _playerInput;
    protected InputManager _playerInputManager;
    public void Register(PlayerInput input)
    {
        _playerInput = input;
        ActivePlayersTracker.LookForPlayerSpawn(this);
        DontDestroyOnLoad(gameObject);
    }
    public int PlayerIndex => _playerInput.playerIndex;
    
    public bool IsFacingLeft => FacingLeftValue == -1;
    public int FacingLeftValue => _playerInputManager.GetFacingDirection();
    
    protected Rigidbody2D _rigidbody2D;

    protected void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerInputManager = GetComponent<InputManager>();
    }
}
