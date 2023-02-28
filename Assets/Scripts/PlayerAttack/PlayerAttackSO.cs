using System;
using UnityEngine;

public abstract class PlayerAttackSO : ScriptableObject, IAttack
{
    [SerializeField] private String attackName;
    [SerializeField] private String attackDescription;
    [SerializeField] private int currentLevel;
    
    [SerializeField] private AudioClip spawnSFX;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected ProjectileData projectileData;
    
    [SerializeField] private bool affectVisibleEnemy;
    
    [SerializeField] private bool levelAccelarateCasting;
    [SerializeField] private float accelarationRate;
    
    private float attackTimer;

    public string AttackName { get => attackName; }
    public string AttackDescription { get => attackDescription; }
    public int CurrentLevel { get => currentLevel; }

    public void Initialize()
    {
        currentLevel = 0;
        attackTimer = 0;
    }
    
    public void Refresh(float _delta)
    {
        if (currentLevel <= 0) return;
        if (affectVisibleEnemy && GameManager.Instance.EnemyList.Count <= 0) return;

        attackTimer -= _delta;
        if (attackTimer > 0) return;
        
        ExecuteAttack();
        AudioManager.Instance.SfxAudioSource.PlayOneShot(spawnSFX);

        if (!levelAccelarateCasting)
        {
            attackTimer = attackSpeed;
        }
        else
        {
            attackTimer = attackSpeed - (currentLevel - 1) * accelarationRate;
        }
    }

    public void LevelUp()
    {
        currentLevel++;
    }

    public abstract void ExecuteAttack();
}
