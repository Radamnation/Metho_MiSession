using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PlayerAttack/Claw", fileName = "ClawSO")]
public class KittyClawSO : PlayerAttackSO
{
    public override void ExecuteAttack()
    {
        var playerTransform = Player.Instance.transform;
        ProjectileFactory.Instance.SpawnProjectile(projectileData, playerTransform.position - playerTransform.localScale.x * new Vector3(2.5f, 0, 0), Quaternion.identity);
    }
}
