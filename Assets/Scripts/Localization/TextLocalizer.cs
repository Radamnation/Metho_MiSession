using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextLocalizer : MonoBehaviour
{
    [SerializeField] private string key;
    private TMP_Text m_text;

    private void Awake()
    {
        m_text = GetComponent<TMP_Text>();
    }

    private void Localize()
    {
        m_text.text = Localization.Instance.GetLocalization(key);
    }

    private void OnEnable()
    {
        Localization.Instance.Subscribe(Localize);
    }

    private void OnDisable()
    {
        Localization.Instance.Unsubscribe(Localize);
    }
}
