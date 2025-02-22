using System.Collections;
using UnityEngine;

public class OnAppQuit : MonoBehaviour 
{
    private static OnAppQuit _instance;
    private static bool _readyToQuit;

    [RuntimeInitializeOnLoadMethod]
    private static void RunOnStart()
    {
        Application.wantsToQuit += WantsToQuit;
        Application.quitting += () => _instance.StartCoroutine(_instance.StartQuiting());
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

    private IEnumerator StartQuiting()
    {
        Logger.Log("Started quiting");
        if (ConnectionManager.instance != null)
            ConnectionManager.instance.StopAllCoroutines();

        if (LobbyManager.instance != null)
            LobbyManager.instance.StopAllCoroutines();

        if (LobbyGUI.instance != null)
            yield return LobbyGUI.instance.DeleteLobby();
        else if (ConnectionManager.instance != null)
        {
            ConnectionManager.instance.DisconnectAllPlayers();
            yield return ConnectionManager.instance.DisposeDriverAndConnections();
        }

        _readyToQuit = true;
        Logger.Log("Server app stopped");
        Application.Quit();
    }
}
