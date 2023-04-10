using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpView : UIView
{
    [SerializeField] private List<UpgradeButton> upgradeButtonList;

    public override void OnShow()
    {
        base.OnShow();
        foreach (var upgradeButton in upgradeButtonList)
        {
            upgradeButton.UpdateButton(GameManager.Instance.GetRandomAttack());
        }

        if (TestManager.Instance.IsTesting)
        {
            upgradeButtonList[Random.Range(0, upgradeButtonList.Count)].GetComponent<Button>().onClick.Invoke();
        }
    }
}
