using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class NetworkingGUI : MonoBehaviour
{
    public static NetworkingGUI Instance;

    [SerializeField] private PlayerCard playerCard;
    
    public Button joinBtn;
    [SerializeField] private Button sendTestMsgBtn;
    [SerializeField] private Button disconnectBtn;

    [SerializeField] private GameObject notJoinedPanel;
    [SerializeField] private GameObject joinedPanel;

    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField messageInput;

    [SerializeField] private TextMeshProUGUI errorText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        playerCard.DisplayData(PlayerManager.LoggedInPlayer);

        joinBtn.onClick.AddListener(JoinRelay);
        disconnectBtn.onClick.AddListener(DisconnectFromHost);
        sendTestMsgBtn.onClick.AddListener(SendChatMessageToHost);
    }

    public void ShowJoinedPanel(bool value)
    {
        if (value)
        {
            joinBtn.interactable = false;
            disconnectBtn.interactable = true;
        }
        else
        {
            joinBtn.interactable = true;
            disconnectBtn.interactable = false;
        }

        joinedPanel.SetActive(value);
        notJoinedPanel.SetActive(!value);
    }

    private void SendChatMessageToHost()
    {
        if (string.IsNullOrEmpty(messageInput.text))
        {
            Debug.LogError("Cannot send empty message!");
            StartCoroutine(DisplayErrorText("Cannot send empty message!"));
            return;
        }

        if (messageInput.text.Length > 500)
        {
            Debug.LogError("Cannot send more than 500 characters at once!");
            StartCoroutine(DisplayErrorText("Cannot send more than 500 characters at once!"));
        }

        RelayManager.Instance.SendChatMessageToHost(messageInput.text);
        messageInput.text = string.Empty;
    }

    private void JoinRelay()
    {
        joinBtn.interactable = false;
        RelayManager.Instance.JoinRelay(joinCodeInputField.text);
        joinCodeInputField.text = string.Empty;
    }

    private void DisconnectFromHost()
    {
        joinBtn.interactable = true;
        disconnectBtn.interactable = false;
        RelayManager.Instance.DisconnectFromHost();
    }

    public IEnumerator DisplayErrorText(string error)
    {
        errorText.text = error;
        yield return new WaitForSeconds(5f);
        errorText.text = string.Empty;
    }
}
