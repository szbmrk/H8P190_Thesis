using PokerParty_SharedDLL;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI ELOText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI gamesPlayedText;

    public void DisplayData(Player player)
    {
        playerNameText.text = player.playerName;
        ELOText.text = $"ELO: {player.ELO}";
        levelText.text = $"Level: {player.level}";
        gamesPlayedText.text = $"Games Played: {player.gamesPlayed}";
    }
}
