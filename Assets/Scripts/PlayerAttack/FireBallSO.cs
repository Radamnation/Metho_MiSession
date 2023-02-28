using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PlayerAttack/FireBall", fileName = "FireBallSO")]
public class FireBallSO : PlayerAttackSO
{
    public override void ExecuteAttack()
    {
        for (int i = 0; i < CurrentLevel; i++)
        {
            var randomEnemy = GameManager.Instance.EnemyOnScreenList[Random.Range(0, GameManager.Instance.EnemyOnScreenList.Count)];
            var shootingDirection = randomEnemy.transform.position - Player.Instance.transform.position;
            ProjectileFactory.Instance.SpawnProjectile(projectileData, Player.Instance.transform.position, Quaternion.LookRotation(randomEnemy.transform.forward, shootingDirection));
        }
    }
}
