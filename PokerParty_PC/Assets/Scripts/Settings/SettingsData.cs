using UnityEngine.Serialization;

[System.Serializable]
public class SettingsData 
{
    public int qualityIndex;
    public int resolutionIndex;
    public int screenModeIndex;
    public int languageModeIndex;
    public float sfxVolumeValue;
    public float musicVolumeValue;
    public SettingsData(int qualityIndex, int resolutionIndex, int screenModeIndex, int languageModeIndex, float sfxVolumeValue, float musicVolumeValue)
    {
        this.qualityIndex = qualityIndex;
        this.resolutionIndex = resolutionIndex;
        this.screenModeIndex = screenModeIndex;
        this.languageModeIndex = languageModeIndex;
        this.sfxVolumeValue = sfxVolumeValue;
        this.musicVolumeValue = musicVolumeValue;
    }
}