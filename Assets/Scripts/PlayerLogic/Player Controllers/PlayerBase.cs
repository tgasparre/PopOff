using System;
using InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputManager))]
public abstract class PlayerBase : MonoBehaviour
{
    [SerializeField] protected PlayerController _controller;
    [SerializeField] private SpriteRenderer _mainSpriteRender;
    protected InputManager _playerInputManager;
    
    public bool IsFacingLeft => FacingLeftValue == -1;
    public int FacingLeftValue => _playerInputManager.GetFacingDirection();
    public int PlayerIndex => _controller.PlayerIndex;
    
    protected Rigidbody2D _rigidbody2D;

    public abstract bool InAir { get; }

    protected void Awake()
    {
        Register();
    }

    public void Register()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerInputManager = GetComponent<InputManager>();
        _mainSpriteRender.color = Game.Instance.PlayerColors[PlayerIndex];
    }
    
    public void FreezePlayer()
    {
        _rigidbody2D.bodyType = RigidbodyType2D.Static;
    }

    public void UnfreezePlayer()
    {
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
    }

    public void SetPlayerFreeze(bool value)
    {
        if (value) FreezePlayer();
        else UnfreezePlayer();
    }

    public void Spawn(Vector2 pos, bool showIndicators)
    {
        UnfreezePlayer();
        _rigidbody2D.linearVelocity = Vector2.zero;
        transform.position = pos;
        
        AudioManager.PlaySound(AudioTrack.PlayerAppear, delay: 0.15f);
        
        if (showIndicators && this is Player player)
        {
            player.StartIndicator();
        }
    }
}
