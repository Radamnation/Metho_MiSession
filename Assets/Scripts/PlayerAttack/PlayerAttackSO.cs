using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class PlayerAttackSO : ScriptableObject, IAttack
{
    [SerializeField] protected int currentLevel;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected ProjectileData projectileData;
    [SerializeField] private bool affectVisibleEnemy;

    private float attackTimer;

    public void Initialize()
    {
        attackTimer = 0;
    }
    
    public void Refresh(float _delta)
    {
        if (affectVisibleEnemy && GameManager.Instance.EnemyList.Count <= 0) return;

        attackTimer -= _delta;
        if (attackTimer > 0) return;
        
        ExecuteAttack();
        attackTimer = attackSpeed;
    }

    public abstract void ExecuteAttack();
}
