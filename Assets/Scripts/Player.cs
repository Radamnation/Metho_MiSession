using System.Collections;
using System.Collections.Generic;
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

    private Rigidbody2D m_rigidbody2D;
    private SpriteRenderer m_spriteRenderer;
    private Animator m_animator;
    private Transform m_transform;

    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private List<PlayerAttackSO> attackList;
    [SerializeField] private float maxHealth = 10;
    [SerializeField] private AudioClip levelUpSFX;
    [SerializeField] private PlayerCanvas playerCanvas;
    [SerializeField] private float invulnerabilityTime;
    [SerializeField] private List<AudioClip> hitSFXList;
    private float m_currentHealth;
    
    private bool canBeDamaged = true;

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
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_animator = GetComponentInChildren<Animator>();
        m_transform = m_animator.transform;

        m_currentHealth = maxHealth;

        UIManager.Instance.UpdateExperience();
    }

    private void Update()
    {
        Pause();
        Move();
    }

    private void FixedUpdate()
    {
        Attack();
    }

    private void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.TogglePauseView();
        }
    }

    private void Move()
    {
        var verticalMovement = Input.GetAxisRaw("Vertical");
        var horizontalMovement = Input.GetAxisRaw("Horizontal");
        
        if (verticalMovement != 0)
        {
            m_rigidbody2D.velocity = new Vector2(m_rigidbody2D.velocity.x, verticalMovement * movementSpeed);
            m_animator.SetFloat(MoveX, 0);
            m_animator.SetFloat(MoveY, verticalMovement);
        }
        else
        {
            m_rigidbody2D.velocity = new Vector2(m_rigidbody2D.velocity.x, 0);
        }

        if (horizontalMovement != 0)
        {
            m_rigidbody2D.velocity = new Vector2(horizontalMovement * movementSpeed , m_rigidbody2D.velocity.y);
            m_transform.localScale = new Vector3(-horizontalMovement, 1, 1);
            m_animator.SetFloat(MoveX, horizontalMovement);
            m_animator.SetFloat(MoveY, 0);
        }
        else
        {
            m_rigidbody2D.velocity = new Vector2(0, m_rigidbody2D.velocity.y);
        }

        m_animator.SetFloat(Velocity, new Vector2(horizontalMovement, verticalMovement).magnitude);
    }

    private void Attack()
    {
        foreach (var attack in attackList)
        {
            attack.Refresh(Time.fixedDeltaTime);
        }
    }

    public void TakeDamage(float _damage)
    {
        if (!canBeDamaged) { return; }
        
        AudioManager.Instance.SfxAudioSource.PlayOneShot(hitSFXList[Random.Range(0, hitSFXList.Count)]);
        m_currentHealth -= _damage;
        if (m_currentHealth <= 0)
        {
            Death();
        }
        playerCanvas.UpdateHealth(m_currentHealth / maxHealth);
        StartCoroutine(StartInvulnerability());
    }

    private IEnumerator StartInvulnerability()
    {
        canBeDamaged = false;
        var color = m_spriteRenderer.color;
        color.a = 0.25f;
        m_spriteRenderer.color = color;
        yield return new WaitForSeconds(invulnerabilityTime);
        canBeDamaged = true;
        color.a = 1f;
        m_spriteRenderer.color = color;
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
        m_nextLevel = (int) (m_nextLevel * 1.2f);
        AudioManager.Instance.SfxAudioSource.PlayOneShot(levelUpSFX);
        UIManager.Instance.ToggleLevelUpView();
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        var collidable = _collision.GetComponent<ICollidable>();
        collidable?.Collide(this);
    }
}
