using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/PlayerAttack/LightningStrike", fileName = "LightningStrikeSO")]
public class LightningSO : PlayerAttackSO
{
    public override void ExecuteAttack()
    {
        for (int i = 0; i < (CurrentLevel + 1) / 2; i++)
        {
            var randomEnemy = GameManager.Instance.EnemyList[Random.Range(0, GameManager.Instance.EnemyList.Count)];
            ProjectileFactory.Instance.SpawnProjectile(projectileData, randomEnemy.transform.position, Quaternion.identity);
            PoolManager.Instance.GetVFX(randomEnemy.transform.position, Quaternion.identity);
        }
    }
}
