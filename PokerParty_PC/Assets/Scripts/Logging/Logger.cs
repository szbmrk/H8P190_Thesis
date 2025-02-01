using System;
using System.IO;
using UnityEngine;

public static class Logger
{
    private static readonly string LOGFilePath = Path.Combine(Application.persistentDataPath, "game_log.txt");

    public static void LogToFile(string message)
    {
        try
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}";
            File.AppendAllText(LOGFilePath, logEntry + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to write log: {ex.Message}");
        }
    }
}