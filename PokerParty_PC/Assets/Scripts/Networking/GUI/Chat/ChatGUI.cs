using PokerParty_SharedDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ChatGUI : MonoBehaviour
{
    public static ChatGUI Instance;

    [SerializeField] private Transform parentForChatBoxes;
    [SerializeField] private GameObject chatBoxPrefab;

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
    }
}
