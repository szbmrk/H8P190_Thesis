using System.Collections.Generic;
using UnityEngine;

public static class PlayerColorManager
{
    private class ColorForPlayer
    {
        public string username;
        public Color color;
        public ColorForPlayer(Color color)
        {
            username = string.Empty;
            this.color = color;
        }
    }

    private static List<ColorForPlayer> ColorForPlayers = new List<ColorForPlayer>
    {
        new ColorForPlayer(new Color(0.729f, 1.0f, 0.961f)),
        new ColorForPlayer(new Color(1.0f, 0.973f, 0.729f)),
        new ColorForPlayer(new Color(0.749f, 0.729f, 1.0f)),
        new ColorForPlayer(new Color(1.0f, 0.365f, 0.475f)),
        new ColorForPlayer(new Color(0.412f, 0.463f, 1.0f)),
        new ColorForPlayer(new Color(0.749f, 1.0f, 0.412f)),
        new ColorForPlayer(new Color(0.984f, 0.506f, 1.0f)),
        new ColorForPlayer(new Color(0.812f, 0.553f, 0.38f))
    };

    public static Color GetColor(string username)
    {
        foreach (ColorForPlayer colorForPlayer in ColorForPlayers) 
        {
            if (colorForPlayer.username == username) return colorForPlayer.color;
        } 

        while (true)
        {
            System.Random rnd = new System.Random();
            int randomIndex = rnd.Next(1, 8);

            if (ColorForPlayers[randomIndex].username == "")
            {
                ColorForPlayers[randomIndex].username = username;
                return ColorForPlayers[randomIndex].color;
            }
        }
    }

    public static void RemoveColorFromPlayer(string username)
    {
        foreach (ColorForPlayer colorForPlayer in ColorForPlayers)
        {
            if (colorForPlayer.username == username) colorForPlayer.username = "";
        }
    }
}
