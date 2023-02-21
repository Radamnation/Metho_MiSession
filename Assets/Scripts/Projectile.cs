using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : PoolableObject
{
    [FormerlySerializedAs("m_speed")] [SerializeField] private float speed = 6f;

    private CircleCollider2D m_circleCollider2D;

    private void Awake()
    {
        m_circleCollider2D = GetComponent<CircleCollider2D>();
    }

    public override void Initialize()
    {
        m_circleCollider2D.enabled = true;
    }

    private void Update()
    {
        transform.Translate(transform.up * (speed * Time.deltaTime), Space.World);
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        var enemy = _collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage();
            enemy.AddForce(transform.position);
            m_circleCollider2D.enabled = false;
            Repool();
        }
    }
}
