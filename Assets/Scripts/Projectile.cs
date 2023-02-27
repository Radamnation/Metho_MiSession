using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class Projectile : PoolableObject
{
    [SerializeField] private float damage = 5f;
    [SerializeField] private Transform orbitFocus;
    
    private PositionConstraint m_positionConstraint;
    private SpriteRenderer m_spriteRenderer;
    private Rigidbody2D m_rigidbody2D;
    
    public AudioClip SpawnSFX { get; set; }
    public bool Rotate { get; set; }
    public float RotationSpeed { get; set; }
    public float OrbitDistance { get; set; }
    public float Speed { get; set; }
    public float LifeTime { get; set; }
    public bool IsOrbital { get; set; }
    public bool UseBoxCollider { get; set; }
    public bool HitOnSpawn { get; set; }
    public CircleCollider2D CircleCollider2D { get; private set; }
    public BoxCollider2D BoxCollider2D { get; private set; }
    public Animator Animator { get; private set; }

    private bool m_isSpawned;
    private float lifeTimer;
    private Collider2D collider;

    private void Awake()
    {
        m_positionConstraint = GetComponentInChildren<PositionConstraint>();
        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_rigidbody2D = GetComponentInChildren<Rigidbody2D>();
        
        CircleCollider2D = GetComponentInChildren<CircleCollider2D>();
        BoxCollider2D = GetComponentInChildren<BoxCollider2D>();
        Animator = GetComponentInChildren<Animator>();

        CircleCollider2D.enabled = false;
        BoxCollider2D.enabled = false;
    }

    public override void Initialize()
    {
        m_spriteRenderer.sprite = null;
        m_isSpawned = false;
        if (!UseBoxCollider)
        {
            collider = CircleCollider2D;
        }
        else
        {
            collider = BoxCollider2D;
        }
        collider.enabled = true;
        lifeTimer = LifeTime;

        if (!IsOrbital)
        {
            m_rigidbody2D.velocity = transform.up * Speed;
        }
        else
        {
            m_rigidbody2D.angularVelocity = 2 * Mathf.PI * OrbitDistance * Speed;
            collider.transform.localPosition = new Vector2(OrbitDistance, 0);
            var constraintSource = new ConstraintSource
            {
                sourceTransform = Player.Instance.transform,
                weight = 1f
            };
            m_positionConstraint.AddSource(constraintSource);
            m_positionConstraint.constraintActive = true;
        }
        
        if (HitOnSpawn)
        {
            StartCoroutine(HitMultipleEnemies());
        }
    }

    private void FixedUpdate()
    {
        if (LifeTime == 0 || !m_isSpawned) return;

        if (Rotate)
        {
            Animator.transform.Rotate(new Vector3(0, 0, RotationSpeed));
        }
        
        lifeTimer -= Time.fixedDeltaTime;
        if (lifeTimer <= 0)
        {
            Repool();
        }
    }

    protected override void Repool()
    {
        m_isSpawned = false;
        collider.enabled = false;

        m_rigidbody2D.velocity = Vector3.zero;
        m_rigidbody2D.transform.localPosition = Vector3.zero;
        
        m_rigidbody2D.angularVelocity = 0;
        m_rigidbody2D.transform.localRotation = Quaternion.identity;
        collider.transform.localPosition = Vector3.zero;
        m_positionConstraint.constraintActive = false;
        Animator.transform.localRotation = Quaternion.identity;
        
        base.Repool();
    }

    public void Hit(Collider2D _collision)
    {
        if (!m_isSpawned)
        {
            m_isSpawned = true;
            return;
        }
        
        if (HitOnSpawn) return;

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

    private IEnumerator HitMultipleEnemies()
    {
        yield return new WaitForEndOfFrame();
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        filter.useTriggers = true;

        List<Collider2D> results = new ();
        collider.OverlapCollider(filter, results);
        foreach (var collider in results)
        {
            var enemyShield = collider.GetComponent<EnemyShield>();
            if (enemyShield != null)
            {
                enemyShield.TakeDamage(damage);
                enemyShield.AddForce(transform.position);
            }
        }
    }
}
