using PokerParty_SharedDLL;
using UnityEngine;
using UnityEngine.UI;

public class ChatGUI : MonoBehaviour
{
    public static ChatGUI instance;

    [SerializeField] public Transform parentForChatBoxes;
    [SerializeField] public GameObject chatBoxPrefab;
    [SerializeField] public ScrollRect chatScrollRect;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void AddChat(ChatMessage chatMessage)
    {
        var chatBox = Instantiate(chatBoxPrefab, parentForChatBoxes);
        chatBox.GetComponent<ChatCard>().SetData(chatMessage);
        chatBox.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentForChatBoxes.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatBox.GetComponent<RectTransform>());
        ScrollToBottom();
    }

    public void ClearChat()
    {
        for (int i = 0; i < parentForChatBoxes.childCount; i++)
        {
            Destroy(parentForChatBoxes.GetChild(i).gameObject);
        }
    }

    public void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0;
    }
}
