﻿using PokerParty_SharedDLL;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;
    [HideInInspector] public List<LobbyPlayerCard> joinedPlayers = new List<LobbyPlayerCard>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public IEnumerator DeleteLobby()
    {
        ConnectionManager.instance.DisconnectAllPlayers();
        PlayerColorManager.ResetColors();
        yield return ConnectionManager.instance.DisposeDriverAndConnections();
        Destroy(ConnectionManager.instance.gameObject);
        Logger.Log("Lobby deleted");
    }

    public bool CheckIfLobbyIsFull(int indexInConnectionsArray)
    {
        if (joinedPlayers.Count < LobbySettings.MaximumPlayers) return false;
        
        ConnectionManager.instance.SendMessageToConnection(ConnectionManager.instance.Connections[indexInConnectionsArray], new LobbyIsFullMessage());
        ConnectionManager.instance.DisconnectPlayer(indexInConnectionsArray);
        return true;

    }
    
    public bool CheckIfPlayerNameIsAlreadyInUse(Player player, int indexInConnectionsArray)
    {
        if (joinedPlayers.All(p => p.assignedPlayer.PlayerName != player.PlayerName)) return false;
        
        ConnectionManager.instance.SendMessageToConnection(ConnectionManager.instance.Connections[indexInConnectionsArray], new PlayerNameAlreadyInUseMessage());
        ConnectionManager.instance.DisconnectPlayer(indexInConnectionsArray);
        return true;

    }
    
    public void AddPlayer(Player player, int indexInConnectionsArray)
    {
        LobbyPlayerCard newPlayer = LobbyGUI.instance.DisplayNewPlayer(player);
        newPlayer.indexInConnectionsArray = indexInConnectionsArray;
        joinedPlayers.Add(newPlayer);
        Logger.Log($"Player {player.PlayerName} joined the lobby");
        LobbyGUI.instance.RefreshPlayerCount();
    }

    public Player GetPlayerByIndexInConnectionsArray(int indexInConnectionsArray)
    {
        return joinedPlayers.Find(p => p.indexInConnectionsArray == indexInConnectionsArray).assignedPlayer;
    }
    
    public void RemovePlayer(Player player)
    {
        LobbyPlayerCard playerCard = GetPlayerCardForPlayer(player);
        joinedPlayers.Remove(playerCard);
        LobbyGUI.instance.RemovePlayerFromDisplay(playerCard);
        Logger.Log($"Player {player.PlayerName} left the lobby");
        LobbyGUI.instance.RefreshPlayerCount();
    }

    public void ModifyPlayerReady(ReadyMessage readyMessage)
    {
        GetPlayerCardForPlayer(readyMessage.Player).SetReady(readyMessage.IsReady);
        LobbyGUI.instance.RefreshPlayerCount();
    }

    private LobbyPlayerCard GetPlayerCardForPlayer(Player player)
    {
        return joinedPlayers.Find(p => p.assignedPlayer.Equals(player));
    }

    public int PlayersReadyCount()
    {
        return joinedPlayers.Count(player => player.isReady);
    }
    
    public bool AreAllPlayersReady()
    {
        return joinedPlayers.All(player => player.isReady);
    }
}
