using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationController : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Localization.Instance.ChangeLanguage("en");
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            Localization.Instance.ChangeLanguage("fr");
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            Localization.Instance.ChangeLanguage("es");
        }
    }
}
