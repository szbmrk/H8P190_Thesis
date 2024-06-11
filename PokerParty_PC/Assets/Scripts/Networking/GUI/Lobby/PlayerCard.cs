using TMPro;
using UnityEngine;
using PokerParty_SharedDLL;

public class PlayerCard : MonoBehaviour
{ 
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI ELOText;
    [SerializeField] private TextMeshProUGUI LevelText;
    [SerializeField] private GameObject readyIcon;

    private bool isReady = false;

    public Player assignedPlayer;
    public bool isPlayerAssigned = false;

    public void RefreshData()
    {
        playerNameText.color = PlayerColorManager.GetColor(assignedPlayer.username);
        playerNameText.text = assignedPlayer.username;
        ELOText.text = $"ELO: {assignedPlayer.ELO}";
        LevelText.text = $"Level: {assignedPlayer.level}";
    }

    public bool IsReady
    {
        get
        {
            return isReady;
        }
    }

    public void SetReady(bool ready)
    {
        isReady = ready;
        readyIcon.SetActive(ready);
    }
}
