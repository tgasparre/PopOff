using System;
using InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputManager))]
public abstract class PlayerBase : MonoBehaviour
{
    protected PlayerController _controller;
    protected InputManager _playerInputManager;
    
    public bool IsFacingLeft => FacingLeftValue == -1;
    public int FacingLeftValue => _playerInputManager.GetFacingDirection();
    public int PlayerIndex => _controller.PlayerIndex;
    
    protected Rigidbody2D _rigidbody2D;

    protected void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _controller = GetComponentInParent<PlayerController>();
        _playerInputManager = GetComponent<InputManager>();
    }
}
