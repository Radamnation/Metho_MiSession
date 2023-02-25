using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

enum CollisionType { OnCollisionEnter, OnCollisionExit, OnCollisionStay };

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] private CollisionType m_collisionType;
    [SerializeField] private UnityEvent<Collision> OnCollisionEvent;

    private void OnCollisionEnter(Collision collision)
    {
        if (m_collisionType != CollisionType.OnCollisionEnter || OnCollisionEvent == null)
            return;

        OnCollisionEvent.Invoke(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisionType != CollisionType.OnCollisionExit || OnCollisionEvent == null)
            return;

        OnCollisionEvent.Invoke(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (m_collisionType != CollisionType.OnCollisionStay || OnCollisionEvent == null)
            return;

        OnCollisionEvent.Invoke(collision);
    }
}
