using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    
    [HideInInspector] public int qualityIndex;
    public TMP_Dropdown qualityDropDown;

    [HideInInspector] public int resolutionIndex;
    public TMP_Dropdown resolutionDropDown;

    [HideInInspector] public int screenModeIndex;
    public Toggle screenModeToggle;

    [HideInInspector] public int languageModeIndex;
    public TMP_Dropdown languageDropDown;
    
    public Language[] languages ={ Language.Hungarian, Language.English };

    [HideInInspector] public float mainVolumeValue;
    public Slider mainVolumeSlider;

    [HideInInspector] public float musicVolumeValue;
    public Slider musicVolumeSlider;

    private Resolution[] resolutions;
    private RefreshRate maxRefreshRate;

    //public AudioManager audioManager;
    //public TextManager textManager;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        
        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }

        FillResolutionsDropdown();
        FillQualityDropdown();

        LoadSettings();
    }

    private void FillResolutionsDropdown()
    {
        resolutionDropDown.ClearOptions();
        
        resolutions = Screen.resolutions;
        resolutions = resolutions.OrderByDescending(x => x.width).ToArray();
        maxRefreshRate = Screen.currentResolution.refreshRateRatio;
        
        List<string> resOptions = new List<string>();
        int currentResIndex = -1;
        foreach (Resolution res in resolutions)
        {
            if (res.refreshRateRatio.value >= maxRefreshRate.value)
            {
                maxRefreshRate = res.refreshRateRatio;
            }
        }
        
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRateRatio.value == maxRefreshRate.value)
            {
                string option = resolutions[i].width + "x" + resolutions[i].height;
                resOptions.Add(option);
            }

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height && resolutions[i].refreshRateRatio.value == maxRefreshRate.value)
            {
                currentResIndex = i;
            }

        }

        resolutionDropDown.AddOptions(resOptions);
        resolutionIndex = currentResIndex;
        resolutionDropDown.value = currentResIndex;
        resolutionDropDown.RefreshShownValue();
    }
    
    private void FillQualityDropdown()
    {
        List<string> qualityOptions = new List<string>();
        
        foreach (string quality in QualitySettings.names)
        {
            qualityOptions.Add(quality);
        }
        
        qualityDropDown.ClearOptions();
        qualityOptions.Reverse();
        qualityDropDown.AddOptions(qualityOptions);
        qualityDropDown.value = QualitySettings.GetQualityLevel();
        qualityDropDown.RefreshShownValue();
    }

    public void ApplySettings()
    {
        qualityIndex = qualityDropDown.value;
        resolutionIndex = resolutionDropDown.value;
        screenModeIndex = screenModeToggle.isOn ? 1 : 0;
        languageModeIndex = languageDropDown.value;
        mainVolumeValue = mainVolumeSlider.value;
        musicVolumeValue = musicVolumeSlider.value;
        
        SaveSettings();
        LoadSettings();
    }

    private void SaveSettings()
    {
        SaveSystem.SaveSettings(qualityIndex, resolutionIndex, screenModeIndex, languageModeIndex, mainVolumeValue, musicVolumeValue);
    }

    private void LoadSettings()
    {
        SettingsData settingsData = SaveSystem.LoadSettings();
        
        if (settingsData == null) return;
        
        qualityIndex = settingsData.qualityIndex;
        resolutionIndex = settingsData.resolutionIndex;
        mainVolumeValue = settingsData.mainVolumeValue;
        musicVolumeValue = settingsData.musicVolumeValue;
        screenModeIndex = settingsData.screenModeIndex;
        languageModeIndex = settingsData.languageModeIndex;
        QualitySettings.SetQualityLevel(qualityIndex);
        Resolution res = resolutions[resolutionIndex];
        FullScreenMode mode = screenModeIndex == 0 ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow; 
        Screen.SetResolution(res.width, res.height, mode, maxRefreshRate);
        //LocalizationSystem.currentlanguage = languages[settingsData.languageModeIndex];
        LoadSettingStates(settingsData);
    }

    private void LoadSettingStates(SettingsData settingsData)
    {
        qualityDropDown.value = settingsData.qualityIndex;
        resolutionDropDown.value = settingsData.resolutionIndex;
        screenModeToggle.isOn = settingsData.screenModeIndex != 0;
        mainVolumeSlider.value = settingsData.mainVolumeValue;
        musicVolumeSlider.value = settingsData.musicVolumeValue;
        languageDropDown.value = settingsData.languageModeIndex;
        languageDropDown.RefreshShownValue();
        qualityDropDown.RefreshShownValue();
        resolutionDropDown.RefreshShownValue();
    }
}


