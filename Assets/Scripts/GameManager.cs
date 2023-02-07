using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
    }

    [SerializeField] private int m_enemyLimit;
    private int m_enemiesOnScreen;

    public int EnemiesOnScreen { get => m_enemiesOnScreen; set => m_enemiesOnScreen = value; }

    public bool CanSpawnEnemy()
    {
        return m_enemiesOnScreen < m_enemyLimit;
    }
}
