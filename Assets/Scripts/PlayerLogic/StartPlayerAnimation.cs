using System;
using UnityEngine;

public class StartPlayerAnimation : MonoBehaviour
{
    [SerializeField] private PlayerStart _playerStart;
    private SpriteRenderer _renderer;
    
    private static readonly int Jump = Animator.StringToHash("jump");
    private static readonly int Movement = Animator.StringToHash("movement");
    private static readonly int InAir = Animator.StringToHash("inAir");
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void TriggerJump()
    {
        _animator.SetTrigger(Jump);
    }

    private void LateUpdate()
    {
        _renderer.flipX = !_playerStart.IsFacingLeft;
        
        _animator.SetFloat(Movement, Mathf.RoundToInt(Mathf.Abs(_playerStart.Movement)));
        _animator.SetBool(InAir, _playerStart.InAir);
    }
}
