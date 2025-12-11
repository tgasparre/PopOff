using System;
using UnityEngine;

public class HPDisplayer : MonoBehaviour
{
    public RectTransform hpBar;
    private float fullSize;
    private AttackHurtbox hurtbox;

    private void Awake()
    {
        fullSize = hpBar.sizeDelta.x;
        
    }

    public void Initialize(AttackHurtbox hb)
    {
        hurtbox = hb;
    }

    void Update()
    {
        if (hurtbox == null) return;

        float pct = hurtbox.HP / 200f;

        // Update width
        hpBar.sizeDelta = new Vector2(fullSize * pct, hpBar.sizeDelta.y);
    }
    
}
