using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private PoolableObject poolObjectPrefab;
    [SerializeField] private int poolObjectCount;
    private List<PoolableObject> m_poolQueue = new();

    private void Awake()
    {
        for (int i = 0; i < poolObjectCount; i++)
        {
            var newPoolObject = Instantiate(poolObjectPrefab, transform);
            newPoolObject.gameObject.SetActive(false);
            newPoolObject.Pool = this;
            m_poolQueue.Add(newPoolObject);
        }
    }
    
    public PoolableObject Depool()
    {
        var objectToSpawn = m_poolQueue[0];
        m_poolQueue.RemoveAt(0);
        objectToSpawn.gameObject.SetActive(true);
        // objectToSpawn.Pool = this;
        return objectToSpawn;
    }

    public PoolableObject Depool(Vector3 _position, Quaternion _rotation)
    {
        var objectToSpawn = m_poolQueue[0];
        m_poolQueue.RemoveAt(0);

        objectToSpawn.transform.SetPositionAndRotation(_position, _rotation);
        objectToSpawn.gameObject.SetActive(true);
        objectToSpawn.Initialize();
        // objectToSpawn.Pool = this;
        return objectToSpawn;
    }

    public void Repool(PoolableObject _poolObject)
    {
        _poolObject.gameObject.SetActive(false);
        m_poolQueue.Add(_poolObject);
    }
}
