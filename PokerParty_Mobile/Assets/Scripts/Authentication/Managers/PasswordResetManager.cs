using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class PasswordResetManager
{
    [Serializable]
    private class EmailData
    {
        public string email;
    }

    [Serializable]
    private class PlayerNameData
    {
        public string playerName;
    }

    public static async Task<string> SendPasswordResetEmail(string emailOrPlayerName)
    {
        if (string.IsNullOrEmpty(emailOrPlayerName))
            throw new Exception("Email or playerName is empty!");

        bool isEmail = false;

        if (emailOrPlayerName.Contains("@"))
            isEmail = true;
        else
            isEmail = false;

        if (isEmail && !EmailValidator.IsValidEmail(emailOrPlayerName))
            throw new Exception("Invalid email adress!");

        object resetPasswordData = null;
        if (isEmail)
        {
            resetPasswordData = new EmailData
            {
                email = emailOrPlayerName
            };
        }
        else
        {
            resetPasswordData = new PlayerNameData
            {
                playerName = emailOrPlayerName
            };
        }

        UnityWebRequest response = await RequestManager.SendPostRequest("/reset-password", JsonUtility.ToJson(resetPasswordData));
        SimpleMessageResponse resetPasswordResponse = JsonUtility.FromJson<SimpleMessageResponse>(response.downloadHandler.text);

        if (response.responseCode != 200L)
        {
            throw new Exception(resetPasswordResponse.msg);
        }
        
        return resetPasswordResponse.msg;
    }
}
