using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/EnemyData", fileName = "EnemyDataSO")]
public class EnemyData : ScriptableObject
{
    public RuntimeAnimatorController animatorController;
    public float colliderRadius;
    public float maxHealth;
    public float shieldHealth;
}
