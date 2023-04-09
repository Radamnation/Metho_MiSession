using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PickupData", fileName = "PickupDataSO")]
public class PickupData : ScriptableObject
{
    public RuntimeAnimatorController AnimatorController;
    public AudioClip PickupSFX;
    
    public int ExperienceValue;
    public int HealthValue;
    public int GoldValue;
}
