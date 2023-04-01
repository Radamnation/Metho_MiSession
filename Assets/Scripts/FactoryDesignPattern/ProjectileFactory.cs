using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFactory : MonoBehaviour
{
    public static ProjectileFactory Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }
    
    // [SerializeField] private List<ProjectileData> projectileDataList;
    //
    // public void SpawnRandomProjectile(Vector3 _position, Quaternion _rotation)
    // {
    //     var projectileData = projectileDataList[Random.Range(0, projectileDataList.Count)];
    //     
    //     Projectile randomProjectile = PoolManager.Instance.GetProjectile();
    //     randomProjectile.transform.SetPositionAndRotation(_position, _rotation);
    //     randomProjectile.IsOrbital = projectileData.IsOrbital;
    //     randomProjectile.Animator.runtimeAnimatorController = projectileData.animatorController;
    //     randomProjectile.CircleCollider2D.radius = projectileData.colliderRadius;
    //     
    //     randomProjectile.Initialize();
    // }
    
    public void SpawnProjectile(ProjectileData _projectileData, Vector3 _position, Quaternion _rotation)
    {
        var newProjectile = PoolManager.Instance.GetProjectile();
        
        newProjectile.transform.SetPositionAndRotation(_position, _rotation);
        newProjectile.Damage = _projectileData.Damage;
        newProjectile.SpawnSFX = _projectileData.SpawnSFX;
        newProjectile.Rotate = _projectileData.Rotate;
        if (newProjectile.Rotate)
        {
            newProjectile.RotationSpeed = _projectileData.RotationSpeed;
        }
        newProjectile.Speed = _projectileData.Speed;
        newProjectile.HitOnSpawn = _projectileData.HitOnSpawn;
        newProjectile.UseBoxCollider = _projectileData.UseBoxCollider;
        if (!newProjectile.UseBoxCollider)
        {
            newProjectile.CircleCollider2D.radius = _projectileData.colliderRadius;
        }
        else
        {
            newProjectile.BoxCollider2D.size = _projectileData.colliderSize;
        }
        newProjectile.IsOrbital = _projectileData.IsOrbital;
        if (newProjectile.IsOrbital)
        {
            newProjectile.OrbitDistance = _projectileData.OrbitDistance;
        }
        newProjectile.LifeTime = _projectileData.lifeTime;
        newProjectile.Animator.runtimeAnimatorController = _projectileData.animatorController;
        newProjectile.Animator.gameObject.transform.localScale =  new Vector3(-Player.Instance.VisualTransform.localScale.x * _projectileData.Scale.x, _projectileData.Scale.y, _projectileData.Scale.z);

        newProjectile.Initialize();
    }
}
