using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowArrow : MonoBehaviour
{
    private SpriteRenderer m_spriteRenderer;

    private void Start()
    {
        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (EnemyChecker.Instance.SortedEnemy.Count > 0)
        {
            m_spriteRenderer.enabled = true;
            if ((transform.position - EnemyChecker.Instance.SortedEnemy.Max.transform.position).magnitude > 0.5f)
            {
                transform.position = Vector3.Lerp(transform.position, EnemyChecker.Instance.SortedEnemy.Max.transform.position, 0.02f);
            }
            else
            {
                transform.position = EnemyChecker.Instance.SortedEnemy.Max.transform.position;
            }
            return;
        }

        if ((transform.position - Player.Instance.transform.position).magnitude > 0.5f)
        {
            transform.position = Vector3.Lerp(transform.position, Player.Instance.transform.position, 0.02f);
        }
        else
        {
            transform.position = Player.Instance.transform.position;
        }
    }
}
