using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }

    [SerializeField] private AudioClip mainSong;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    
    public AudioSource SfxAudioSource => sfxAudioSource;

    private void Start()
    {
        musicAudioSource.clip = mainSong;
        musicAudioSource.loop = true;
        musicAudioSource.Play();
    }
}
