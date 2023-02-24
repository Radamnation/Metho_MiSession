using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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

    [SerializeField] private int enemyLimit;
    private List<Enemy> m_enemyList = new();

    private bool m_gameIsPaused;

    public List<Enemy> EnemyList { get => m_enemyList; set => m_enemyList = value; }

    public bool CanSpawnEnemy()
    {
        return m_enemyList.Count < enemyLimit;
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
