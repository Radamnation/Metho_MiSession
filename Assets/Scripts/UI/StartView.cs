using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class StartView : UIView
{
    [SerializeField] private Selector characterSelector1;
    [SerializeField] private Selector characterSelector2;
    
    [SerializeField] private Selector levelSelector1;
    [SerializeField] private Selector levelSelector2;
    
    [SerializeField] private TMP_Text currentLoginText;

    [SerializeField] private StartButton m_startButton;

    public List<Selector> Selectors;
    private bool characterSelected;
    private bool levelSelected;
    
    public bool CharacterSelected
    {
        get => characterSelected;
    }
    
    public bool LevelSelected
    {
        get => levelSelected;
    }
    
    private int currentPlayerChoice;
    private int currentLevelChoice;

    private void Awake()
    {
        Selectors.Add(characterSelector1);
        Selectors.Add(characterSelector2);
        Selectors.Add(levelSelector1);
        Selectors.Add(levelSelector2);
    }

    public void SetCharacterSelected()
    {
        characterSelected = true;
    }

    public void SetLevelSelected()
    {
        levelSelected = true;
    }

    public override void OnShow()
    {
        characterSelector1.Interactable = true;
        characterSelector1.Initialize();
        characterSelector2.Interactable = SaveManager.Instance.SaveFile.extraSkinUnlocked;
        characterSelector2.Initialize();

        characterSelected = false;
        levelSelected = false;
        
        m_startButton.UpdateInteractability();
        
        levelSelector1.Interactable = true;
        levelSelector1.Initialize();
        levelSelector2.Interactable = SaveManager.Instance.SaveFile.extraLevelUnlocked;
        levelSelector2.Initialize();
        
        if (LoginManager.Instance.IsLoggedIn(out var userName))
        {
            currentLoginText.text = "Currently logged as " + userName;
        }
        else
        {
            currentLoginText.text = "Not currently logged in";
        }
        
        base.OnShow();
    }
}
