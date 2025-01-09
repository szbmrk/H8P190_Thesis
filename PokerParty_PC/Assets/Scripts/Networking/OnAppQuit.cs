using System.Collections;
using UnityEngine;

public class OnAppQuit : MonoBehaviour 
{
    private static bool _readyToQuit;

    private static OnAppQuit _instance; 


    [RuntimeInitializeOnLoadMethod]
    private static void RunOnStart()
    {
        Application.wantsToQuit += WantsToQuit;
    }

    private void Awake()
    {
        _instance = this;
    }

    private static bool WantsToQuit()
    {
        if (_instance == null || ConnectionManager.instance == null || !ConnectionManager.instance.NetworkDriver.IsCreated)
            return true;

        _instance.StartCoroutine(_instance.StartQuiting());

        return _readyToQuit;
    }

    IEnumerator StartQuiting()
    {
        if (ConnectionManager.instance != null)
            ConnectionManager.instance.StopAllCoroutines();

        if (LobbyManager.instance != null)
            LobbyManager.instance.StopAllCoroutines();

        if (LobbyGUI.instance != null)
            yield return LobbyGUI.instance.DeleteLobby();
        else
        {
            ConnectionManager.instance.DisconnectAllPlayers();
            yield return ConnectionManager.instance.DisposeDriverAndConnections();
        }

        _readyToQuit = true;
        Debug.Log("Server app stopped");
        Application.Quit();
    }
}
