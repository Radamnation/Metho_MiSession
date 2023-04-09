using System;

[Serializable]
public class SaveFile
{
    public int gold;
    public float bestTime;

    public bool extraSkinUnlocked;
    public bool extraLevelUnlocked;
    public bool doubleXPUnlocked;

    public void IncreaseGold(int _gold)
    {
        gold += _gold;
        UIManager.Instance.MainView.UpdateGold();
    }
}
