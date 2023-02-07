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

    [SerializeField] private float m_speed = 8f;

    private Animator m_animator;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
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
            transform.Translate(new Vector3(0, verticalMovement, 0) * m_speed * Time.deltaTime, Space.World);
            m_animator.SetFloat("MoveY", verticalMovement);
            m_animator.SetFloat("MoveX", 0);
        }

        if (horizontalMovement != 0)
        {
            transform.Translate(new Vector3(horizontalMovement, 0, 0) * m_speed * Time.deltaTime, Space.World);
            transform.localScale = new Vector3(-horizontalMovement, 1, 1);
            m_animator.SetFloat("MoveX", horizontalMovement);
            m_animator.SetFloat("MoveY", 0);
        }

        m_animator.SetFloat("Velocity", new Vector3(horizontalMovement, verticalMovement, 0).magnitude);
    }

    private void Attack()
    {

    }
}
