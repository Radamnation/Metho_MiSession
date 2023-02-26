using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/PlayerAttack/LightningStrike", fileName = "LightningStrikeSO")]
public class LightningStrikeSO : PlayerAttackSO
{
    public override void ExecuteAttack()
    {
        var randomEnemy = GameManager.Instance.EnemyList[Random.Range(0, GameManager.Instance.EnemyList.Count)];
        PoolManager.Instance.GetVFX(randomEnemy.transform.position, Quaternion.identity);
        randomEnemy.Death();
    }
}
