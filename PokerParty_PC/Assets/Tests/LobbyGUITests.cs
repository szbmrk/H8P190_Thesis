using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PokerParty_SharedDLL;
using NUnit.Framework;

public class LobbyGUITests
{
    private GameObject lobbyGUIObject;
    private LobbyGUI lobbyGUI;
    
    [SetUp]
    public void Setup()
    {
        lobbyGUIObject = new GameObject("LobbyGUI");
        lobbyGUI = lobbyGUIObject.AddComponent<LobbyGUI>();
        LobbyGUI.instance = lobbyGUI;

        ConnectionManager.instance = new GameObject().AddComponent<ConnectionManager>();
        AudioManager.instance = new GameObject().AddComponent<AudioManager>();
        AudioManager.instance.playerJoinedSource = new GameObject().AddComponent<AudioSource>();
        
        lobbyGUI.joinCodeText = new GameObject().AddComponent<TextMeshProUGUI>();
        lobbyGUI.deleteLobbyBtn = new GameObject().AddComponent<Button>();
        lobbyGUI.startGameBtn = new GameObject().AddComponent<Button>();
        lobbyGUI.conditionToStartText = new GameObject();
        lobbyGUI.playerCount = new GameObject().AddComponent<TextMeshProUGUI>();
        lobbyGUI.lobbyPanel = new GameObject();
        lobbyGUI.parentForPlayerCards = new GameObject().transform;
        lobbyGUI.startingMoneyDropdown = new GameObject().AddComponent<TMP_Dropdown>();
    }
    
    [Test]
    public void DisplayNewPlayer_IncreasesPlayerCount()
    {
        // Arrange
        Player testPlayer = new Player("TestPlayer");
        lobbyGUI.playerCardPrefab = new GameObject().AddComponent<LobbyPlayerCard>();
        int initialCount = LobbyGUI.instance.parentForPlayerCards.transform.childCount;

        // Act
        lobbyGUI.DisplayNewPlayer(testPlayer);

        // Assert
        Assert.AreEqual(initialCount + 1, LobbyGUI.instance.parentForPlayerCards.transform.childCount);
    }
    
    [Test]
    public void RemovePlayerFromDisplay_DecreasesPlayerCount()
    {
        // Arrange
        LobbyPlayerCard testCard = new GameObject().AddComponent<LobbyPlayerCard>();
        lobbyGUIObject.transform.SetParent(new GameObject().transform);

        // Act
        lobbyGUI.RemovePlayerFromDisplay(testCard);

        // Assert
        Assert.AreEqual(0, lobbyGUIObject.transform.childCount);
    }
    
    [TearDown]
    public void Teardown()
    {
        Object.Destroy(lobbyGUIObject);
    }
}
