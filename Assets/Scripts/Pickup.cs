using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : PoolableObject, ICollidable
{
    // [SerializeField] private int m_experienceValue;
    // [SerializeField] private int m_healthValue;
    // [SerializeField] private int m_goldValue;

    [SerializeField] private AudioClip pickupSFX;

    private CircleCollider2D m_circleCollider2D;
    private Animator m_animator;

    public Animator Animator => m_animator;
    
    public AudioClip PickupSFX { get; set; }
    public int ExperienceValue { get; set; }
    public int HealthValue { get; set; }
    public int GoldValue { get; set; }

    private void Awake()
    {
        m_circleCollider2D = GetComponent<CircleCollider2D>();
        m_animator = GetComponent<Animator>();
    }

    public void Collide(Player _player)
    {
        if (ExperienceValue > 0)
        {
            if (SaveManager.Instance.SaveFile.doubleXPUnlocked)
            {
                ExperienceValue *= 2;
            }
            _player.IncreaseExperience(ExperienceValue);
        }
        if (HealthValue > 0)
        {
            _player.IncreaseHealth(HealthValue);
        }
        if (GoldValue > 0)
        {
            SaveManager.Instance.SaveFile.IncreaseGold(GoldValue);
        }

        AudioManager.Instance.SfxAudioSource.PlayOneShot(pickupSFX);
        m_circleCollider2D.enabled = false;
        Repool();
    }

    public override void Initialize()
    {
        base.Initialize();
        
        m_circleCollider2D.enabled = true;
    }
}
