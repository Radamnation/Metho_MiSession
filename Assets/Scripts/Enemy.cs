using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : PoolableObject, ICollidable, IDamagable
{
    [SerializeField] private float movementSpeed = 1.5f;
    [SerializeField] private float damage = 1;
    [SerializeField] private List<AudioClip> hitSFXList;
    [SerializeField] private float stuntTime;
    private float m_hitvalue;
    private float m_currentHealth;

    private bool m_canMove = true;
    
    private SpriteRenderer m_spriteRenderer;
    private Rigidbody2D m_rigidbody2D;
    private CircleCollider2D m_collider;
    private Animator m_animator;
    private EnemyShield m_shield;

    private Vector3 m_currentScale;
    private bool m_isVisible;

    private int m_goldValue;

    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int IsDead = Animator.StringToHash("IsDead");
    private static readonly int HitValue = Shader.PropertyToID("_HitValue");

    public Animator Animator => m_animator;
    public CircleCollider2D Collider => m_collider;
    public int GoldValue => m_goldValue;

    public float MaxHealth { get; set; }
    public float ShieldHealth { get; set; }

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_collider = GetComponentInChildren<CircleCollider2D>();
        m_animator = GetComponent<Animator>();
        m_shield = GetComponentInChildren<EnemyShield>();

        m_goldValue = Random.Range(10, 101);
    }

    public override void Initialize()
    {
        GameManager.Instance.EnemyList.Add(this);
        m_shield.Initialize(ShieldHealth);
        m_currentHealth = MaxHealth;
        m_currentScale = transform.localScale;
        m_canMove = true;
        m_animator.speed = 1;
        m_collider.enabled = true;
        m_spriteRenderer.color = Color.white;
        m_animator.SetBool(IsDead, false);
    }

    private void Update()
    {
        Move();
        m_hitvalue = Mathf.Lerp(m_hitvalue, 0, Time.deltaTime * 5f);
        m_spriteRenderer.material.SetFloat(HitValue, m_hitvalue);
    }

    private void Move()
    {
        if (!m_canMove) return;

        var playerDirection = (Player.Instance.transform.position - transform.position).normalized;
        m_rigidbody2D.velocity = playerDirection * movementSpeed;

        if (Mathf.Abs(playerDirection.y) > 0.7f)
        {
            m_animator.SetFloat(MoveX, 0);
            m_animator.SetFloat(MoveY, playerDirection.y);
        }

        if (Mathf.Abs(playerDirection.x) > 0.7f)
        {
            transform.localScale = new Vector3(-Mathf.Sign(playerDirection.x) * m_currentScale.x, m_currentScale.y,  m_currentScale.z);
            m_animator.SetFloat(MoveX, playerDirection.x);
            m_animator.SetFloat(MoveY, 0);
        }
    }

    public void TakeDamage(float _damage)
    {
        m_hitvalue = 1;
        AudioManager.Instance.SfxAudioSource.PlayOneShot(hitSFXList[Random.Range(0, hitSFXList.Count)]);
        m_currentHealth -= _damage;
        if (m_currentHealth <= 0)
        {
            Death();
        }
    }

    public void AddForce(Vector3 _impact)
    {
        var direction = (transform.position - _impact).normalized;
        StartCoroutine(Stunt(direction));
    }
    
    private IEnumerator Stunt(Vector3 _direction)
    {
        m_canMove = false;
        m_rigidbody2D.velocity = Vector2.zero;
        m_rigidbody2D.AddForce(_direction * 5f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(stuntTime);
        m_canMove = true;
    }

    public void Death()
    {
        m_animator.SetBool(IsDead, true);
        m_canMove = false;
        m_animator.speed = 0;
        m_collider.enabled = false;
        m_spriteRenderer.color = Color.grey;
        PoolManager.Instance.GetPickup(transform.position, Quaternion.identity);
        StartCoroutine(RepoolAfterDelay());
    }

    public IEnumerator RepoolAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Repool();
    }

    public void Collide(Player _player)
    {
        _player.TakeDamage(damage);
    }

    private void OnTriggerExit2D(Collider2D _other)
    {
        if (!m_isVisible)
        {
            Repool();
        }
    }

    protected override void Repool()
    {
        GameManager.Instance.EnemyList.Remove(this);
        base.Repool();
    }

    private void OnBecameVisible()
    {
        m_isVisible = true;
        GameManager.Instance.EnemyOnScreenList.Add(this);
    }

    private void OnBecameInvisible()
    {
        m_isVisible = false;
        GameManager.Instance.EnemyOnScreenList.Remove(this);
    }
}
