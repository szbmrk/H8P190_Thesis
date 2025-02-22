using System.Collections;
using UnityEngine;

public class OnAppQuit : MonoBehaviour
{
    private static OnAppQuit instance;
    private static bool readyToQuit;

    [RuntimeInitializeOnLoadMethod]
    private static void RunOnStart()
    {
        Application.wantsToQuit += WantsToQuit;
        Application.quitting += () => instance.StartCoroutine(instance.StartQuiting());
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
        Logger.Log(Application.persistentDataPath);
        Logger.Log("Started quiting");
        if (ConnectionManager.instance.NetworkDriver.IsCreated)
        {
            ConnectionManager.instance.StopAllCoroutines();
            ConnectionManager.instance.DisconnectFromHost();
            yield return ConnectionManager.instance.DisposeNetworkDriver();
        }

        readyToQuit = true;
        Logger.Log("Client app stopped");
        Application.Quit();
    }
}
