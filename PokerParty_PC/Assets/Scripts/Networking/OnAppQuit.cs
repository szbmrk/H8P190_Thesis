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
        RelayManager.Instance.DeleteLobby();

        RelayManager.Instance.StopAllCoroutines();

        yield return RelayManager.Instance.DisposeDriver();
        yield return RelayManager.Instance.DisposeConnections();

        ReadyToQuit = true;
        Debug.Log("Server app stopped");
        Application.Quit();
    }
}
