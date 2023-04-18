using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathView : UIView
{
    [SerializeField] private TMP_Text deathText;
    
    public void ChangeText(string _text)
    {
        deathText.text = _text;
    }
}
