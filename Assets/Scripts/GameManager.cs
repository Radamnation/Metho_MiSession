using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
    [SerializeField] private List<PlayerAttackSO> playerAttackList;
    // private List<Enemy> m_enemyList = new();
    // private List<Enemy> m_enemyOnScreenList = new();
    
    private bool m_gameIsPaused;
    public Scene currentScene;

    // public List<Enemy> EnemyList { get => m_enemyList; set => m_enemyList = value; }
    // public List<Enemy> EnemyOnScreenList { get => m_enemyOnScreenList; set => m_enemyOnScreenList = value; }

    private void Start()
    {
        foreach (var playerAttack in playerAttackList)
        {
            playerAttack.Initialize();
        }
        playerAttackList[Random.Range(0, playerAttackList.Count)].LevelUp();
    }

    public PlayerAttackSO GetRandomAttack()
    {
        return playerAttackList[Random.Range(0, playerAttackList.Count)];
    }

    public bool CanSpawnEnemy()
    {
        return Enemy.ActiveEnemyList.Count < enemyLimit * Player.Instance.CurrentLevel;
    }

    public void PauseGame()
    {
        if (!m_gameIsPaused)
        {
            Time.timeScale = 0;
            m_gameIsPaused = true;
            InputSystem.Instance.AddHandler(UIManager.Instance.Controller);
        }
        else
        {
            if (TestManager.Instance.IsTesting)
            {
                Time.timeScale = 10;
            }
            else
            {
                Time.timeScale = 1;
            }
            m_gameIsPaused = false;
            InputSystem.Instance.RemoveHandler(UIManager.Instance.Controller);
        }
    }
    
    public void UnpauseGame()
    {
        Time.timeScale = 1;
        m_gameIsPaused = false;
    }
}
