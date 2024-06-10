using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class NetworkingGUI : MonoBehaviour
{
    public static NetworkingGUI Instance;

    [SerializeField] private PlayerCard playerCard;
    
    [SerializeField] private Button joinBtn;
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

        joinBtn.onClick.AddListener(() => RelayManager.Instance.JoinRelay(joinCodeInputField.text));
        disconnectBtn.onClick.AddListener(() => RelayManager.Instance.DisconnectFromHost());
        sendTestMsgBtn.onClick.AddListener(SendChatMessageToHost);
    }

    public void ShowJoinedPanel(bool value)
    {
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
        messageInput.text = "";
    }

    private IEnumerator DisplayErrorText(string error)
    {
        errorText.text = error;
        yield return new WaitForSeconds(5f);
        errorText.text = "";
    }
}
