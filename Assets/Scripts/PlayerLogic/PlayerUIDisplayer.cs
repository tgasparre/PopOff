using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _health;
    
    [Space]
    [SerializeField] private Slider _ultimateAttackSlider;
    [SerializeField] private Image _sliderImage;
    [SerializeField] private Gradient _ultimateGradient;
    
    private Player _player;

    public void InitializePlayerUI(Player player)
    {
        _sliderImage.enabled = false;
        
        _player = player;
        _name.text = GameUtils.PlayerNames[_player.PlayerIndex];
        UpdateHealth(player.PlayerHealth);
        UpdateUltimateAttack(0f, false);
        Register();
    }
    
    private void Register()
    {
        _player.UICallback_PlayerHealthChange += UpdateHealth;
        _player.ultimateAttackTracker.UICallback_OnUltimateAttackChange += UpdateUltimateAttack;
    }

    private void OnDestroy()
    {
        if (_player == null) return;
        _player.UICallback_PlayerHealthChange -= UpdateHealth;
        _player.ultimateAttackTracker.UICallback_OnUltimateAttackChange -= UpdateUltimateAttack;
    }

    public void UpdateHealth(float health)
    {
        health = Mathf.Max(0f, health);
        _health.text = health.ToString();
    }

    public void UpdateUltimateAttack(float value, bool isActive)
    {
        _sliderImage.enabled = value > 0;
        _sliderImage.color = _ultimateGradient.Evaluate(value);
        _ultimateAttackSlider.value = value;
    }
}
