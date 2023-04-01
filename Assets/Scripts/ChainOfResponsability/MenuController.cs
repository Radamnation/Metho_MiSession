using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour, IHandleInput
{
    public bool HandleInput()
    {
        if (Pause()) return true;

        return true;
    }
    
    private bool Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.TogglePauseView();
            return true;
        }

        return false;
    }
}