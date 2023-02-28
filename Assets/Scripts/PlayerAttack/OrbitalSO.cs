using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PlayerAttack/Orbital", fileName = "OrbitalSO")]
public class OrbitalSO : PlayerAttackSO
{
    public override void ExecuteAttack()
    {
        var intervals = 360 / CurrentLevel;
        for (int i = 0; i < CurrentLevel; i++)
        {
            ProjectileFactory.Instance.SpawnProjectile(projectileData, Player.Instance.transform.position, Quaternion.Euler(0, 0, i * intervals));
        }
    }
}
