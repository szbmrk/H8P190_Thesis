using PokerParty_SharedDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
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

    public static System.Random rnd = new System.Random();

    private static List<ColorForPlayer> ColorForPlayers = new List<ColorForPlayer>
    {
        new ColorForPlayer(new Color(186, 255, 245)),
        new ColorForPlayer(new Color(255, 248, 186)),
        new ColorForPlayer(new Color(191, 186, 255)),
        new ColorForPlayer(new Color(255, 93, 121)),
        new ColorForPlayer(new Color(105, 118, 255)),
        new ColorForPlayer(new Color(191, 255, 105)),
        new ColorForPlayer(new Color(251, 129, 255)),
        new ColorForPlayer(new Color(207, 141, 97))
    };

    public static Color GetColor(string username)
    {
        foreach (ColorForPlayer colorForPlayer in ColorForPlayers) 
        {
            if (colorForPlayer.username == username) return colorForPlayer.color;
        } 

        while (true)
        {
            int randomIndex = rnd.Next(1, 8);

            if (ColorForPlayers[randomIndex].username == "")
            {
                ColorForPlayers[randomIndex].username = username;
                return ColorForPlayers[randomIndex].color;
            }
        }
    }
}
