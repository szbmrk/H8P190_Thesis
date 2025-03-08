using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour 
{
    public static AudioManager instance;
    
    public List<AudioSource> sfxSources = new List<AudioSource>();
    public List<AudioSource> musicSources = new List<AudioSource>();
    
    public AudioSource menuClickSource;
    
    public AudioSource readySource;
    public AudioSource chatMessageSource;
    public AudioSource disconnectedSource;
    public AudioSource playerJoinedSource;
    public AudioSource lobbyLoadedSource;
    
    public AudioSource cardDealingSource;
    public AudioSource cardFlippingSource;
    public AudioSource pokerChipSource;
    public AudioSource checkSource;
    public AudioSource newRoundStartedSource;
    public AudioSource gameOverSource;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        SettingsData settings = SaveSystem.LoadSettings();
        
        if (settings != null)
        {
            SetVolumes(settings.sfxVolumeValue, settings.musicVolumeValue);
        }
    }

    public void SetVolumes(float sfxVolume, float musicVolume)
    {
        foreach (AudioSource sfxSource in sfxSources)
        {
            sfxSource.volume = sfxVolume;
        }
        
        foreach (AudioSource musicSource in musicSources)
        {
            musicSource.volume = musicVolume;
        }
    }
    
    public void PlayMenuClick()
    {
        menuClickSource.Play();
    }
}
