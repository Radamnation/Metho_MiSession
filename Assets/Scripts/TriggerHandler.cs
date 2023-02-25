using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

enum TriggerType { OnTriggerEnter, OnTriggerExit, OnTriggerStay };

public class TriggerHandler : MonoBehaviour
{
    [SerializeField] private TriggerType triggerType;
    [SerializeField] private UnityEvent<Collider2D> onTriggerEvent;

    private void OnTriggerEnter2D(Collider2D _collider2D)
    {
        if (!enabled || triggerType != TriggerType.OnTriggerEnter || onTriggerEvent == null)
            return;
        
        onTriggerEvent.Invoke(_collider2D);
    }
    
    private void OnTriggerExit2D(Collider2D _collider2D)
    {
        if (!enabled || triggerType != TriggerType.OnTriggerExit || onTriggerEvent == null)
            return;
        
        onTriggerEvent.Invoke(_collider2D);
    }
    
    private void OnTriggerStay2D(Collider2D _collider2D)
    {
        if (!enabled || triggerType != TriggerType.OnTriggerStay || onTriggerEvent == null)
            return;
        
        onTriggerEvent.Invoke(_collider2D);
    }
}
