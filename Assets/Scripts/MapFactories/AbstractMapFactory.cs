using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AbstractMapFactory : MonoBehaviour
{
    [SerializeField] private MapBlock mapBlockPrefab;

    public List<MapBlock> GenerateMap()
    {
        var mapBlocks = new List<MapBlock>();
        
        for (int i = -24; i <= 24; i = i + 8)
        {
            var newMapBlock = Instantiate(mapBlockPrefab, new Vector3(i, transform.position.y, 0), Quaternion.identity);
            newMapBlock.transform.SetParent(transform);
            mapBlocks.Add(newMapBlock);
            newMapBlock.Initialize();
        }

        return mapBlocks;
    }
}
