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
    private List<Enemy> m_enemyList = new List<Enemy>();

    private bool m_gameIsPaused = false;

    public List<Enemy> EnemyList { get => m_enemyList; set => m_enemyList = value; }

    public bool CanSpawnEnemy()
    {
        return m_enemyList.Count < m_enemyLimit;
    }

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
