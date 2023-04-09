using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupFactory : MonoBehaviour
{
    public static PickupFactory Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }
    
    [SerializeField] private PickupData experiencePickup;
    [SerializeField] private PickupData goldPickup;

    public void SpawnExperiencePickup(Vector3 _position, Quaternion _rotation)
    {
        Pickup newPickup = PoolManager.Instance.GetPickup();
        newPickup.transform.SetPositionAndRotation(_position, _rotation);
        
        newPickup.Animator.runtimeAnimatorController = experiencePickup.AnimatorController;
        newPickup.PickupSFX = experiencePickup.PickupSFX;
        newPickup.ExperienceValue = experiencePickup.ExperienceValue;
        newPickup.HealthValue = experiencePickup.HealthValue;
        newPickup.GoldValue = experiencePickup.GoldValue;
        
        newPickup.Initialize();
    }
    
    public void SpawnGoldPickup(Vector3 _position, Quaternion _rotation)
    {
        Pickup newPickup = PoolManager.Instance.GetPickup();
        newPickup.transform.SetPositionAndRotation(_position, _rotation);
        
        newPickup.Animator.runtimeAnimatorController = goldPickup.AnimatorController;
        newPickup.PickupSFX = goldPickup.PickupSFX;
        newPickup.ExperienceValue = goldPickup.ExperienceValue;
        newPickup.HealthValue = goldPickup.HealthValue;
        newPickup.GoldValue = goldPickup.GoldValue;
        
        newPickup.Initialize();
    }
}
