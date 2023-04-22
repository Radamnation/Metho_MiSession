using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    private Button m_button;
    
    public void Start()
    {
        m_button = GetComponent<Button>();
        var selectors = UIManager.Instance.StartView.Selectors;
        foreach (var selector in selectors)
        {
            selector.Subscribe(UpdateInteractability);
        }
    }

    public void UpdateInteractability()
    {
        if (UIManager.Instance.StartView.CharacterSelected
            && UIManager.Instance.StartView.LevelSelected)
        {
            m_button.interactable = true;
            return;
        }
        m_button.interactable = false;
    }
}
