using TMPro;
using UnityEngine;
using PokerParty_SharedDLL;
using UnityEngine.UI;

public class LobbyPlayerCard : MonoBehaviour
{ 
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI ELOText;
    [SerializeField] private TextMeshProUGUI LevelText;
    [SerializeField] private GameObject readyIcon;
    [SerializeField] private Button kickBtn;


    public Player assignedPlayer;
    public bool isPlayerAssigned = false;
    [HideInInspector] public int indexInConnectionsArray = 0;

    private void Awake()
    {
        kickBtn.onClick.AddListener(KickPlayer);
    }

    public void RefreshData()
    {
        playerNameText.color = PlayerColorManager.GetColor(assignedPlayer.playerName);
        playerNameText.text = assignedPlayer.playerName;
        ELOText.text = $"ELO: {assignedPlayer.ELO}";
        LevelText.text = $"Level: {assignedPlayer.level}";
    }

    public void ResetData()
    {
        isPlayerAssigned = false;
        SetReady(false);
        PlayerColorManager.RemoveColorFromPlayer(assignedPlayer.playerName);

    }

    private bool isReady = false;
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

    private void KickPlayer()
    {
        LobbyManager.Instance.RemovePlayer(assignedPlayer);
        RelayManager.Instance.DisconnectPlayer(indexInConnectionsArray);
    }
}
