using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float height;
    [SerializeField] private float spawnRate = 0.1f;

    private float m_timer;

    private void Start()
    {
        m_timer = spawnRate;
    }

    private void Update()
    {
        m_timer -= Time.deltaTime;
        if (m_timer <= 0 && GameManager.Instance.CanSpawnEnemy())
        {
            var randomHeight = Random.Range(-height, height);
            EnemyFactory.Instance.SpawnRandomEnemy(new Vector2(transform.position.x, randomHeight), Quaternion.identity);
            m_timer = spawnRate;
        }
    }
}
