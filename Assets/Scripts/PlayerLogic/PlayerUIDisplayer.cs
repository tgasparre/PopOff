using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _health;

    private AttackHurtbox _hurtbox;
    
    
    public void UpdateHealth(string health)
    {
        _health.text = $"<color=red>{health}</color>";
    }

    private void Update()
    {
        if (_hurtbox != null)
        {
            UpdateHealth(Mathf.RoundToInt(_hurtbox.HP).ToString());
        }
    }

    public void InitializePlayerUI(int index, AttackHurtbox hurtbox)
    {
        _name.text = index switch
        {
            0 => "Player One",
            1 => "Player Two",
            2 => "Player Three",
            3 => "Player Four",
            _ => _name.text
        };
        UpdateHealth("200");
        _hurtbox = hurtbox;
    }

    public void UpdateUltimateAttackUI(float fillAmount)
    {
        //TODO
    }
}

// public HorizontalLayoutGroup healthBarContainer;
//     
// [SerializeField] private GameObject playerDisplayPrefab;
//     
// public Slider slider;
// public Image fillImage;
// public Gradient gradient;
//     
// // Start is called once before the first execution of Update after the MonoBehaviour is created
// void Start()
// {
//     slider.value = 0;
//
//     healthBarContainer = gameObject.GetComponentInParent<HorizontalLayoutGroup>();
// }
//     
// public void SubscribeToTracker(UltimateAttackTracker tracker)
// {
//     tracker.OnUltimateAttackUnlocked += OnUltimateAttackUnlocked;
// }
//     
// public void UpdateUI(float fillAmount)
// {
//     slider.value = fillAmount;
// }
//
// public void InitializePlayerUI(AttackHurtbox hurtbox)
// {
//     gameObject.GetComponentInChildren<HPDisplayer>().Initialize(hurtbox);
// }
//
// public void DeletePlayerUI()
// {
//     Destroy(gameObject);
// }
//
// private void OnUltimateAttackUnlocked()
// {
//     if (fillImage != null)
//     { 
//         fillImage.color = Color.gold;
//     }
// }
