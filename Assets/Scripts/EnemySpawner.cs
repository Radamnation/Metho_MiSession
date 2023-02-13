using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy m_enemy;
    [SerializeField] private float m_height;
    [SerializeField] private float m_spawnRate = 0.1f;

    private float timer;

    private void Start()
    {
        timer = m_spawnRate;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 && GameManager.Instance.CanSpawnEnemy())
        {
            var randomHeight = Random.Range(-m_height, m_height);
            EnemyFactory.Instance.SpawnRandomEnemy(transform.position + new Vector3(0, randomHeight, 0), Quaternion.identity);
            timer = m_spawnRate;
        }
    }
}
