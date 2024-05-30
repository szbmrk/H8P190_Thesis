using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    private string baseUrl = "http://localhost:5000/api";

    [System.Serializable]
    public class AuthData
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class AuthResponse
    {
        public string msg;
        public User user;
    }

    public async Task Register(string username, string password)
    {
        AuthData registerData = new AuthData
        {
            username = username,
            password = password
        };

        string response = await SendPostRequest($"{baseUrl}/register", JsonUtility.ToJson(registerData));
        Debug.Log("Register Response: " + response);
    }

    public async Task Login(string username, string password)
    {
        AuthData loginData = new AuthData
        {
            username = username,
            password = password
        };

        string response = await SendPostRequest($"{baseUrl}/login", JsonUtility.ToJson(loginData));
        AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(response);
        UserManager.LoggedInUser = authResponse.user;
        Debug.Log("Login Response: " + authResponse.msg);
        Debug.Log("Logged in: " + authResponse.user);
    }

    private async Task<string> SendPostRequest(string url, string json)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                return null;
            }

            return request.downloadHandler.text;
        }
    }
}
