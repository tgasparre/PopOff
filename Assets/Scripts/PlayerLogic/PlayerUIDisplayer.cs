using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIDisplayer : MonoBehaviour
{
    public HorizontalLayoutGroup healthBarContainer;
    
    [SerializeField] private GameObject playerDisplayPrefab;
    
    public Slider slider;
    public Image fillImage;
    public Gradient gradient;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider.value = 0;

        healthBarContainer = gameObject.GetComponentInParent<HorizontalLayoutGroup>();
    }
    
    public void SubscribeToTracker(UltimateAttackTracker tracker)
    {
        tracker.OnUltimateAttackUnlocked += OnUltimateAttackUnlocked;
    }
    
    public void UpdateUI(float fillAmount)
    {
        slider.value = fillAmount;
    }

    public void InitializePlayerUI(AttackHurtbox hurtbox)
    {
        gameObject.GetComponentInChildren<HPDisplayer>().Initialize(hurtbox);
    }

    public void DeletePlayerUI()
    {
        Destroy(gameObject);
    }

    private void OnUltimateAttackUnlocked()
    {
        if (fillImage != null)
        { 
            fillImage.color = Color.gold;
        }
    }
    
}
