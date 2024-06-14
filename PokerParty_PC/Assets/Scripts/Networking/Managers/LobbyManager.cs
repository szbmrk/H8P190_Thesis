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
    [HideInInspector] public List<LobbyPlayerCard> joinedPlayers = new List<LobbyPlayerCard>();

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
        RelayManager.Instance.DeleteLobby();
        LobbyGUI.Instance.HidePanel();
        LobbyGUI.Instance.joinCodeText.text = string.Empty;
    }

    public void AddPlayer(Player player, int indexInConnectionsArray)
    {
        LobbyPlayerCard newPlayer = LobbyGUI.Instance.DisplayNewPlayer(player);
        newPlayer.indexInConnectionsArray = indexInConnectionsArray;
        joinedPlayers.Add(newPlayer);
    }

    public void RemovePlayer(Player player)
    {
        LobbyGUI.Instance.RemovePlayerFromDisplay(player);
        joinedPlayers.Remove(GetPlayerCardForPlayer(player));
    }

    public void RemoveAllPlayers()
    {
        LobbyGUI.Instance.ClearDisplay();
        joinedPlayers.Clear();
    }

    public void ModifyPlayerReady(ReadyMessage readyMessage)
    {
        GetPlayerCardForPlayer(readyMessage.player).SetReady(readyMessage.isReady);
    }

    public LobbyPlayerCard GetPlayerCardForPlayer(Player player)
    {
        return joinedPlayers.Find(p => p.assignedPlayer.Equals(player));
    }
    public bool AreAllPlayersReady()
    {
        return joinedPlayers.All(player => player.IsReady);
    }
}
