using UnityEngine;

public interface IDamagable
{
    public void TakeDamage(float _damage);
    public void AddForce(Vector3 _impact);
}
