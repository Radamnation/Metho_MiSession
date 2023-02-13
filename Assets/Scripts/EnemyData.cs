using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ScriptableObject/EnemyData", fileName = "EnemyDataSO")]
public class EnemyData : ScriptableObject
{
    public RuntimeAnimatorController animatorController;
    public float colliderRadius;
}
