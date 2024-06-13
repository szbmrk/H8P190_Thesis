using PokerParty_SharedDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    public List<PlayerCard> joinedPlayers = new List<PlayerCard>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void DisconnectAllPlayers()
    {
        RelayManager.Instance.DisconnectAllPlayers();
        LobbyGUI.Instance.ClearDisplay();
    }

    public void DeleteLobby()
    {
        RelayManager.Instance.DeleteLobbyAndDisposeNetworkDriver();
        LobbyGUI.Instance.HidePanel();
        LobbyGUI.Instance.joinCodeText.text = string.Empty;
    }

    public void AddPlayer(Player player)
    {
        joinedPlayers.Add(LobbyGUI.Instance.DisplayNewPlayer(player));
    }

    public void RemovePlayer(Player player)
    {
        joinedPlayers.Remove(joinedPlayers.Find(p => p.assignedPlayer.Equals(player)));
        LobbyGUI.Instance.RemovePlayerFromDisplay(player);
    }

    public void ModifyPlayerReady(ReadyMessage readyMessage)
    {
        GetPlayerCardForPlayer(readyMessage.player).SetReady(readyMessage.isReady);
    }

    public PlayerCard GetPlayerCardForPlayer(Player player)
    {
        foreach (PlayerCard playerCard in joinedPlayers)
        {
            if (playerCard.assignedPlayer.Equals(player))
            {
                return playerCard;
            }
        }

        return null;
    }

    public bool AreAllPlayersReady()
    {
        return joinedPlayers.All(player => player.IsReady);
    }
}
