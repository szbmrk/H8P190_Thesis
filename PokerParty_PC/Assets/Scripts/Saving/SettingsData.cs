[System.Serializable]
public class SettingsData 
{
    public int qualityIndex;
    public int resolutionIndex;
    public int screenModeIndex;
    public int languageModeIndex;
    public float mainVolumeValue;
    public float musicVolumeValue;
    public SettingsData(int qualityIndex, int resolutionIndex, int screenModeIndex, int languageModeIndex, float mainVolumeValue, float musicVolumeValue)
    {
        this.qualityIndex = qualityIndex;
        this.resolutionIndex = resolutionIndex;
        this.screenModeIndex = screenModeIndex;
        this.languageModeIndex = languageModeIndex;
        this.mainVolumeValue = mainVolumeValue;
        this.musicVolumeValue = musicVolumeValue;
    }
}