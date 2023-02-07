using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float m_speed = 2f;
    [SerializeField] private ParticleSystem m_particle;

    private void Update()
    {
        var playerDirection = Player.Instance.transform.position - transform.position;
        transform.Translate(playerDirection.normalized * m_speed * Time.deltaTime, Space.World);
    }

    public void Kill()
    {
        Instantiate(m_particle, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        GameManager.Instance.EnemiesOnScreen++;
    }

    private void OnDisable()
    {
        GameManager.Instance.EnemiesOnScreen--;
    }
}
