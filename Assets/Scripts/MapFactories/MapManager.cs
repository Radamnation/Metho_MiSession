using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        Destroy(gameObject);

        m_compositeCollider2D = GetComponent<CompositeCollider2D>();
    }

    [SerializeField] private AbstractMapFactory currentMap;
    public List<MapBlock> mapBlocks = new List<MapBlock>();
    private int m_playerPosition = 0;

    private CompositeCollider2D m_compositeCollider2D;

    [SerializeField] private CinemachineConfiner m_cinemachineConfiner;

    private void Start()
    {
        mapBlocks = currentMap.GenerateMap();
    }

    private void Update()
    {
        var playerPosition = Player.Instance.transform.position.x;

        if (playerPosition >= m_playerPosition + 8)
        {
            m_playerPosition += 8;
            var firstMapBlock = mapBlocks[0];
            var lastMapBlock = mapBlocks[^1];
            mapBlocks.Remove(firstMapBlock);
            firstMapBlock.transform.position = lastMapBlock.transform.position + new Vector3(8, 0, 0);
            firstMapBlock.Initialize();
            mapBlocks.Add(firstMapBlock);
            m_cinemachineConfiner.InvalidatePathCache();
        }
        else if (playerPosition <= m_playerPosition - 8)
        {
            m_playerPosition -= 8;
            var firstMapBlock = mapBlocks[0];
            var lastMapBlock = mapBlocks[^1];
            mapBlocks.Remove(lastMapBlock);
            lastMapBlock.transform.position = firstMapBlock.transform.position - new Vector3(8, 0, 0);
            lastMapBlock.Initialize();
            mapBlocks.Insert(0, lastMapBlock);
            m_cinemachineConfiner.InvalidatePathCache();
        }
    }
}
