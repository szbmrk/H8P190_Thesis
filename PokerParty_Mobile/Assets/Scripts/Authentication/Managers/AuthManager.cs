using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance;

    [Serializable]
    private class PostData
    {
        public string username;
        public string password;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public async Task Register(string username, string password)
    {
        PostData registerData = new PostData
        {
            username = username,
            password = password
        };

        UnityWebRequest response = await RequestManager.SendPostRequest("/register", JsonUtility.ToJson(registerData));
        SimpleMessageResponse registerResponse = JsonUtility.FromJson<SimpleMessageResponse>(response.downloadHandler.text);

        if (response.responseCode != 201L)
        {
            throw new InvalidRegisterException(registerResponse.msg);
        }
    }

    public async Task Login(string username, string password)
    {
        PostData loginData = new PostData
        {
            username = username,
            password = password
        };

        UnityWebRequest response = await RequestManager.SendPostRequest("/login", JsonUtility.ToJson(loginData));
        LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(response.downloadHandler.text);

        if (response.responseCode != 200L)
        {
            throw new InvalidLoginException(loginResponse.msg);
        }

        UserManager.LoggedInUser = loginResponse.user;
    }
}
