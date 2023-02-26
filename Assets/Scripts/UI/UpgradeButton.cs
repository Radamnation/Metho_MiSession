using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    private PlayerAttackSO playerAttack;
    [SerializeField] private TextMeshProUGUI attackName;
    [SerializeField] private TextMeshProUGUI attackDescription;
    [SerializeField] private TextMeshProUGUI nextLevel;

    public void UpdateButton(PlayerAttackSO _playerAttack)
    {
        playerAttack = _playerAttack;
        attackName.text = playerAttack.AttackName;
        // attackDescription.text = playerAttack.AttackDescription;
        nextLevel.text = (playerAttack.CurrentLevel + 1).ToString();
    }

    public void ButtonClick()
    {
        playerAttack.LevelUp();
    }
}
