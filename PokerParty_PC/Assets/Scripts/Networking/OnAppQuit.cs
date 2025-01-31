using System.Collections;
using UnityEngine;

public class OnAppQuit : MonoBehaviour 
{
    private static bool readyToQuit;

    private static OnAppQuit instance; 


    [RuntimeInitializeOnLoadMethod]
    private static void RunOnStart()
    {
        Application.wantsToQuit += WantsToQuit;
    }

    private void Awake()
    {
        instance = this;
    }

    private static bool WantsToQuit()
    {
        if (instance == null || ConnectionManager.instance == null || !ConnectionManager.instance.NetworkDriver.IsCreated)
            return true;

        instance.StartCoroutine(instance.StartQuiting());

        return readyToQuit;
    }

    private IEnumerator StartQuiting()
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

        readyToQuit = true;
        Debug.Log("Server app stopped");
        Application.Quit();
    }
}
