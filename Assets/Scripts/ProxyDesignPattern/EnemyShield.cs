using System.Runtime.InteropServices.WindowsRuntime;
using TNRD;
using UnityEngine;

public class EnemyShield : MonoBehaviour, IDamagable, ICollidable
{
    [SerializeField] private SerializableInterface<IDamagable> realDamagable;
    [SerializeField] private SerializableInterface<ICollidable> realCollidable;
    [SerializeField] private SpriteRenderer m_spriteRenderer;
    [SerializeField] private float maxHealth = 5f;
    [SerializeField] private AudioClip hitSFX;
    [SerializeField] private AudioClip breakSFX;
    private float currentHealth;
    private bool isDead;
    
    private static readonly int Shield = Shader.PropertyToID("_Shield");

    public void Initialize(float _maxHealth, float _shieldChance)
    {
        if (Random.Range(0, 100) < _shieldChance)
        {
            isDead = false;
            maxHealth = _maxHealth;
        }
        else
        {
            isDead = true;
            maxHealth = 0;
        }
        currentHealth = maxHealth;
        UpdateShieldVisual();
    }

    private void UpdateShieldVisual()
    {
        var shaderShield = 0f;
        if (currentHealth > 0)
        {
            shaderShield = 0.5f;
        }
        m_spriteRenderer.material.SetFloat(Shield, shaderShield);
    }
    
    public void TakeDamage(float _damage)
    {
        if (!isDead)
        {
            currentHealth -= _damage;
            if (currentHealth <= 0)
            {
                Death();
                return;
            }
            AudioManager.Instance.SfxAudioSource.PlayOneShot(hitSFX);
        }
        else
        {
            realDamagable.Value.TakeDamage(_damage);
        }
    }

    private void Death()
    {
        isDead = true;
        UpdateShieldVisual();
        AudioManager.Instance.SfxAudioSource.PlayOneShot(breakSFX);
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
