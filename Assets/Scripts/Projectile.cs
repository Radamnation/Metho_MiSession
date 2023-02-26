using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class Projectile : PoolableObject
{
    [SerializeField] private float speed = 6f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float orbitDistance = 3f;
    [SerializeField] private Transform orbitFocus;

    private PositionConstraint m_positionConstraint;
    private Rigidbody2D m_rigidbody2D;
    
    public float LifeTime { get; set; }
    public bool IsOrbital { get; set; }
    public CircleCollider2D Collider { get; private set; }
    public Animator Animator { get; private set; }

    private bool m_isSpawned;
    private float lifeTimer;

    private void Awake()
    {
        m_positionConstraint = GetComponentInChildren<PositionConstraint>();
        m_rigidbody2D = GetComponentInChildren<Rigidbody2D>();
        
        Collider = GetComponentInChildren<CircleCollider2D>();
        Animator = GetComponentInChildren<Animator>();
    }

    public override void Initialize()
    {
        m_isSpawned = false;
        Collider.enabled = true;
        lifeTimer = LifeTime;

        if (!IsOrbital)
        {
            m_rigidbody2D.velocity = transform.up * speed;
        }
        else
        {
            m_rigidbody2D.angularVelocity = 2 * Mathf.PI * orbitDistance * speed;
            Collider.transform.localPosition = new Vector2(orbitDistance, 0);
            var constraintSource = new ConstraintSource
            {
                sourceTransform = Player.Instance.transform,
                weight = 1f
            };
            m_positionConstraint.AddSource(constraintSource);
            m_positionConstraint.constraintActive = true;
        }
    }

    private void FixedUpdate()
    {
        if (LifeTime == 0 || !m_isSpawned) return;
        
        lifeTimer -= Time.fixedDeltaTime;
        if (lifeTimer <= 0)
        {
            Repool();
        }
    }

    protected override void Repool()
    {
        m_isSpawned = false;
        Collider.enabled = false;

        m_rigidbody2D.velocity = Vector3.zero;
        m_rigidbody2D.transform.localPosition = Vector3.zero;
        
        m_rigidbody2D.angularVelocity = 0;
        m_rigidbody2D.transform.localRotation = Quaternion.identity;
        Collider.transform.localPosition = Vector3.zero;
        m_positionConstraint.constraintActive = false;
        
        base.Repool();
    }

    public void Hit(Collider2D _collision)
    {
        if (!m_isSpawned)
        {
            m_isSpawned = true;
            return;
        }

        var enemyShield = _collision.GetComponent<EnemyShield>();
        if (enemyShield != null)
        {
            enemyShield.TakeDamage(damage);
            enemyShield.AddForce(transform.position);
        }

        if (!IsOrbital)
        {
            Repool();
        }
    }
}
