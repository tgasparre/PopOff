using System;
using System.Collections;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private static readonly int Movement = Animator.StringToHash("movement");
    private static readonly int InAir = Animator.StringToHash("inAir");
    private static readonly int Attack = Animator.StringToHash("attack");
    private static readonly int Ultimate = Animator.StringToHash("ultimate");
    private static readonly int Jump = Animator.StringToHash("jump");
    private static readonly int StartAttack = Animator.StringToHash("startAttack");
    private static readonly int Died = Animator.StringToHash("died");

    [SerializeField] private SpriteRenderer _render;
    [SerializeField] private Animator _animator;
    private Player _player;

    private Coroutine _damageCoroutine;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    public void DamageFlash()
    {
        _damageCoroutine ??= StartCoroutine(Flash(0.15f));
        return;
        
        IEnumerator Flash(float flashTime)
        {
            float elapsed = 0f;
            Color defaultColor = _render.color;
            while (elapsed < flashTime)
            {
                elapsed += Time.deltaTime;
                _render.color = (Mathf.FloorToInt(elapsed * 20) % 2 == 0) ? Color.white : Color.red;
                yield return null;
            }
            _render.color = defaultColor;
            _damageCoroutine = null;
        }   
    }

    private void LateUpdate()
    {
        _render.flipX = !_player.IsFacingLeft;
        
        _animator.SetFloat(Movement, Mathf.RoundToInt(Mathf.Abs(_player.Movement)));
        _animator.SetBool(InAir, _player.InAir);
    }

    public void TriggerAttack(int direction)
    {
        _animator.SetInteger(Attack, direction);
        _animator.SetTrigger(StartAttack);
    }

    public void TriggerJump()
    {
        _animator.SetTrigger(Jump);
    }

    public void TriggerUltimate()
    {
        _animator.SetTrigger(Ultimate);
    }

    public void TriggerDeath()
    {
        _animator.SetTrigger(Died);
    }
}
