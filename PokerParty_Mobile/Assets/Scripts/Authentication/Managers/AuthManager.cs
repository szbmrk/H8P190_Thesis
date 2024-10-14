using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Networking;
using PokerParty_SharedDLL;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance;

    [Serializable]
    private class RegisterData
    {
        public string email;
        public string playerName;
        public string password;
    }

    [Serializable]
    private class LoginData
    {
        public string playerName;
        public string password;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public async Task Register(string email, string playerName, string password)
    {
        RegisterData registerData = new RegisterData
        {
            email = email,
            playerName = playerName,
            password = password
        };

        UnityWebRequest response = await RequestManager.SendPostRequest("/register", JsonUtility.ToJson(registerData));
        SimpleMessageResponse registerResponse = JsonUtility.FromJson<SimpleMessageResponse>(response.downloadHandler.text);

        if (response.responseCode != 201L)
        {
            throw new InvalidRegisterException(registerResponse.msg);
        }
    }

    public async Task Login(string playerName, string password)
    {
        LoginData loginData = new LoginData
        {
            playerName = playerName,
            password = password
        };

        UnityWebRequest response = await RequestManager.SendPostRequest("/login", JsonUtility.ToJson(loginData));
        LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(response.downloadHandler.text);

        if (response.responseCode != 200L)
        {
            throw new InvalidLoginException(loginResponse.msg);
        }

        PlayerManager.LoggedInPlayer = loginResponse.player;
    }
}
