using System.Collections;
using System.Collections.Generic;
using TNRD;
using UnityEngine;

public class EnemyShield : MonoBehaviour, IDamagable
{
    [SerializeField] private SerializableInterface<IDamagable> enemy;
    [SerializeField] private float shieldPercentage = 0.5f;

    public void TakeDamage(float _damage)
    {
        Debug.Log("Shield reduce damage by " + shieldPercentage * 100 + "%");
        enemy.Value.TakeDamage(_damage * shieldPercentage);
    }

    public void AddForce(Vector3 _impact)
    {
        enemy.Value.AddForce(_impact);
    }
}
