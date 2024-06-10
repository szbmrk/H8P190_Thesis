using PokerParty_SharedDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ChatGUI : MonoBehaviour
{
    public static ChatGUI Instance;

    [SerializeField] private Transform parentForChatBoxes;
    [SerializeField] private GameObject chatBoxPrefab;
    [SerializeField] private ScrollRect chatScrollRect;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
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
