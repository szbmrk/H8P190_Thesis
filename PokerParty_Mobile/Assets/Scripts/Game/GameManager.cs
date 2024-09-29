using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PokerParty_SharedDLL;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        //Loader.Instance.StartLoading();
        //MessageSender.SendMessageToHost(new LoadedToGameMessage());
    }

    public void EveryOneLoaded()
    {
        Loader.Instance.StopLoading();
    }
}
