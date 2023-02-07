using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIView : MonoBehaviour
{
    [SerializeField] private Selectable m_defaultSelectable;
    [SerializeField] protected Selectable m_currentSelectable = null;
    [SerializeField] protected UIView m_previousView = null;

    public UIView PreviousView { get => m_previousView; set => m_previousView = value; }
    public Selectable CurrentSelectable { get => m_currentSelectable; set => m_currentSelectable = value; }

    public virtual void Start()
    {
        OnHide();
    }

    public virtual void OnShow()
    {
        if (m_currentSelectable != null)
        {
            EventSystem.current.SetSelectedGameObject(m_currentSelectable.gameObject);
        }
        else if (m_defaultSelectable != null)
        {
            EventSystem.current.SetSelectedGameObject(m_defaultSelectable.gameObject);
        }
        gameObject.SetActive(true);
    }

    public virtual void OnHide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateCurrentSelectable(Selectable _selectable)
    {
        m_currentSelectable = _selectable;
    }
}
