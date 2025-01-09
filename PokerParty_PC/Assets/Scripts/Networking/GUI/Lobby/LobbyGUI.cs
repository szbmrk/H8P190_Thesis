using System.Collections;
using TMPro;
using UnityEngine;
using PokerParty_SharedDLL;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyGUI : MonoBehaviour
{
    public static LobbyGUI instance;
    public TextMeshProUGUI joinCodeText;

    [SerializeField] private Button deleteLobbyBtn;
    [SerializeField] private Button startGameBtn;
    [SerializeField] private GameObject minimumPlayersText;

    [SerializeField] private TextMeshProUGUI playerCount;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private LobbyPlayerCard playerCardPrefab;
    [SerializeField] private Transform parentForPlayerCards;
    [SerializeField] private TMP_Dropdown startingMoneyDropdown;

    private int numOfPlayers;
    private int startingMoney = 1000;

    private void Start()
    {
        if (instance == null)
            instance = this;

        startingMoneyDropdown.onValueChanged.AddListener(HandleStartingMoneyDropdownValueChanged);
        deleteLobbyBtn.onClick.AddListener(() => StartCoroutine(DeleteLobby()));
        startGameBtn.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        Settings.startingMoney = startingMoney;
        Settings.playerCount = numOfPlayers;
        ConnectionManager.instance.SendMessageToAllConnections(new GameStartedMessage());
        SceneManager.LoadScene("Game");
    }

    public IEnumerator DeleteLobby()
    {
        Loader.Instance.StartLoading();
        joinCodeText.text = string.Empty;
        ClearPlayers();
        ChatGUI.instance.ClearChat();
        yield return LobbyManager.instance.DeleteLobby();

        SceneManager.LoadScene("MainMenu");
    }

    public LobbyPlayerCard DisplayNewPlayer(Player player)
    {
        Transform playerCardTransform = Instantiate(playerCardPrefab, parentForPlayerCards).transform;
        LobbyPlayerCard playerCard = playerCardTransform.GetComponent<LobbyPlayerCard>();
        playerCard.assignedPlayer = player;
        playerCardTransform.GetComponent<LobbyPlayerCard>().RefreshData();

        playerCard.gameObject.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(parentForPlayerCards.GetComponent<RectTransform>());

        numOfPlayers++;
        RefreshPlayerCount();
        return playerCard;
    }

    public void RemovePlayerFromDisplay(Player player)
    {
        if (numOfPlayers == 0) return;

        LobbyPlayerCard playerCard = LobbyManager.instance.GetPlayerCardForPlayer(player);

        if (!playerCard) return;

        playerCard.ResetData();
        Destroy(playerCard.gameObject);

        numOfPlayers--;
        RefreshPlayerCount();
    }

    private void ClearPlayers()
    {
        if (numOfPlayers == 0) return;

        for (int i = 0; i < parentForPlayerCards.childCount; i++)
        {
            Destroy(parentForPlayerCards.GetChild(i).gameObject);
        }

        numOfPlayers = 0;
        RefreshPlayerCount();
    }

    public void RefreshPlayerCount()
    {
        if (numOfPlayers >= 4 && LobbyManager.instance.AreAllPlayersReady())
        {
            startGameBtn.interactable = true;
            minimumPlayersText.SetActive(false);
        }
        else
        {
            startGameBtn.interactable = false;
            minimumPlayersText.SetActive(true);
        }

        playerCount.text = numOfPlayers.ToString() + "/8";
    }

    private void HandleStartingMoneyDropdownValueChanged(int index)
    {
        int newStartingMoney = int.Parse(startingMoneyDropdown.options[index].text.Split(' ')[0]);
        startingMoney = newStartingMoney;
    }

    public void ShowPanel()
    {
        lobbyPanel.SetActive(true);
    }
}
