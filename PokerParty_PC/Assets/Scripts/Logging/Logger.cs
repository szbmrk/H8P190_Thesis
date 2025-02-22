using System;
using System.IO;
using UnityEngine;

public static class Logger
{
    private static readonly string LOGFilePath = Path.Combine(Application.persistentDataPath, "game_log.txt");

    public static void Log(string message)
    {
        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}";

#if UNITY_EDITOR
        Debug.Log(logEntry);
#else
        try
        {
            File.AppendAllText(LOGFilePath, logEntry + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to write log: {ex.Message}");
        }
#endif
    }
}