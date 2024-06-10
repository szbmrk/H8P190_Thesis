using TMPro;
using UnityEngine;
using PokerParty_SharedDLL;

public class PlayerCard : MonoBehaviour
{ 
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI ELOText;
    [SerializeField] private TextMeshProUGUI LevelText;

    public Player assignedPlayer;
    public bool isPlayerAssigned = false;
    public void RefreshData()
    {
        playerNameText.color = PlayerColorManager.GetColor(assignedPlayer.username);
        playerNameText.text = assignedPlayer.username;
        ELOText.text = $"ELO: {assignedPlayer.ELO}";
        LevelText.text = $"Level: {assignedPlayer.level}";
    }
}
