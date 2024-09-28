using PokerParty_SharedDLL;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public IEnumerator DeleteLobby()
    {
        ConnectionManager.Instance.DisconnectAllPlayers();
        yield return ConnectionManager.Instance.DisposeDriverAndConnections();
        Destroy(ConnectionManager.Instance.gameObject);
        Debug.Log("Lobby deleted");
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
