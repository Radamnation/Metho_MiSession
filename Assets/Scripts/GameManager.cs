using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            m_inputSystem = GetComponent<InputSystem>();
            return;
        }
        Destroy(gameObject);
    }

    [SerializeField] private int enemyLimit;
    [SerializeField] private List<PlayerAttackSO> playerAttackList;
    private List<Enemy> m_enemyList = new();
    private List<Enemy> m_enemyOnScreenList = new();
    
    private bool m_gameIsPaused;

    private InputSystem m_inputSystem;

    public List<Enemy> EnemyList { get => m_enemyList; set => m_enemyList = value; }
    public List<Enemy> EnemyOnScreenList { get => m_enemyOnScreenList; set => m_enemyOnScreenList = value; }

    private void Start()
    {
        m_inputSystem.AddHandler(Player.Instance.Controller);
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
        return m_enemyList.Count < enemyLimit * Player.Instance.CurrentLevel;
    }

    public void PauseGame()
    {
        if (!m_gameIsPaused)
        {
            Time.timeScale = 0;
            m_gameIsPaused = true;
            m_inputSystem.AddHandler(UIManager.Instance.Controller);
        }
        else
        {
            Time.timeScale = 1;
            m_gameIsPaused = false;
            m_inputSystem.RemoveHandler(UIManager.Instance.Controller);
        }
    }
    
    public void UnpauseGame()
    {
        Time.timeScale = 1;
        m_gameIsPaused = false;
    }
}
