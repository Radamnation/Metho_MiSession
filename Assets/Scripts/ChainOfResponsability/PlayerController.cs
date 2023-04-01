using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IHandleInput
{
    public bool HandleInput()
    {
        if (Pause()) return true;
        if (Player.Instance.Move()) return true;

        return false;
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
