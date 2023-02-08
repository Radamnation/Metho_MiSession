using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXLightningStrike : PoolableObject
{
    private LineRenderer m_lineRenderer;
    private ParticleSystem m_particleSystem;

    private float m_lightningSpeed;
    private float m_lightningTimer;

    private void Awake()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
        m_particleSystem = GetComponent<ParticleSystem>();
    }

    public override void Initialize()
    {
        m_lightningSpeed = m_particleSystem.main.duration;
        m_lightningTimer = 0;
        m_particleSystem.Play();
        m_lineRenderer.SetPosition(0, new Vector3(0, 0));
        for (int i = 1; i < m_lineRenderer.positionCount; i++)
        {
            m_lineRenderer.SetPosition(i, new Vector3(Random.Range(-0.5f, 0.5f), i));
        }
        m_lineRenderer.startColor = Color.clear;
    }

    private void Update()
    {
        m_lightningTimer += Time.deltaTime;
        if (m_lightningTimer < m_lightningSpeed / 2)
        {
            m_lineRenderer.startColor = Color.Lerp(Color.clear, Color.white, m_lightningTimer / (m_lightningSpeed / 2));
        }
        else if (m_lightningTimer > m_lightningSpeed / 2 && m_lightningTimer < m_lightningSpeed)
        {
            m_lineRenderer.startColor = Color.Lerp(Color.clear, Color.white, m_lightningTimer / (m_lightningSpeed / 2));
        }
        else
        {
            m_lightningTimer = 0;
            Repool();
        }
    }
}
