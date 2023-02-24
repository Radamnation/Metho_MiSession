using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : PoolableObject
{
    [SerializeField] private float speed = 6f;
    
    private CircleCollider2D m_circleCollider2D;
    private Rigidbody2D m_rigidbody2D;

    private bool m_spawned;

    private void Awake()
    {
        m_circleCollider2D = GetComponent<CircleCollider2D>();
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public override void Initialize()
    {
        m_spawned = false;
        m_circleCollider2D.enabled = true;
        m_rigidbody2D.velocity = transform.up * speed;
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (!m_spawned)
        {
            m_spawned = true;
            return;
        }
        
        Debug.Log(_collision.gameObject);
        
        var enemy = _collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage();
            enemy.AddForce(transform.position);
        }
        m_circleCollider2D.enabled = false;
        m_spawned = false;
        Repool();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Repool();
    }
}
