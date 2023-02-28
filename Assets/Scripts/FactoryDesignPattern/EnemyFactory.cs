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
        newEnemy.ShieldHealth = enemyData.shieldHealth;
        
        if (Random.Range(0, 100) < bossChance)
        {
            newEnemy.MaxHealth = enemyData.maxHealth * 5f;
            newEnemy.transform.localScale *= 1.2f;
        }
        else
        {
            newEnemy.MaxHealth = enemyData.maxHealth;
            newEnemy.transform.localScale = Vector3.one;
        }
        newEnemy.Initialize();
    }
}
