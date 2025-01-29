using System.Collections.Generic;
using UnityEngine;

public static class PlayerColorManager
{
    private class ColorForPlayer
    {
        public string PlayerName;
        public readonly Color Color;
        public ColorForPlayer(Color color)
        {
            PlayerName = string.Empty;
            this.Color = color;
        }
    }

    private static readonly List<ColorForPlayer> ColorForPlayers = new List<ColorForPlayer>
    {
        new ColorForPlayer(new Color(0.729f, 1.0f, 0.961f)),
        new ColorForPlayer(new Color(1.0f, 0.973f, 0.729f)),
        new ColorForPlayer(new Color(0.749f, 0.729f, 1.0f)),
        new ColorForPlayer(new Color(1.0f, 0.365f, 0.475f)),
        new ColorForPlayer(new Color(0.412f, 0.463f, 1.0f)),
        new ColorForPlayer(new Color(0.749f, 1.0f, 0.412f)),
        new ColorForPlayer(new Color(0.984f, 0.506f, 1.0f)),
        new ColorForPlayer(new Color(0.812f, 0.553f, 0.38f)),
    };

    public static Color GetColor(string playerName)
    {
        foreach (ColorForPlayer colorForPlayer in ColorForPlayers) 
        {
            if (colorForPlayer.PlayerName == playerName) return colorForPlayer.Color;
        } 

        while (true)
        {
            System.Random rnd = new System.Random();
            int randomIndex = rnd.Next(1, ColorForPlayers.Count);

            if (ColorForPlayers[randomIndex].PlayerName == string.Empty)
            {
                ColorForPlayers[randomIndex].PlayerName = playerName;
                return ColorForPlayers[randomIndex].Color;
            }
        }
    }

    public static void RemoveColorFromPlayer(string playerName)
    {
        foreach (ColorForPlayer colorForPlayer in ColorForPlayers)
        {
            if (colorForPlayer.PlayerName == playerName) colorForPlayer.PlayerName = string.Empty;
        }
    }

    public static void ResetColors()
    {
        foreach (ColorForPlayer colorForPlayer in ColorForPlayers)
        {
            colorForPlayer.PlayerName = string.Empty;
        }
    }
}
