using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapBlock : MonoBehaviour
{
    [SerializeField] private List<GameObject> obstacles;

    public void Initialize()
    {
        var seed = (int) transform.position.x;
        Random.InitState(seed);

        for (int i = 0; i < obstacles.Count; i++)
        {
            if (TestManager.Instance.IsTesting)
            {
                obstacles[i].SetActive(false);
            }
            else
            {
                obstacles[i].SetActive(Random.Range(0, 100) < 10);
            }
        }
    }
}
