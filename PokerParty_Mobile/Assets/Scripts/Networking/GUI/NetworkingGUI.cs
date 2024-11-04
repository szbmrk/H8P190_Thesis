using PokerParty_SharedDLL;
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
    [SerializeField] private Button readyBtn;

    private bool isReady = false;

    [SerializeField] private GameObject notJoinedPanel;
    [SerializeField] private GameObject joinedPanel;

    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField messageInput;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        playerCard.DisplayData(PlayerManager.LoggedInPlayer);

        joinBtn.onClick.AddListener(JoinRelay);
        disconnectBtn.onClick.AddListener(DisconnectFromHost);
        sendTestMsgBtn.onClick.AddListener(SendChatMessageToHost);
        readyBtn.onClick.AddListener(Ready);
    }

    public void ShowJoinedPanel(bool value)
    {
        if (value)
        {
            joinBtn.interactable = false;
            disconnectBtn.interactable = true;
            LogoutGUI.Instance.HideLogoutBtn();
        }
        else
        {
            joinBtn.interactable = true;
            disconnectBtn.interactable = false;
            LogoutGUI.Instance.ShowLogoutBtn();
        }

        Loader.Instance.StopLoading();
        messageInput.text = string.Empty;
        joinedPanel.SetActive(value);
        notJoinedPanel.SetActive(!value);
    }

    private void SendChatMessageToHost()
    {
        if (string.IsNullOrEmpty(messageInput.text))
        {
            PopupManager.Instance.ShowPopup(PopupType.ErrorPopup, "Cannot send empty message");
            return;
        }

        if (messageInput.text.Length > 500)
        {
            PopupManager.Instance.ShowPopup(PopupType.ErrorPopup, "Cannot send more than 500 characters at once");
            return;
        }

        ChatMessage chatMessage = new ChatMessage() { message = messageInput.text };
        MessageSender.SendMessageToHost(chatMessage);
        messageInput.text = string.Empty;
    }

    private void JoinRelay()
    {
        Loader.Instance.StartLoading();
        joinBtn.interactable = false;
        ConnectionManager.Instance.JoinRelay(joinCodeInputField.text);
        joinCodeInputField.text = string.Empty;
    }

    public void ShowJoinError(string error)
    {
        Loader.Instance.StopLoading();
        joinBtn.interactable = true;
        PopupManager.Instance.ShowPopup(PopupType.ErrorPopup, error);
    }

    private void DisconnectFromHost()
    {
        Loader.Instance.StartLoading();
        joinBtn.interactable = true;
        disconnectBtn.interactable = false;
        ConnectionManager.Instance.DisconnectFromHost();
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

        ReadyMessage message = new ReadyMessage() { isReady = isReady };
        MessageSender.SendMessageToHost(message);
    }

    public void ResetReadyButton()
    {
        isReady = false;
        readyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
        disconnectBtn.interactable = true;
    }
}
