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
    [SerializeField] private GameObject conditionToStartText;

    [SerializeField] private TextMeshProUGUI playerCount;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private LobbyPlayerCard playerCardPrefab;
    [SerializeField] private Transform parentForPlayerCards;
    [SerializeField] private TMP_Dropdown startingMoneyDropdown;

    private int numOfPlayers;

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
        Settings.startingMoney = LobbySettings.startingMoney;
        Settings.playerCount = numOfPlayers;
        ConnectionManager.instance.SendMessageToAllConnections(new GameStartedMessage());
        Logger.LogToFile("Game started");
        SceneManager.LoadScene("Game");
    }

    public IEnumerator DeleteLobby()
    {
        Loader.instance.StartLoading();
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
        return playerCard;
    }

    public void RemovePlayerFromDisplay(LobbyPlayerCard playerCard)
    {
        if (numOfPlayers == 0) return;

        if (!playerCard) return;

        playerCard.ResetData();
        Destroy(playerCard.gameObject);

        numOfPlayers--;
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
        if (numOfPlayers >= LobbySettings.MinimumPlayers && LobbyManager.instance.AreAllPlayersReady())
        {
            startGameBtn.interactable = true;
            conditionToStartText.SetActive(false);
        }
        else
        {
            startGameBtn.interactable = false;
            
            conditionToStartText.GetComponent<TextMeshProUGUI>().text = numOfPlayers < LobbySettings.MinimumPlayers ?
                $"Minimum players: {LobbySettings.MinimumPlayers}" :
                $"All players must be ready: {LobbyManager.instance.PlayersReadyCount()}/{numOfPlayers}";
            
            conditionToStartText.SetActive(true);
        }

        playerCount.text = numOfPlayers.ToString() + "/" + LobbySettings.MaximumPlayers;
    }

    private void HandleStartingMoneyDropdownValueChanged(int index)
    {
        int newStartingMoney = int.Parse(startingMoneyDropdown.options[index].text.Replace('$', ' '));
        LobbySettings.startingMoney = newStartingMoney;
    }

    public void ShowPanel()
    {
        lobbyPanel.SetActive(true);
    }
}
