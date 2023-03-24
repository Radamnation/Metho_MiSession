using System;

[Serializable]
public class SaveFile
{
    public int gold;

    public void AddGold(int _amount)
    {
        gold += _amount;
    }
    
    public void RemoveGold(int _amount)
    {
        gold -= _amount;
    }
    
    public void ResetGold()
    {
        gold = 0;
    }
}
