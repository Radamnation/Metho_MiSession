using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PlayerAttack/FireBall", fileName = "FireBallSO")]
public class FireBallSO : ScriptableObject, IAttack
{
    public void Execute()
    {
        var randomEnemy = GameManager.Instance.EnemyList[Random.Range(0, GameManager.Instance.EnemyList.Count)];
        var shootingDirection = randomEnemy.transform.position - Player.Instance.transform.position;
        ProjectileFactory.Instance.SpawnRandomProjectile(Player.Instance.transform.position, Quaternion.LookRotation(randomEnemy.transform.forward, shootingDirection));
    }
}
