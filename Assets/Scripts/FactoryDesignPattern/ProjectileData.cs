using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ProjectileData", fileName = "ProjectileDataSO")]
public class ProjectileData : ScriptableObject
{
    public AudioClip SpawnSFX;
    public bool HitOnSpawn;
    public Vector3 Scale;
    public float Speed;
    public bool IsOrbital;
    public float OrbitDistance;
    public RuntimeAnimatorController animatorController;
    public bool UseBoxCollider;
    public float colliderRadius;
    public Vector2 colliderSize;
    public float lifeTime;
    public bool Rotate;
    public float RotationSpeed;
}
