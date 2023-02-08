using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolableObject : MonoBehaviour
{
    private ObjectPool m_pool;

    public ObjectPool Pool { get => m_pool; set => m_pool = value; }

    public abstract void Initialize();

    public void Repool()
    {
        m_pool.Repool(this);
    }
}
