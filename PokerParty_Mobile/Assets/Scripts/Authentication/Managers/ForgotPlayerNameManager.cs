using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;

public static class ForgotPlayerNameManager
{
    [Serializable]
    private class EmailData
    {
        public string email;
    }

    public static async Task<string> SendPlayerNameReminderEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new Exception("Email is empty!");

        if (!EmailValidator.IsValidEmail(email))
            throw new Exception("Invalid email adress!");

        EmailData emailData = new EmailData
        {
                email = email
        };

        UnityWebRequest response = await RequestManager.SendPostRequest("/forgot-player-name", JsonUtility.ToJson(emailData));
        SimpleMessageResponse forgotPlayerNameResponse = JsonUtility.FromJson<SimpleMessageResponse>(response.downloadHandler.text);

        if (response.responseCode != 200L)
        {
            throw new Exception(forgotPlayerNameResponse.msg);
        }

        return forgotPlayerNameResponse.msg;
    }
}
