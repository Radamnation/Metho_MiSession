using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private ObjectPool m_enemyPool;
    [SerializeField] private ObjectPool m_vfxPool;
    [SerializeField] private ObjectPool m_pickupPool;

    public void GetEnemy(Vector3 _position, Quaternion _rotation)
    {
        m_enemyPool.Depool(_position, _rotation);
    }

    public void GetVFX(Vector3 _position, Quaternion _rotation)
    {
        m_vfxPool.Depool(_position, _rotation);
    }

    public void GetPickup(Vector3 _position, Quaternion _rotation)
    {
        m_pickupPool.Depool(_position, _rotation);
    }
}
