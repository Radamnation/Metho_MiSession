using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ProjectileData", fileName = "ProjectileDataSO")]
public class ProjectileData : ScriptableObject
{
    public bool IsOrbital;
    public RuntimeAnimatorController animatorController;
    public float colliderRadius;
    public float lifeTime;
}
