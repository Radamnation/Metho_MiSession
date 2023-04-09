using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        Destroy(gameObject);
    }

    [SerializeField] private ObjectPool enemyPool;
    [SerializeField] private ObjectPool vfxPool;
    [SerializeField] private ObjectPool pickupPool;
    [SerializeField] private ObjectPool projectilePool;

    public Enemy GetEnemy()
    {
        return (Enemy)enemyPool.Depool();
    }

    public void GetVFX(Vector3 _position, Quaternion _rotation)
    {
        vfxPool.Depool(_position, _rotation);
    }

    public Pickup GetPickup()
    {
        return (Pickup)pickupPool.Depool();
    }

    public Projectile GetProjectile()
    {
        return (Projectile)projectilePool.Depool();
    }

    public void ResetPools()
    {
        List<PoolableObject> values = new();
        foreach (var value in PoolableObject.ActivePoolableObjects.Values)
        {
            values.Add(value);
        }
        for (int i = 0; i < values.Count; i++)
        {
            values[i].Repool();
        }
    }
}