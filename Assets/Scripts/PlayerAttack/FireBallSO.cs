using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PlayerAttack/FireBall", fileName = "FireBallSO")]
public class FireBallSO : PlayerAttackSO
{
    public override void ExecuteAttack()
    {
        for (int i = 0; i < CurrentLevel; i++)
        {
            var randomEnemy = Enemy.EnemyOnScreenList.ElementAt(Random.Range(0, Enemy.EnemyOnScreenList.Count)).Value;
            var shootingDirection = randomEnemy.transform.position - Player.Instance.transform.position;
            ProjectileFactory.Instance.SpawnProjectile(projectileData, Player.Instance.transform.position, Quaternion.LookRotation(randomEnemy.transform.forward, shootingDirection));
        }
    }
}
