using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

public static class RequestManager
{
    private static string baseUrl = "https://pokerparty.szobo.dev/api";

    public static async Task<UnityWebRequest> SendPostRequest(string url, string json)
    {
        UnityWebRequest request = new UnityWebRequest(baseUrl + url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        var operation = request.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        return request;
    }
}