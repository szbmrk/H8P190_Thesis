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
        if (Instance == null || RelayManager.Instance == null || !RelayManager.Instance.networkDriver.IsCreated)
            return true;

        Instance.StartCoroutine(Instance.StartQuiting());

        return ReadyToQuit;
    }

    IEnumerator StartQuiting()
    {
        RelayManager.Instance.DisconnectFromHost();

        RelayManager.Instance.StopAllCoroutines();
        yield return RelayManager.Instance.DisposeNetworkDriver();

        ReadyToQuit = true;
        Debug.Log("Client app stopped");
        Application.Quit();
    }
}
