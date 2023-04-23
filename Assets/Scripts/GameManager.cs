using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public int CharacterSelected = -1;
    public int LevelSelected = -1;

    [SerializeField] private int enemyLimit;
    [SerializeField] private List<PlayerAttackSO> playerAttackList;

    private bool m_gameIsPaused;
    public Scene currentScene;

    public void SetCharactedSelection(int _selection)
    {
        CharacterSelected = _selection;
    }

    public void SetLevelSelection(int _selection)
    {
        LevelSelected = _selection;
    }

    public PlayerAttackSO GetRandomAttack()
    {
        return playerAttackList[Random.Range(0, playerAttackList.Count)];
    }

    public bool CanSpawnEnemy()
    {
        return Enemy.ActiveEnemyList.Count < enemyLimit * Player.Instance.CurrentLevel;
    }

    public bool IsPaused()
    {
        return m_gameIsPaused;
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
