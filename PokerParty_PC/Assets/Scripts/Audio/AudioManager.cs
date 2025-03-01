using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour 
{
    public static AudioManager instance;
    public List<AudioSource> sfxSources;
    public List<AudioSource> musicSources;
    
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
    
}
