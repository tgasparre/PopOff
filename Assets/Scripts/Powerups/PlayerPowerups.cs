using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPowerups : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup _powerupUI;
    [SerializeField] private Image _radialTimer;
    [SerializeField] private Image _powerupIcon; 
    
    private Powerup _currentPower = null;
    public bool HasPower => _currentPower == null;
    
    [Header("Powerup References")]
    [SerializeField] private GameObject _field;
    private Rigidbody2D _rigidbody2D;
    private Player _player;
    
    private void Awake()
    {
        _powerupUI.alpha = 0;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _player = GetComponent<Player>();
        _field.SetActive(false);
    }

    public void ApplyPower(Powerup p)
    {
        _powerupUI.alpha = 1f;
        _currentPower = p;
        _powerupIcon.sprite = p.GetIcon();
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
        if (HasPower) return;
        _currentPower.UsePowerup(this);
        if (!_currentPower.IsInfinite) StartCoroutine(AwaitTimeExpire());
        else RemovePower();
    }
    
    private IEnumerator AwaitTimeExpire()
    {
        float elapsed = 0;
        float timeToExpire = _currentPower.GetExpireTime();
        while (elapsed < timeToExpire)
        {
            elapsed += Time.deltaTime;
            SetRadialTimer(1f - (elapsed / timeToExpire));
            yield return null;
        }
        RemovePower();
    }
    public void SetRadialTimer(float percent)
    {
        _radialTimer.fillAmount = percent;
    }
    
    #region Powerup Actions

    public void Dash(DashStats stats)
    {
        float xDirection = _rigidbody2D.linearVelocity.normalized.x;
        if (xDirection == 0) xDirection =  _player.FacingLeftValue * 0.75f;
        Vector2 direction = new Vector2(xDirection * stats.dashForce * 100f, stats.yForce * 50f);
        _rigidbody2D.AddForce(direction);
    }

    public void Dart(GameObject prefab, DartStats dartStats)
    {
        GameObject o = Instantiate(prefab, transform.position, Quaternion.identity);
        Dart dart = o.GetComponent<Dart>();
        dart.Throw(gameObject, dartStats, _player.FacingLeftValue);
    }

    public void Field(FieldStats stats)
    {
        _field.GetComponent<Field>().StartField(stats.Force);
        _field.SetActive(true);
    }
    public void DisableField()
    {
        _field.SetActive(false);
    }

    public void Trap(GameObject prefab, TrapStats trapStats)
    {
        GameObject o = Instantiate(prefab, transform.position, Quaternion.identity);
        Trap trap = o.GetComponent<Trap>();
        trap.Throw(gameObject, trapStats, _player.FacingLeftValue);
    }

    public void SpawnExplosion(GameObject explosion, Transform pos)
    {
        Debug.Log("spawn");
        Instantiate(explosion, pos.position, Quaternion.identity);
    }
    #endregion
}
