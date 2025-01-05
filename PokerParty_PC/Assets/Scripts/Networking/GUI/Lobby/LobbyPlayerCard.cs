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
    [HideInInspector] public int indexInConnectionsArray = 0;

    private void Awake()
    {
        kickBtn.onClick.AddListener(KickPlayer);
    }

    public void RefreshData()
    {
        playerNameText.color = PlayerColorManager.GetColor(assignedPlayer.PlayerName);
        playerNameText.text = assignedPlayer.PlayerName;
    }

    public void ResetData()
    {
        SetReady(false);
        PlayerColorManager.RemoveColorFromPlayer(assignedPlayer.PlayerName);
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
        ConnectionManager.Instance.DisconnectPlayer(indexInConnectionsArray);
    }
}
