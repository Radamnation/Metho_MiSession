using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public static Player Instance;

    private void Awake()
    {
        // if (Instance == null)
        // {
            Instance = this;
            m_rigidbody2D = GetComponent<Rigidbody2D>();
            m_collider2D = GetComponent<Collider2D>();
            m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            m_animator = GetComponentInChildren<Animator>();
            m_controller = GetComponent<PlayerController>();
            if (TestManager.Instance.IsTesting)
            {
                Time.timeScale = 10;
            }
            // return;
        // }
        //
        // Destroy(gameObject);
    }

    [SerializeField] private bool IsIntro;
    [SerializeField] private RuntimeAnimatorController character1Controller;
    [SerializeField] private RuntimeAnimatorController character2Controller;

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

    private Pickup pickupTarget;
    private Enemy enemyTarget;

    private int m_currentLevel = 1;
    private int m_currentExperience = 0;
    private int m_nextLevel = 8;

    public float TimeSurvived;

    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int Velocity = Animator.StringToHash("Velocity");

    public int CurrentLevel
    {
        get => m_currentLevel;
    }

    public int CurrentExperience
    {
        get => m_currentExperience;
    }

    public int NextLevel
    {
        get => m_nextLevel;
    }

    public Transform VisualTransform => m_visualTransform;

    private void Start()
    {
        if (GameManager.Instance.CharacterSelected > 0)
        {
            m_animator.runtimeAnimatorController = character2Controller;
        }
        else
        {
            m_animator.runtimeAnimatorController = character1Controller;
        }
        
        m_visualTransform = m_animator.transform;
        m_currentHealth = maxHealth;

        UIManager.Instance.UpdateExperience();

        if (attackList.Count > 0)
        {
            foreach (var attack in attackList)
            {
                attack.Initialize();
            }
            attackList[Random.Range(0, attackList.Count)].LevelUp();
        }
        
        m_currentLevel = 1;
        UIManager.Instance.MainView.UpdateLevel();
    }

    private void Update()
    {
        if (IsIntro)
        {
            Move();
            return;
        }
        
        TimeSurvived += Time.deltaTime;
        if (TimeSurvived >= 1800 && !GameManager.Instance.IsPaused())
        {
            TimeSurvived = 1800;
            if (TestManager.Instance.IsTesting)
            {
                UIManager.Instance.DeathView.ChangeText("Test Complete!");
            }
            else
            {
                UIManager.Instance.DeathView.ChangeText("You Win!");
            }
            GameManager.Instance.PauseGame();
            UIManager.Instance.ToggleDeathView();
            UIManager.Instance.MainView.UpdateTimer();
            UIManager.Instance.GoToEndScreen();
            enabled = false;
            return;
        }
        UIManager.Instance.MainView.UpdateTimer();
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

        if (TestManager.Instance.IsTesting)
        {
            if (pickupTarget == null || !Pickup.ActivePickupList.ContainsValue(pickupTarget))
            {
                if (Pickup.ActivePickupList.Count > 0)
                {
                    pickupTarget = Pickup.ActivePickupList.ElementAt(Random.Range(0, Pickup.ActivePickupList.Count))
                        .Value;
                }
            }

            if (enemyTarget == null || !Enemy.EnemyOnScreenList.ContainsValue(enemyTarget))
            {
                if (Enemy.EnemyOnScreenList.Count > 0)
                {
                    enemyTarget = Enemy.EnemyOnScreenList.ElementAt(Random.Range(0, Enemy.EnemyOnScreenList.Count))
                        .Value;
                }
            }

            if (pickupTarget != null && Pickup.ActivePickupList.ContainsValue(pickupTarget))
            {
                if (Vector3.Distance(transform.position, pickupTarget.transform.position) > 0)
                {
                    verticalMovement = Mathf.Sign(pickupTarget.transform.position.y - transform.position.y);
                    horizontalMovement = Mathf.Sign(pickupTarget.transform.position.x - transform.position.x);
                }
            }
            else if (enemyTarget != null && Enemy.EnemyOnScreenList.ContainsValue(enemyTarget))
            {
                if (Vector3.Distance(transform.position, enemyTarget.transform.position) > 2)
                {
                    verticalMovement = Mathf.Sign(enemyTarget.transform.position.y - transform.position.y);
                    horizontalMovement = Mathf.Sign(enemyTarget.transform.position.x - transform.position.x);
                }
            }
        }

        if (IsIntro)
        {
            horizontalMovement = 1;
        }

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
            m_rigidbody2D.velocity = new Vector2(horizontalMovement * movementSpeed, m_rigidbody2D.velocity.y);
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
        if (TestManager.Instance.IsTesting) return;
        if (!canBeDamaged)
        {
            return;
        }

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

    public void IncreaseHealth(int _health)
    {
        m_currentHealth += _health;
        if (m_currentHealth >= maxHealth)
        {
            m_currentHealth = maxHealth;
        }

        playerCanvas.UpdateHealth(m_currentHealth / maxHealth);
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
        UIManager.Instance.DeathView.ChangeText("You Died...");
        StartCoroutine(ShowDeathView());
        enabled = false;
    }

    private IEnumerator ShowDeathView()
    {
        yield return new WaitForSeconds(0f);
        UIManager.Instance.ToggleDeathView();
        UIManager.Instance.GoToEndScreen();
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
        m_nextLevel = (int)(m_nextLevel * 1.2f);
        UIManager.Instance.MainView.UpdateLevel();
        AudioManager.Instance.SfxAudioSource.PlayOneShot(levelUpSFX);
        UIManager.Instance.ToggleLevelUpView();
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        var collidable = _collision.GetComponent<ICollidable>();
        collidable?.Collide(this);
    }
}