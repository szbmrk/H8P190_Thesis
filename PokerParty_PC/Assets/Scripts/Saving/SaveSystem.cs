using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private static readonly string SavePath = Application.persistentDataPath + "/saves/settings.save";
    
    public static void SaveSettings(int qualityIndex, int resolutionIndex, int screenModeIndex,int languageModeIndex, float mainVolumeIndex, float musicVolumeIndex)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(SavePath, FileMode.Create);

        SettingsData data = new SettingsData(qualityIndex, resolutionIndex, screenModeIndex, languageModeIndex, mainVolumeIndex, musicVolumeIndex);

        bf.Serialize(stream, data);
        stream.Close();
    }
    
    public static SettingsData LoadSettings()
    {
        if (!File.Exists(SavePath)) return null;
        
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(SavePath, FileMode.Open);

        SettingsData data = bf.Deserialize(stream) as SettingsData;
        stream.Close();

        return data;

    }
}