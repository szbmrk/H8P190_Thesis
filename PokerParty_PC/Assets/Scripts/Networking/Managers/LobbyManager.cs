using PokerParty_SharedDLL;
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
        Debug.Log("Lobby deleted");
    }

    public bool CheckIfPlayerNameIsAlreadyInUse(Player player, int indexInConnectionsArray)
    {
        if (!joinedPlayers.Any(p => p.assignedPlayer.PlayerName == player.PlayerName)) return false;
        ConnectionManager.instance.SendMessageToConnection(ConnectionManager.instance.Connections[indexInConnectionsArray], new PlayerNameAlreadyInUseMessage());
        ConnectionManager.instance.DisconnectPlayer(indexInConnectionsArray);
        return true;

    }
    
    public void AddPlayer(Player player, int indexInConnectionsArray)
    {
        LobbyPlayerCard newPlayer = LobbyGUI.instance.DisplayNewPlayer(player);
        newPlayer.indexInConnectionsArray = indexInConnectionsArray;
        joinedPlayers.Add(newPlayer);
    }

    public Player GetPlayerByIndexInConnectionsArray(int indexInConnectionsArray)
    {
        return joinedPlayers.Find(p => p.indexInConnectionsArray == indexInConnectionsArray).assignedPlayer;
    }
    
    public void RemovePlayer(Player player)
    {
        LobbyGUI.instance.RemovePlayerFromDisplay(player);
        joinedPlayers.Remove(GetPlayerCardForPlayer(player));
    }

    public void ModifyPlayerReady(ReadyMessage readyMessage)
    {
        GetPlayerCardForPlayer(readyMessage.Player).SetReady(readyMessage.IsReady);
        LobbyGUI.instance.RefreshPlayerCount();
    }

    public LobbyPlayerCard GetPlayerCardForPlayer(Player player)
    {
        Debug.Log(joinedPlayers.Find(p => p.assignedPlayer.Equals(player)).assignedPlayer);
        return joinedPlayers.Find(p => p.assignedPlayer.Equals(player));
    }

    public bool AreAllPlayersReady()
    {
        return joinedPlayers.All(player => player.isReady);
    }
}
