using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PlayerAttack/LightningStrike", fileName = "LightningStrike")]
public class LightningStrike : PlayerAttack
{
    public override void Execute()
    {
        var randomEnemy = GameManager.Instance.EnemyList[Random.Range(0, GameManager.Instance.EnemyList.Count)];
        PoolManager.Instance.GetVFX(randomEnemy.transform.position, Quaternion.identity);
        randomEnemy.Death();
    }
}
