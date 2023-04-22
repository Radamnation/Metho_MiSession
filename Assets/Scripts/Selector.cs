using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    public bool Interactable;
    public bool IsDefault;
    
    [SerializeField] private Image foreground;
    [SerializeField] private Image background;
    
    [SerializeField] private UnityEvent<Color> OnClick;

    public void Initialize()
    {
        if (Interactable)
        {
            foreground.color = Color.white;
            background.color = IsDefault ? Color.green : Color.white;
        }
        else
        {
            foreground.color = Color.black;
            background.color = Color.grey;
        }
    }

    public void Click()
    {
        if (!Interactable) return;
        
        OnClick?.Invoke(Color.white);
        ChangeBackgroundColor(Color.green);
        onChange?.Invoke();
    }

    public void ChangeBackgroundColor(Color _color)
    {
        if (!Interactable) return;
        
        background.color = _color;
    }
    
    private Action onChange;
    
    internal void Subscribe(Action onChange)
        => this.onChange += onChange;
}
