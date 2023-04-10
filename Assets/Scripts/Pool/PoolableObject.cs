using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolableObject : MonoBehaviour
{
    public static Dictionary<GameObject, PoolableObject> ActivePoolableObjects = new();
    private ObjectPool m_pool;

    public ObjectPool Pool { set => m_pool = value; }

    public virtual void Initialize()
    {
        if (!ActivePoolableObjects.ContainsKey(gameObject))
        {
            ActivePoolableObjects.Add(gameObject, this);
        }
    }

    public virtual void Repool()
    {
        if (ActivePoolableObjects.ContainsKey(gameObject))
        {
            ActivePoolableObjects.Remove(gameObject);
        }
        m_pool.Repool(this);
    }
}
