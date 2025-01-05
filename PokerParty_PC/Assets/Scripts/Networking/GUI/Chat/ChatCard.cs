using PokerParty_SharedDLL;
using TMPro;
using UnityEngine;

public class ChatCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI chatMessageText;
    
    public void SetData(ChatMessage chatMessage)
    {
        playerNameText.color = PlayerColorManager.GetColor(chatMessage.player.PlayerName);
        playerNameText.text = chatMessage.player.PlayerName;
        chatMessageText.text = chatMessage.message;
    }
}
