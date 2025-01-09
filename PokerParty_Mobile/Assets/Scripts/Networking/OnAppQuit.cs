using System.Collections;
using UnityEngine;

public class OnAppQuit : MonoBehaviour
{
    private static bool readyToQuit;

    private static OnAppQuit instance;

    [RuntimeInitializeOnLoadMethod]
    static void RunOnStart()
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

    IEnumerator StartQuiting()
    {
        if (ConnectionManager.instance != null)
        {
            ConnectionManager.instance.DisconnectFromHost();
            ConnectionManager.instance.StopAllCoroutines();
            yield return ConnectionManager.instance.DisposeNetworkDriver();
        }

        readyToQuit = true;
        Debug.Log("Client app stopped");
        Application.Quit();
    }
}
