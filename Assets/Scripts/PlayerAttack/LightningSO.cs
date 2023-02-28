using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/PlayerAttack/LightningStrike", fileName = "LightningStrikeSO")]
public class LightningSO : PlayerAttackSO
{
    public override void ExecuteAttack()
    {
        for (int i = 0; i < CurrentLevel; i++)
        {
            var randomEnemy = GameManager.Instance.EnemyOnScreenList[Random.Range(0, GameManager.Instance.EnemyOnScreenList.Count)];
            ProjectileFactory.Instance.SpawnProjectile(projectileData, randomEnemy.transform.position, Quaternion.identity);
            PoolManager.Instance.GetVFX(randomEnemy.transform.position, Quaternion.identity);
        }
    }
}
