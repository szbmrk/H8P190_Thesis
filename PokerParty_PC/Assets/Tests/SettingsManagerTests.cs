using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManagerTests
{
    private GameObject settingsManagerObject;
    private SettingsManager settingsManager;
    
    [SetUp]
    public void Setup()
    {
        settingsManagerObject = new GameObject("SettingsManager");
        settingsManager = settingsManagerObject.AddComponent<SettingsManager>();
        settingsManagerObject.AddComponent<AudioManager>();
        
        SettingsManager.instance = settingsManager;

        // Mocking UI Elements
        settingsManager.qualityDropDown = new GameObject("QualityDropdown").AddComponent<TMP_Dropdown>();
        settingsManager.resolutionDropDown = new GameObject("ResolutionDropdown").AddComponent<TMP_Dropdown>();
        settingsManager.screenModeToggle = new GameObject("ScreenModeToggle").AddComponent<Toggle>();
        settingsManager.sfxVolumeSlider = new GameObject("SFXVolumeSlider").AddComponent<Slider>();
        settingsManager.musicVolumeSlider = new GameObject("MusicVolumeSlider").AddComponent<Slider>();
    }

    [Test]
    public void FillResolutionsDropdown_AddsResolutionOptions()
    {
        // Act
        settingsManager.FillResolutionsDropdown();
        
        // Assert
        Assert.Greater(settingsManager.resolutionDropDown.options.Count, 0);
    }
    
    [Test]
    public void FillQualityDropdown_AddsQualityOptions()
    {
        // Act
        settingsManager.FillQualityDropdown();
        
        // Assert
        Assert.Greater(settingsManager.qualityDropDown.options.Count, 0);
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(settingsManagerObject);
    }
}
