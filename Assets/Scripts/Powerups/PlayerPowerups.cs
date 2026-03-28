using System;
using System.Collections;
using InputManagement;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPowerups : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup _powerupUI;
    [SerializeField] private Image _radialTimer;
    [SerializeField] private Image _powerupIcon; 
    
    private Powerup _currentPower = null;
    private int _chargesUsed = -1;
    public bool HasPower => _currentPower != null;
    
    // [Header("Powerup References")]
    // [SerializeField] private GameObject _field;

    private Rigidbody2D _rigidbody2D;
    private Player _player;
    private bool _canUsePowerup = true;
    private Coroutine _powerupCoroutine;
    private InputManager _inputManager;

    private Vector2 SpawnPosition => transform.position + (Vector3.right * _player.FacingLeftValue);
    
    private void Awake()
    {
        _powerupUI.alpha = 0;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _player = GetComponent<Player>();
        // _field.SetActive(false);
        _inputManager = GetComponent<InputManager>();
    }

    public void ApplyPower(Powerup p)
    {
        if (_powerupCoroutine != null) StopCoroutine(_powerupCoroutine);
        _powerupUI.alpha = 1f;
        _currentPower = p;
        _powerupIcon.sprite = p.GetIcon();
        _canUsePowerup = true;
        _chargesUsed = 0;
        SetRadialTimer(1f);
    }
    public void RemovePower()
    {
        _currentPower.Expire(this);
        _currentPower = null;
        _powerupUI.alpha = 0f;
    }
    
    public void UsePower()
    {
        if (HasPower && _canUsePowerup)
        {
            AudioManager.PlaySound(AudioTrack.PowerupThrow);
            
            _currentPower.UsePowerup(this);
            StartCoroutine(PowerupTimer(_currentPower.UseCooldown));
            
            if (_currentPower.HasTimer) _powerupCoroutine ??= StartCoroutine(AwaitTimeExpire());
            else CheckCharges();
        }
    }
    
    private IEnumerator AwaitTimeExpire()
    {
        float elapsed = 0;
        float timeToExpire = _currentPower.GetCharges();
        while (elapsed < timeToExpire)
        {
            elapsed += Time.deltaTime;
            SetRadialTimer(1f - (elapsed / timeToExpire));
            yield return null;
        }
        _powerupCoroutine = null;
        RemovePower();
    }

    private void CheckCharges()
    {
        _chargesUsed++;
        SetRadialTimer(1f - (_chargesUsed / _currentPower.GetCharges()));
        if (_chargesUsed >= _currentPower.GetCharges())
        {
            RemovePower();
        }
    }
    
    public void SetRadialTimer(float percent)
    {
        _radialTimer.fillAmount = percent;
    }
    
    #region Powerup Actions

    public void Dash(DashStats stats)
    {
        Vector2 direction = _inputManager.GetMoveInput().normalized;
        if (direction.x == 0) direction.x = _player.FacingLeftValue;
        Vector2 force = direction * stats.dashForce; //new Vector2(direction.x * stats.dashForce, direction.y * stats.yForce);
        _rigidbody2D.AddForce(force, ForceMode2D.Impulse);
    }

    public void Dart(GameObject prefab, DartStats dartStats)
    {
        GameObject o = Instantiate(prefab, SpawnPosition, Quaternion.identity);
        Dart dart = o.GetComponent<Dart>();
        dart.Throw(gameObject, dartStats, _player.FacingLeftValue);
    }

    // public void Field(FieldStats stats)
    // {
    //     _field.GetComponent<Field>().StartField(stats.Force);
    //     _field.SetActive(true);
    // }
    // public void DisableField()
    // {
    //     _field.SetActive(false);
    // }

    public void Trap(GameObject prefab, TrapStats trapStats)
    {
        GameObject o = Instantiate(prefab, SpawnPosition, Quaternion.identity);
        Trap trap = o.GetComponent<Trap>();
        trap.Throw(gameObject, trapStats, _player.FacingLeftValue);
    }

    public void SpawnExplosion(GameObject explosion, Transform pos)
    {
        Instantiate(explosion, pos.position, Quaternion.identity);
    }

    private IEnumerator PowerupTimer(float duration)
    {
        _canUsePowerup = false;
        yield return new WaitForSeconds(duration);
        _canUsePowerup = true;
    }
    #endregion
}
