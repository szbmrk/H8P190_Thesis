using PokerParty_SharedDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class NetworkMessageManager
{
    public static void ProcessMesage(NetworkMessageType type, string data)
    {
        switch (type)
        {
            case NetworkMessageType.GameStartedMessage:
                SceneManager.LoadScene("Game");
                break;
            case NetworkMessageType.EveryoneLoadedMessage:
                GameManager.Instance.EveryOneLoaded();
                break;
        }
    }

    private static T FromStringToJson<T>(string message)
    {
        return JsonUtility.FromJson<T>(message);
    }
}
