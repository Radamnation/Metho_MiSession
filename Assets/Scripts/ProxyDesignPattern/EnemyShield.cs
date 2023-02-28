using TNRD;
using UnityEngine;

public class EnemyShield : MonoBehaviour, IDamagable, ICollidable
{
    [SerializeField] private SerializableInterface<IDamagable> realDamagable;
    [SerializeField] private SerializableInterface<ICollidable> realCollidable;
    [SerializeField] private float shieldPercentage = 0.5f;

    public void TakeDamage(float _damage)
    {
        realDamagable.Value.TakeDamage(_damage * shieldPercentage);
    }

    public void AddForce(Vector3 _impact)
    {
        realDamagable.Value.AddForce(_impact);
    }

    public void Collide(Player _player)
    {
        realCollidable.Value.Collide(_player);
    }
}
