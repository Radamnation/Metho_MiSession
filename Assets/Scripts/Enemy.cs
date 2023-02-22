using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Enemy : PoolableObject, ICollidable
{
    [FormerlySerializedAs("speed")] [SerializeField] private float movementSpeed = 1.5f;
    [SerializeField] private int damage = 1;

    private bool m_canMove = true;
    
    private SpriteRenderer m_spriteRenderer;
    private Rigidbody2D m_rigidbody2D;
    private CircleCollider2D m_collider;
    private Animator m_animator;

    private Vector3 m_currentScale;

    private int m_goldValue;

    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int IsDead = Animator.StringToHash("IsDead");

    public Animator Animator
    {
        get => m_animator;
        set => m_animator = value;
    }

    public CircleCollider2D Collider
    {
        get => m_collider;
        set => m_collider = value;
    }

    public int GoldValue
    {
        get => m_goldValue;
    }

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<CircleCollider2D>();
        m_animator = GetComponent<Animator>();

        m_goldValue = Random.Range(10, 101);
    }

    public override void Initialize()
    {
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
    }

    private void Move()
    {
        if (!m_canMove) return;

        var playerDirection = (Player.Instance.transform.position - transform.position).normalized;
        m_rigidbody2D.velocity = playerDirection * movementSpeed;
        // transform.Translate(playerDirection * (speed * Time.deltaTime), Space.World);

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

    public void TakeDamage()
    {
        Death();
    }

    public void AddForce(Vector3 _impact)
    {
        var direction = transform.position - _impact;
        m_rigidbody2D.AddForce(direction * 1f, ForceMode2D.Impulse);
    }

    public void Death()
    {
        m_animator.SetBool(IsDead, true);
        m_canMove = false;
        m_animator.speed = 0;
        m_collider.enabled = false;
        m_spriteRenderer.color = Color.grey;
        StartCoroutine(RepoolAfterDelay());
    }

    public IEnumerator RepoolAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        PoolManager.Instance.GetPickup(transform.position, Quaternion.identity);
        Repool();
    }

    public void Collide(Player _player)
    {
        _player.TakeDamage(damage);
    }

    private void OnTriggerExit2D(Collider2D _other)
    {
        if (_other.gameObject.layer == LayerMask.NameToLayer("Confiner"))
        {
            Repool();
        }
    }

    private void OnBecameVisible()
    {
        GameManager.Instance.EnemyList.Add(this);
        EnemyChecker.Instance.SortedEnemy.Add(this);
    }

    private void OnBecameInvisible()
    {
        GameManager.Instance.EnemyList.Remove(this);
        EnemyChecker.Instance.SortedEnemy.Remove(this);
    }
}
