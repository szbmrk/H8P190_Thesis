using PokerParty_SharedDLL;
using TMPro;
using UnityEngine;

public class ChatCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI chatMessageText;
    
    public void SetData(ChatMessage chatMessage)
    {
        playerNameText.color = PlayerColorManager.GetColor(chatMessage.Player.PlayerName);
        playerNameText.text = chatMessage.Player.PlayerName;
        chatMessageText.text = chatMessage.Message;
    }
}
