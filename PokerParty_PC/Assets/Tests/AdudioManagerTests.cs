using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class AudioManagerTests
{
    private GameObject audioManagerObject;
    private AudioManager audioManager;
    
    [SetUp]
    public void Setup()
    {
        audioManagerObject = new GameObject("AudioManager");
        audioManager = audioManagerObject.AddComponent<AudioManager>();
        AudioManager.instance = audioManager;
    }

    [Test]
    public void SetVolumes_UpdatesAllSFXVolumes()
    {
        // Arrange
        var sfx1 = audioManagerObject.AddComponent<AudioSource>();
        var sfx2 = audioManagerObject.AddComponent<AudioSource>();
        audioManager.sfxSources = new List<AudioSource> { sfx1, sfx2 };
        float expectedVolume = 0.5f;
        
        // Act
        audioManager.SetVolumes(expectedVolume, 1.0f);
        
        // Assert
        Assert.AreEqual(expectedVolume, sfx1.volume);
        Assert.AreEqual(expectedVolume, sfx2.volume);
    }

    [Test]
    public void SetVolumes_UpdatesAllMusicVolumes()
    {
        // Arrange
        var music1 = audioManagerObject.AddComponent<AudioSource>();
        var music2 = audioManagerObject.AddComponent<AudioSource>();
        audioManager.musicSources = new List<AudioSource> { music1, music2 };
        float expectedVolume = 0.3f;
        
        // Act
        audioManager.SetVolumes(1.0f, expectedVolume);
        
        // Assert
        Assert.AreEqual(expectedVolume, music1.volume);
        Assert.AreEqual(expectedVolume, music2.volume);
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(audioManagerObject);
    }
}