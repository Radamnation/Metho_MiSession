using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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

    private bool m_gameIsPaused = false;

    public void PauseGame()
    {
        if (!m_gameIsPaused)
        {
            Time.timeScale = 0;
            m_gameIsPaused = true;
            UIManager.Instance.OpenPauseView();
        }
        else
        {
            Time.timeScale = 1;
            m_gameIsPaused = false;
            UIManager.Instance.ClosePauseView();
        }
    }
}
