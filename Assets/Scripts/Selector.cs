using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    [SerializeField] private bool Enabled;
    [SerializeField] private UnityEvent OnClick;

    [SerializeField] private Image background;

    private void OnMouseUp()
    {
        if (!Enabled) return;
        ChangeColor(Color.green);
    }

    public void ChangeColor(Color _color)
    {
        background.color = _color;
    }
}
