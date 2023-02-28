using TNRD;
using UnityEngine;

public class EnemyShield : MonoBehaviour, IDamagable, ICollidable
{
    [SerializeField] private SerializableInterface<IDamagable> realDamagable;
    [SerializeField] private SerializableInterface<ICollidable> realCollidable;
    [SerializeField] private float maxHealth = 5f;
    private float currentHealth;

    public void Initialize(float _maxHealth)
    {
        maxHealth = _maxHealth;
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(float _damage)
    {
        var extraDamage = currentHealth - _damage;
        currentHealth -= _damage;
        if (currentHealth <= 0)
        {
            realDamagable.Value.TakeDamage(Mathf.Abs(extraDamage));
        }
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
