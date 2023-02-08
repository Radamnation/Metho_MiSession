using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private PoolableObject m_poolObjectPrefab;
    [SerializeField] private int m_poolObjectCount;
    private Queue<PoolableObject> m_poolQueue = new Queue<PoolableObject>();

    private void Awake()
    {
        for (int i = 0; i < m_poolObjectCount; i++)
        {
            var newPoolObject = Instantiate(m_poolObjectPrefab, transform);
            newPoolObject.gameObject.SetActive(false);
            m_poolQueue.Enqueue(newPoolObject);
        }
    }

    private void Start()
    {
        
    }

    public void Depool(Vector3 _position, Quaternion _rotation)
    {
        var objectToSpawn = m_poolQueue.Dequeue();

        objectToSpawn.transform.SetPositionAndRotation(_position, _rotation);
        objectToSpawn.gameObject.SetActive(true);
        objectToSpawn.Initialize();
        objectToSpawn.Pool = this;
    }

    public void Repool(PoolableObject _poolObject)
    {
        _poolObject.gameObject.SetActive(false);
        m_poolQueue.Enqueue(_poolObject);
    }
}
