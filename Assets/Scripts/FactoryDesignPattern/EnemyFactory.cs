using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    public static EnemyFactory Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }
    
    [SerializeField] private List<EnemyData> enemiesData;
    [SerializeField] private float bossChance = 0f;

    public void SpawnEnemy(Vector3 _position, Quaternion _rotation)
    {
        var enemyData = enemiesData[Player.Instance.CurrentLevel / 5];
        
        Enemy newEnemy = PoolManager.Instance.GetEnemy();
        newEnemy.transform.SetPositionAndRotation(_position, _rotation);
        newEnemy.Animator.runtimeAnimatorController = enemyData.animatorController;
        newEnemy.Collider.radius = enemyData.colliderRadius;
        
        
        if (Random.Range(0, 100) < bossChance * Player.Instance.CurrentLevel / 5)
        {
            newEnemy.ShieldHealth = enemyData.shieldHealth * 5f;
            newEnemy.ShieldChance = enemyData.shieldChance / 2;
            newEnemy.MaxHealth = enemyData.maxHealth * 10f;
            newEnemy.transform.localScale *= 1.3f;
        }
        else
        {
            newEnemy.ShieldHealth = enemyData.shieldHealth;
            newEnemy.ShieldChance = enemyData.shieldChance;
            newEnemy.MaxHealth = enemyData.maxHealth;
            newEnemy.transform.localScale = Vector3.one;
        }
        newEnemy.Initialize();
    }
}
