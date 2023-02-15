using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChecker : MonoBehaviour
{
    public static EnemyChecker Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }
    
    public SortedSet<Enemy> SortedEnemy = new SortedSet<Enemy>(new EnemyComparer());

    private class EnemyComparer : IComparer<Enemy>
    {
        public int Compare(Enemy _x, Enemy _y)
        {
            if (_x.GoldValue != _y.GoldValue)
            {
                return _x.GoldValue.CompareTo(_y.GoldValue);
            }
            return _x.GetInstanceID().CompareTo(_y.GetInstanceID());
        }
    }
}
