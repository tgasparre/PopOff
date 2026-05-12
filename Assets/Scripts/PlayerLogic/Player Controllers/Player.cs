using System;
using System.Collections;
using ControllerSystem.Platformer2D;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : PlayerBase
{
    public event Action<Player> OnDeath;
    public event Action<float> UICallback_PlayerHealthChange;
    
    private PlayerStats _defaultStats;
    
    // ===== References =====
    public PlayerPowerups powerups { private set; get; }
    public AttackHurtbox hurtbox { private set; get; }
    public PlayerStats activePlayerStats { private set; get; }
    public UltimateAttackTracker ultimateAttackTracker { private set; get; }
    public int PlayerHealth
    {
        get
        {
            if (hurtbox != null) return hurtbox.HP;
            return 0;
        }
        set 
        {
            if (hurtbox != null)
            {
                hurtbox.HP = value;
                UICallback_PlayerHealthChange?.Invoke(hurtbox.HP);
            }
        }
    }
    
    public bool IsFrozen { get; private set; }
    public float Movement => _playerInputManager.GetMoveInput().x;
    public override bool InAir => !_jumpModule.Grounded;
    public void TriggerAttack(int direction) { _animation.TriggerAttack(direction); }

    //animation trigger so it doesn't kill the player again while it's playing the death animation
    private bool _hasDied = false;
    // public void ResetHasDied() { _hasDied = false; }
    
    public void TriggerDeath()
    {
        // if (PlayerIndex == ActivePlayersTracker.IMMORTAL_PLAYER_INDEX)
        // {
        //     if (PlayingState.CurrentGameplayState == GameplayStates.Combat)
        //     {
        //         PlayerHealth = 1;
        //         return;
        //     }
        // }
        
        if (_hasDied) return;
        
        Game.CameraShake.DeathShake();
        AudioManager.PlaySound(AudioTrack.PlayerDeath, 0.1f);
        
        _hasDied = true;
        _animation.TriggerDeath();
        
        StartCoroutine(DeathCountdown());
        return;

        IEnumerator DeathCountdown()
        {
            yield return new WaitForSecondsRealtime(0.452f);
            KillPlayer();
        }
    }

    public void TriggerUltimate()
    {
        _animation.TriggerUltimate();
    }
    public void TriggerJump()
    {
        AudioManager.PlaySound(AudioTrack.PlayerJump);
        _animation.TriggerJump();
    }
    private void TriggerWallJump(int dir)
    {
        TriggerJump();
    }

    // ===== Internal References =====
    private PlayerAnimation _animation;
    private Powerup _attachedPowerup;
    private float _savedMovementSpeed;
    private PlatformerHorizontalMovementModule _horizontalMovementModule;
    private PlatformerJumpModule _jumpModule;
    private PlatformerWallModule _wallModule;
    private IndicatorUI _indicatorUI;
    private CombatInputHandler _combatInputHandler;

    private Coroutine _damageCoroutine;
    private Coroutine _freezeMovementCoroutine;

    private bool _canPogo = true;
    
    new void Awake()
    {
        base.Awake();
        hurtbox = GetComponentInChildren<AttackHurtbox>();
        powerups = GetComponent<PlayerPowerups>();
        _animation = GetComponentInChildren<PlayerAnimation>();
        ultimateAttackTracker = GetComponent<UltimateAttackTracker>();
        _jumpModule = GetComponent<PlatformerJumpModule>();
        _wallModule = GetComponent<PlatformerWallModule>();
        _horizontalMovementModule = GetComponent<PlatformerHorizontalMovementModule>();
        _combatInputHandler = GetComponent<CombatInputHandler>();
        //SetWeightClass(DEBUG_stats);

        _jumpModule.JumpTriggered += TriggerJump;
        _wallModule.OnWallJump += TriggerWallJump;
    }
    
    private void OnDestroy()
    {
        OnDeath = null;
        if (_jumpModule) _jumpModule.JumpTriggered -= TriggerJump;
        if (_wallModule) _wallModule.OnWallJump -= TriggerWallJump;
    }

    public void Register(Action<Player> deathCallback)
    {
        _indicatorUI = GetComponentInChildren<IndicatorUI>();
        _indicatorUI.SetColor(Game.Instance.PlayerColors[PlayerIndex]);
        
        base.Register();
        OnDeath = deathCallback;
    }

    public void StartIndicator()
    {
        _indicatorUI.StartIndicator();
    }

    #region Inputs

    public void UsePower(InputAction.CallbackContext context)
    {
        if (_playerInputManager == null) return;
        if (!_playerInputManager.isInputEnabled) return;
        if (context.performed) powerups.UsePower();
    }

    #endregion
    
    #region Health
    public void TakeDamage(int damage)
    {
        PlayerHealth -= damage;
        _animation.DamageFlash();
        
        if (PlayerHealth <= 0) TriggerDeath();
        else
        {
            Game.CameraShake.HitShake();
            AudioManager.PlaySound(AudioTrack.PlayerHit);
        }
    }

    public void InstantDeath()
    {
        PlayerHealth = 0;
        TriggerDeath();
    }
    
    public void KillPlayer()
    {
        _hasDied = false;
        OnDeath(this);
    }

    public void HealHP(int heal)
    {
        PlayerHealth += heal;
        if (PlayerHealth > CombatParameters.MAX_PLAYER_HEALTH)
        {
            PlayerHealth = CombatParameters.MAX_PLAYER_HEALTH;
        }
    }

    public void ResetHealth()
    {
        hurtbox.ResetHealth();
    }
    #endregion
    
    #region Combat 
    public void ApplyHitStun(float duration)
    { 
        FreezePlayer(duration, 0.1f);
    }
    
    public void ApplyKnockback(Vector2 direction, float knockbackForce)
    {
        _damageCoroutine ??= StartCoroutine(AddKnockback());
        return;
        
        IEnumerator AddKnockback()
        {
            float elapsedTime = 0f;
            while (elapsedTime < CombatParameters.KNOCKBACK_DURATION)
            {
                float normalizedTime = elapsedTime / CombatParameters.KNOCKBACK_DURATION;
                float currentForce = CombatParameters.KNOCKBACK_CURVE.Evaluate(normalizedTime) * knockbackForce;

                _rigidbody2D.AddForce(direction.normalized * currentForce * 10);
            
                elapsedTime += Time.fixedDeltaTime;
                yield return null;
            }
            _damageCoroutine = null;
        }
    }

    public void Pogo()
    {
        if (!_canPogo) return;
        _rigidbody2D.AddForce(Vector2.up * CombatParameters.POGO_FORCE, ForceMode2D.Impulse);
        _canPogo = false;
        Invoke(nameof(EnablePogo), 0.5f);
    }
    private void EnablePogo() => _canPogo = true;

    public void ResetWeightClass()
    {
        if (_defaultStats == null) return;
        AssignWeightClass(_defaultStats);
    }

    public void SetWeightClass(PlayerStats stats)
    {
        _defaultStats = stats;
        transform.localScale = Vector2.one * stats.IncreaseScaleSize;
        AssignWeightClass(_defaultStats);
    }
    
    public void AssignWeightClass(PlayerStats stats)
    {
        activePlayerStats = stats;
        switch (stats.Type)
        {
            case WeightClassType.Light:
                _jumpModule.Config.SetJumpTypeToLight();
                _horizontalMovementModule.SetMovementTypeToFast();
                break;
            case WeightClassType.Heavy:
                _jumpModule.Config.SetJumpTypeToHeavy();
                _horizontalMovementModule.SetMovementTypeToSlow();
                break;
            case WeightClassType.Default:
                //reset to regular stats if not light or heavy
                _jumpModule.Config.ResetJumpType();
                _horizontalMovementModule.ResetMovement();
                break;
            case WeightClassType.Custom:
                _jumpModule.Config.SetParameters(stats.PlayerParameters);
                _horizontalMovementModule.SetParameters(stats.PlayerParameters);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    #endregion
    
    public void FreezePlayerMovement()
    {
        IsFrozen = true;
        _savedMovementSpeed = _horizontalMovementModule.GetMovementSpeed();
        _horizontalMovementModule.SetMovementSpeed(0f);
        _jumpModule.Config.DisableJump();
    }

    public void UnfreezePlayerMovement()
    {
        IsFrozen = false;
        _horizontalMovementModule.SetMovementSpeed(_savedMovementSpeed);
        _jumpModule.Config.ReEnableJump();
    }

    /// <summary>
    /// Freeze the player for a brief moment 
    /// </summary>
    public void FreezePlayer(float duration, float cooldown = 0.35f)
    {
        _freezeMovementCoroutine ??= StartCoroutine(Freeze());
        return;
        
        IEnumerator Freeze()
        {
            if (IsFrozen) yield break;
        
            FreezePlayerMovement();
            yield return new WaitForSeconds(duration);
            UnfreezePlayerMovement();
            yield return new WaitForSeconds(cooldown);
            _freezeMovementCoroutine = null;
        }
    }

    public void DEBUG_UnlockUltimate()
    {
        // _combatInputHandler.DEBUG_UnlockUltimate();
    }
}

