using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/PlayerAttack/LightningStrike", fileName = "LightningStrikeSO")]
public class LightningSO : PlayerAttackSO
{
    public override void ExecuteAttack()
    {
        for (int i = 0; i < CurrentLevel; i++)
        {
            var randomEnemy = Enemy.EnemyOnScreenList.ElementAt(Random.Range(0, Enemy.EnemyOnScreenList.Count)).Value;
            ProjectileFactory.Instance.SpawnProjectile(projectileData, randomEnemy.transform.position, Quaternion.identity);
            PoolManager.Instance.GetVFX(randomEnemy.transform.position, Quaternion.identity);
        }
    }
}
