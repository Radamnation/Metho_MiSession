using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : PoolableObject, ICollidable
{
    [FormerlySerializedAs("m_speed")] [SerializeField] private float speed = 1.5f;
    [FormerlySerializedAs("m_damage")] [SerializeField] private int damage = 1;

    private bool m_canMove = true;

    private SpriteRenderer m_spriteRenderer;
    private Animator m_animator;
    
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int MoveX = Animator.StringToHash("MoveX");

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_animator = GetComponent<Animator>();
    }

    public override void Initialize()
    {
        m_canMove = true;
        m_animator.speed = 1;
        m_spriteRenderer.color = Color.white;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (!m_canMove) return;

        var playerDirection = (Player.Instance.transform.position - transform.position).normalized;
        transform.Translate(playerDirection * (speed * Time.deltaTime), Space.World);

        if (Mathf.Abs(playerDirection.y) > 0.7f)
        {
            m_animator.SetFloat(MoveX, 0);
            m_animator.SetFloat(MoveY, playerDirection.y);
        }

        if (Mathf.Abs(playerDirection.x) > 0.7f)
        {
            transform.localScale = new Vector3(-Mathf.Sign(playerDirection.x), 1, 1);
            m_animator.SetFloat(MoveX, playerDirection.x);
            m_animator.SetFloat(MoveY, 0);
        }
    }

    public void TakeDamage()
    {
        Death();
    }

    public void Death()
    {
        m_canMove = false;
        m_animator.speed = 0;
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

    private void OnBecameVisible()
    {
        GameManager.Instance.EnemyList.Add(this);
    }

    private void OnBecameInvisible()
    {
        GameManager.Instance.EnemyList.Remove(this);
    }
}
