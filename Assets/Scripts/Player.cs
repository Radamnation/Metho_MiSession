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
            m_rigidbody2D = GetComponent<Rigidbody2D>();
            m_collider2D = GetComponent<Collider2D>();
            m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            m_animator = GetComponentInChildren<Animator>();
            m_controller = GetComponent<PlayerController>();
            return;
        }
        Destroy(gameObject);
    }

    private Rigidbody2D m_rigidbody2D;
    private Collider2D m_collider2D;
    private SpriteRenderer m_spriteRenderer;
    private Animator m_animator;
    private Transform m_visualTransform;

    private PlayerController m_controller;
    public PlayerController Controller => m_controller;

    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private List<PlayerAttackSO> attackList;
    [SerializeField] private float maxHealth = 10;
    [SerializeField] private AudioClip levelUpSFX;
    [SerializeField] private PlayerCanvas playerCanvas;
    [SerializeField] private float invulnerabilityTime;
    [SerializeField] private List<AudioClip> hitSFXList;
    [SerializeField] private List<AudioClip> deathSFXList;
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
    public Transform VisualTransform => m_visualTransform;

    private void Start()
    {
        m_visualTransform = m_animator.transform;
        m_currentHealth = maxHealth;

        UIManager.Instance.UpdateExperience();
    }

    private void FixedUpdate()
    {
        Attack();
    }

    public bool Move()
    {
        var moved = false;
        var verticalMovement = Input.GetAxisRaw("Vertical");
        var horizontalMovement = Input.GetAxisRaw("Horizontal");
        
        if (verticalMovement != 0)
        {
            m_rigidbody2D.velocity = new Vector2(m_rigidbody2D.velocity.x, verticalMovement * movementSpeed);
            m_animator.SetFloat(MoveX, 0);
            m_animator.SetFloat(MoveY, verticalMovement);
            moved = true;
        }
        else
        {
            m_rigidbody2D.velocity = new Vector2(m_rigidbody2D.velocity.x, 0);
        }

        if (horizontalMovement != 0)
        {
            m_rigidbody2D.velocity = new Vector2(horizontalMovement * movementSpeed , m_rigidbody2D.velocity.y);
            m_visualTransform.localScale = new Vector3(-horizontalMovement, 1, 1);
            m_animator.SetFloat(MoveX, horizontalMovement);
            m_animator.SetFloat(MoveY, 0);
            moved = true;
        }
        else
        {
            m_rigidbody2D.velocity = new Vector2(0, m_rigidbody2D.velocity.y);
        }

        m_animator.SetFloat(Velocity, new Vector2(horizontalMovement, verticalMovement).magnitude);
        return moved;
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
        
        m_currentHealth -= _damage;
        playerCanvas.UpdateHealth(m_currentHealth / maxHealth);
        if (m_currentHealth <= 0)
        {
            Death();
            return;
        }
        AudioManager.Instance.SfxAudioSource.PlayOneShot(hitSFXList[Random.Range(0, hitSFXList.Count)]);
        StartCoroutine(StartInvulnerability());
    }

    private IEnumerator StartInvulnerability()
    {
        canBeDamaged = false;
        m_collider2D.enabled = false;
        var color = m_spriteRenderer.color;
        color.a = 0.25f;
        m_spriteRenderer.color = color;
        yield return new WaitForSeconds(invulnerabilityTime);
        m_collider2D.enabled = true;
        color.a = 1f;
        m_spriteRenderer.color = color;
        canBeDamaged = true;
    }

    private void Death()
    {
        canBeDamaged = false;
        m_rigidbody2D.velocity = Vector2.zero;
        AudioManager.Instance.SfxAudioSource.PlayOneShot(deathSFXList[Random.Range(0, deathSFXList.Count)]);
        m_spriteRenderer.enabled = false;
        playerCanvas.gameObject.SetActive(false);
        StartCoroutine(ShowDeathView());
        enabled = false;
    }

    private IEnumerator ShowDeathView()
    {
        yield return new WaitForSeconds(2f);
        UIManager.Instance.ToggleDeathView();
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
