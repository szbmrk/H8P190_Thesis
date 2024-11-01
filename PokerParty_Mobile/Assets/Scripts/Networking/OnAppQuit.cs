using System.Collections;
using UnityEngine;

public class OnAppQuit : MonoBehaviour
{
    public static bool ReadyToQuit;

    private static OnAppQuit Instance;

    [RuntimeInitializeOnLoadMethod]
    static void RunOnStart()
    {
        Application.wantsToQuit += WantsToQuit;
    }

    private void Awake()
    {
        Instance = this;
    }

    public static bool WantsToQuit()
    {
        if (Instance == null || ConnectionManager.Instance == null || !ConnectionManager.Instance.networkDriver.IsCreated)
            return true;

        Instance.StartCoroutine(Instance.StartQuiting());

        return ReadyToQuit;
    }

    IEnumerator StartQuiting()
    {
        if (ConnectionManager.Instance != null)
        {
            ConnectionManager.Instance.DisconnectFromHost();
            ConnectionManager.Instance.StopAllCoroutines();
            yield return ConnectionManager.Instance.DisposeNetworkDriver();
        }

        ReadyToQuit = true;
        Debug.Log("Client app stopped");
        Application.Quit();
    }
}
