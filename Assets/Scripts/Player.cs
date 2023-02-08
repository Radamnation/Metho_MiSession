using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

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

    [SerializeField] private float m_movementSpeed = 4f;
    [SerializeField] private float m_attackSpeed = 0.5f;

    [SerializeField] private List<PlayerAttack> m_attackList;

    [SerializeField] private int m_maxHealth = 10;
    private int m_currentHealth;

    private float m_attackTimer;

    private int m_currentLevel = 1;
    private int m_currentExperience = 0;
    private int m_nextLevel = 8;

    public int CurrentLevel { get => m_currentLevel; }
    public int CurrentExperience { get => m_currentExperience; }
    public int NextLevel { get => m_nextLevel; }

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_currentHealth = m_maxHealth;
        m_attackTimer = m_attackSpeed;
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
            transform.Translate(new Vector3(0, verticalMovement, 0) * m_movementSpeed * Time.deltaTime, Space.World);
            m_animator.SetFloat("MoveY", verticalMovement);
            m_animator.SetFloat("MoveX", 0);
        }

        if (horizontalMovement != 0)
        {
            transform.Translate(new Vector3(horizontalMovement, 0, 0) * m_movementSpeed * Time.deltaTime, Space.World);
            transform.localScale = new Vector3(-horizontalMovement, 1, 1);
            m_animator.SetFloat("MoveX", horizontalMovement);
            m_animator.SetFloat("MoveY", 0);
        }

        m_animator.SetFloat("Velocity", new Vector3(horizontalMovement, verticalMovement, 0).magnitude);
    }

    private void Attack()
    {
        m_attackTimer -= Time.deltaTime;
        if (m_attackTimer <= 0 && GameManager.Instance.EnemyList.Count > 0)
        {
            m_attackList.ForEach(attack => attack.Execute());
            m_attackTimer = m_attackSpeed;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var collidable = collision.GetComponent<ICollidable>();
        if (collidable != null)
        {
            collidable.Collide(this);
        }
    }
}
