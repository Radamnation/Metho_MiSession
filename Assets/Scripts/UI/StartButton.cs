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
        if (GameManager.Instance.CharacterSelected > -1
            && GameManager.Instance.LevelSelected > -1)
        {
            m_button.interactable = true;
            return;
        }
        m_button.interactable = false;
    }
}
