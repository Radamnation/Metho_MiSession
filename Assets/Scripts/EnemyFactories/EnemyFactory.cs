using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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

    public void SpawnRandomEnemy(Vector3 _position, Quaternion _rotation)
    {
        var enemyData = enemiesData[Random.Range(0, enemiesData.Count)];
        Enemy randomEnemy = PoolManager.Instance.GetEnemy(_position, _rotation);
        randomEnemy.Animator.runtimeAnimatorController = enemyData.animatorController;
        randomEnemy.Collider.radius = enemyData.colliderRadius;
        if (Random.Range(0, 100) < bossChance)
        {
            randomEnemy.transform.localScale *= 1.5f;
            return;
        }
        randomEnemy.transform.localScale = Vector3.one;
    }
}
