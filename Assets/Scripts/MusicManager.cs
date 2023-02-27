using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        Destroy(gameObject);

        musicAudioSource = GetComponent<AudioSource>();
    }

    [SerializeField] private AudioClip mainSong;

    private AudioSource musicAudioSource;

    private void Start()
    {
        musicAudioSource.clip = mainSong;
        musicAudioSource.loop = true;
        musicAudioSource.Play();
    }
}
