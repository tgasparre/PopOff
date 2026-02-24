using System;
using System.Collections;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private static readonly int Movement = Animator.StringToHash("movement");
    private static readonly int InAir = Animator.StringToHash("inAir");
    private static readonly int Attack = Animator.StringToHash("attack");

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
            Color _defaultColor = _render.color;
            while (elapsed < flashTime)
            {
                elapsed += Time.deltaTime;
                _render.color = (Mathf.FloorToInt(elapsed * 20) % 2 == 0) ? Color.white : Color.red;
                yield return null;
            }
            _render.color = _defaultColor;
            _damageCoroutine = null;
        }   
    }

    private void LateUpdate()
    {
        _render.flipX = !_player.IsFacingLeft;
        
        _animator.SetFloat(Movement, _player.Movement);
        _animator.SetBool(InAir, _player.InAir);
    }

    public void TriggerAttack()
    {
        _animator.SetTrigger(Attack);
    }
}
