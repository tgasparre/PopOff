using System;
using System.Collections;
using ControllerSystem.Platformer2D;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : PlayerBase
{
    [SerializeField] private PlayerStats _defaultStats;
 
    public event Action<Player> OnDeath;
    public event Action<float> UICallback_PlayerHealthChange;
    
    // ===== References =====
    public PlayerPowerups powerups { private set; get; }
    public AttackHurtbox hurtbox { private set; get; }
    public PlayerStats playerStats { private set; get; }
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

    public void TriggerDeath()
    {
        _animation.TriggerDeath();
        Invoke(nameof(KillPlayer), 0.28f);
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

    private Coroutine _damageCoroutine;
    private Coroutine _freezeMovementCoroutine;
    
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
        ResetWeightClass();

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
        if (!_playerInputManager.isInputEnabled) return;
        if (context.performed) powerups.UsePower();
    }

    #endregion
    
    #region Health
    public void TakeDamage(int damage)
    {
        PlayerHealth -= damage;
        _animation.DamageFlash();
        
        if (PlayerHealth <= 0)
        {
            Game.CameraShake.DeathShake();
            AudioManager.PlaySound(AudioTrack.PlayerDeath);
            TriggerDeath();
        }
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
    
    private void KillPlayer()
    {
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
        _rigidbody2D.AddForce(Vector2.up * CombatParameters.POGO_FORCE, ForceMode2D.Impulse);
    }

    public void ResetWeightClass()
    {
        if (_defaultStats == null) Debug.LogError("Default Stats should not be null!");
        else if (playerStats == null) AssignWeightClass(_defaultStats);
        else AssignWeightClass(playerStats);
    }
    
    public void AssignWeightClass(PlayerStats stats)
    {
        playerStats = stats;
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
    
    //replaced AddHitStun with FreezePlayer instead which makes sure no coroutine is stopped early, disabling movement 
    [Obsolete("replaced with FreezePlayer")]
    IEnumerator AddHitStun(float duration)
    {
        if (IsFrozen) yield break;
        
        FreezePlayerMovement();
        yield return new WaitForSeconds(duration);
        UnfreezePlayerMovement();
        _freezeMovementCoroutine = null;
    }
}

