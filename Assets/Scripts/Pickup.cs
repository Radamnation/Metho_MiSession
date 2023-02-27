using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : PoolableObject, ICollidable
{
    [SerializeField] private int m_experienceValue;
    [SerializeField] private int m_healthValue;
    [SerializeField] private int m_goldValue;

    [SerializeField] private AudioClip pickupSFX;

    private CircleCollider2D m_circleCollider2D;

    private void Awake()
    {
        m_circleCollider2D = GetComponent<CircleCollider2D>();
    }

    public void Collide(Player _player)
    {
        _player.IncreaseExperience(m_experienceValue);
        AudioManager.Instance.SfxAudioSource.PlayOneShot(pickupSFX);
        m_circleCollider2D.enabled = false;
        Repool();
    }

    public override void Initialize()
    {
        m_circleCollider2D.enabled = true;
    }
}
