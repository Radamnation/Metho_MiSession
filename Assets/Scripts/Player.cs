using System.Collections.Generic;
using TNRD;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public static Player Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }

    private Animator m_animator;
    private Transform m_transform;

    [FormerlySerializedAs("m_movementSpeed")] [SerializeField] private float movementSpeed = 4f;
    [FormerlySerializedAs("m_attackSpeed")] [SerializeField] private float attackSpeed = 0.5f;
    
    [FormerlySerializedAs("m_attackList")] [SerializeField] private List<SerializableInterface<IAttack>> attackList;
    
    [FormerlySerializedAs("m_maxHealth")] [SerializeField] private int maxHealth = 10;
    private int m_currentHealth;

    private float m_attackTimer;

    private int m_currentLevel = 1;
    private int m_currentExperience = 0;
    private int m_nextLevel = 8;
    
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int Velocity = Animator.StringToHash("Velocity");

    public int CurrentLevel { get => m_currentLevel; }
    public int CurrentExperience { get => m_currentExperience; }
    public int NextLevel { get => m_nextLevel; }

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_transform = GetComponent<Transform>();
        
        m_currentHealth = maxHealth;
        m_attackTimer = attackSpeed;
        
        UIManager.Instance.UpdateExperience();
    }

    private void Update()
    {
        Pause();
        Move();
        Attack();
    }

    private void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.PauseGame();
        }
    }

    private void Move()
    {
        var verticalMovement = Input.GetAxisRaw("Vertical");
        var horizontalMovement = Input.GetAxisRaw("Horizontal");
        
        if (verticalMovement != 0)
        {
            m_transform.Translate(new Vector2(0, verticalMovement) * (movementSpeed * Time.deltaTime), Space.World);
            m_animator.SetFloat(MoveX, 0);
            m_animator.SetFloat(MoveY, verticalMovement);
        }

        if (horizontalMovement != 0)
        {
            m_transform.Translate(new Vector2(horizontalMovement, 0) * (movementSpeed * Time.deltaTime), Space.World);
            m_transform.localScale = new Vector3(-horizontalMovement, 1, 1);
            m_animator.SetFloat(MoveX, horizontalMovement);
            m_animator.SetFloat(MoveY, 0);
        }

        m_animator.SetFloat(Velocity, new Vector2(horizontalMovement, verticalMovement).magnitude);
    }

    private void Attack()
    {
        m_attackTimer -= Time.deltaTime;
        if (m_attackTimer <= 0 && GameManager.Instance.EnemyList.Count > 0)
        {
            attackList.ForEach(_attack => _attack.Value.Execute());
            m_attackTimer = attackSpeed;
        }
    }

    public void TakeDamage(int _damage)
    {
        m_currentHealth -= _damage;
        if (m_currentHealth <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        Destroy(gameObject);
    }

    public void IncreaseExperience(int _experience)
    {
        m_currentExperience += _experience;
        if (m_currentExperience >= m_nextLevel)
        {
            LevelUp();
        }
        UIManager.Instance.UpdateExperience();
    }

    private void LevelUp()
    {
        m_currentLevel++;
        m_currentExperience = 0;
        m_nextLevel = (int) (m_nextLevel * 1.5f);
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        var collidable = _collision.GetComponent<ICollidable>();
        collidable?.Collide(this);
    }
}
