using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartView : UIView
{
    [SerializeField] private Selector characterSelector1;
    [SerializeField] private Selector characterSelector2;
    
    [SerializeField] private Selector levelSelector1;
    [SerializeField] private Selector levelSelector2;
    
    private int currentPlayerChoice;
    private int currentLevelChoice;

    public override void OnShow()
    {
        base.OnShow();

        characterSelector1.Interactable = true;
        characterSelector1.Initialize();
        characterSelector2.Interactable = SaveManager.Instance.SaveFile.extraSkinUnlocked;
        characterSelector2.Initialize();
        
        levelSelector1.Interactable = true;
        levelSelector1.Initialize();
        levelSelector2.Interactable = SaveManager.Instance.SaveFile.extraLevelUnlocked;
        levelSelector2.Initialize();
    }
}
