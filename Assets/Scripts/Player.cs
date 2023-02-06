using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float m_playerSpeed = 8f;

    private Rigidbody2D m_rigidbody;
    private Animator m_animator;

    private void Start()
    {
        m_rigidbody= GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
        Attack();
    }

    private void Move()
    {
        var verticalMovement = Input.GetAxisRaw("Vertical");
        var horizontalMovement = Input.GetAxisRaw("Horizontal");
        
        if (verticalMovement != 0)
        {
            transform.Translate(new Vector3(0, verticalMovement, 0) * m_playerSpeed * Time.deltaTime, Space.World);
            m_animator.SetFloat("MoveY", verticalMovement);
            m_animator.SetFloat("MoveX", 0);
        }

        if (horizontalMovement != 0)
        {
            transform.Translate(new Vector3(horizontalMovement, 0, 0) * m_playerSpeed * Time.deltaTime, Space.World);
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
