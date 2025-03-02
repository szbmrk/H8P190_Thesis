using TMPro;
using UnityEngine;
using PokerParty_SharedDLL;
using UnityEngine.UI;

public class LobbyPlayerCard : MonoBehaviour
{ 
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private GameObject readyIcon;
    [SerializeField] private Button kickBtn;


    public Player assignedPlayer;
    [HideInInspector] public int indexInConnectionsArray;

    private void Awake()
    {
        if (kickBtn != null)
            kickBtn.onClick.AddListener(KickPlayer);
    }

    public void RefreshData()
    {
        if (playerNameText == null)
            return;
        
        playerNameText.color = PlayerColorManager.GetColor(assignedPlayer.PlayerName);
        playerNameText.text = assignedPlayer.PlayerName;
    }

    public void ResetData()
    {
        SetReady(false);
        PlayerColorManager.RemoveColorFromPlayer(assignedPlayer.PlayerName);
    }

    public bool isReady { get; private set; }

    public void SetReady(bool ready)
    {
        isReady = ready;
        readyIcon.SetActive(ready);
    }

    private void KickPlayer()
    {
        LobbyManager.instance.RemovePlayer(assignedPlayer);
        ConnectionManager.instance.DisconnectPlayer(indexInConnectionsArray);
    }
}
