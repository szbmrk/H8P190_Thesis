using System;
using PokerParty_SharedDLL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkingGUI : MonoBehaviour
{
    public static NetworkingGUI instance;

    public Button joinBtn;
    [SerializeField] private Button sendMsgBtn;
    [SerializeField] private Button disconnectBtn;
    [SerializeField] private Button readyBtn;

    private bool isReady;

    [SerializeField] private GameObject notJoinedPanel;
    [SerializeField] private GameObject joinedPanel;

    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField messageInput;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        joinBtn.onClick.AddListener(JoinRelay);
        disconnectBtn.onClick.AddListener(DisconnectFromHost);
        sendMsgBtn.onClick.AddListener(SendChatMessageToHost);
        readyBtn.onClick.AddListener(Ready);
    }

    private void Start()
    {
        playerNameInputField.text = PlayerPrefs.GetString("playerName", string.Empty);
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

        Loader.instance.StopLoading();
        messageInput.text = string.Empty;
        joinedPanel.SetActive(value);
        notJoinedPanel.SetActive(!value);
    }

    private void SendChatMessageToHost()
    {
        if (string.IsNullOrEmpty(messageInput.text))
        {
            PopupManager.instance.ShowPopup(PopupType.ErrorPopup, "Cannot send empty message");
            return;
        }

        if (messageInput.text.Length > 500)
        {
            PopupManager.instance.ShowPopup(PopupType.ErrorPopup, "Cannot send more than 500 characters at once");
            return;
        }

        ChatMessage chatMessage = new ChatMessage() { Message = messageInput.text };
        MessageSender.SendMessageToHost(chatMessage);
        messageInput.text = string.Empty;
    }

    private void JoinRelay()
    {
        Loader.instance.StartLoading();
        joinBtn.interactable = false;
        
        if (string.IsNullOrEmpty(playerNameInputField.text))
        {
            ShowJoinError("Player name is empty");
            return;
        }
        
        PlayerManager.loggedInPlayer = new Player(playerNameInputField.text);
        ConnectionManager.instance.JoinRelay(joinCodeInputField.text);
        joinCodeInputField.text = string.Empty;
    }

    public void ShowJoinError(string error)
    {
        Loader.instance.StopLoading();
        joinBtn.interactable = true;
        PopupManager.instance.ShowPopup(PopupType.ErrorPopup, error);
    }

    private void DisconnectFromHost()
    {
        Loader.instance.StartLoading();
        joinBtn.interactable = true;
        disconnectBtn.interactable = false;
        ConnectionManager.instance.DisconnectFromHost();
        ShowJoinedPanel(false);
        ResetReadyButton();
    }

    private void Ready()
    {
        if (isReady)
        {
            readyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
            isReady = false;
            disconnectBtn.interactable = true;
        }
        else
        {
            readyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Not Ready";
            isReady = true;
            disconnectBtn.interactable = false;
        }

        ReadyMessage message = new ReadyMessage() { IsReady = isReady };
        MessageSender.SendMessageToHost(message);
    }

    public void ResetReadyButton()
    {
        isReady = false;
        readyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
        disconnectBtn.interactable = true;
    }
}
