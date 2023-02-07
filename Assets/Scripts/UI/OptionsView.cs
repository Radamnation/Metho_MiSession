using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OptionsView : UIView
{
    [SerializeField] private Slider m_masterVolumeSlider;
    private float m_masterVolume;

    public override void Start()
    {
        base.Start();
        LoadOptions();
    }

    public override void OnShow()
    {
        base.OnShow();
    }

    private void LoadOptions()
    {
        m_masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.7f);
        m_masterVolumeSlider.value = m_masterVolume;
    }

    public void SaveOptions()
    {
        PlayerPrefs.SetFloat("MasterVolume", m_masterVolume);
        PlayerPrefs.Save();
    }

    public void UpdateMasterVolume(float _volume)
    {
        m_masterVolume = _volume;
    }
}
