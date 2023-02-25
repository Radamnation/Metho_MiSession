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
    
    [SerializeField] private List<ProjectileData> projectileDataList;

    public void SpawnRandomProjectile(Vector3 _position, Quaternion _rotation)
    {
        var projectileData = projectileDataList[Random.Range(0, projectileDataList.Count)];
        
        Projectile randomProjectile = PoolManager.Instance.GetProjectile();
        randomProjectile.transform.SetPositionAndRotation(_position, _rotation);
        randomProjectile.IsOrbital = projectileData.IsOrbital;
        randomProjectile.Animator.runtimeAnimatorController = projectileData.animatorController;
        randomProjectile.Collider.radius = projectileData.colliderRadius;
        
        randomProjectile.Initialize();
    }
}
