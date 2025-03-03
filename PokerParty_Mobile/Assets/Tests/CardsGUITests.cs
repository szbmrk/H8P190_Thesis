using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using PokerParty_SharedDLL;
using TMPro;

[TestFixture]
public class CardsGUITests
{
    private GameObject gameObject;
    private CardsGUI cardsGUI;
    private Image card1;
    private Image card2;
    private GameObject cardsHolder;
    private GameObject noCardsInHandText;
    private TextMeshProUGUI bestHandText;

    [SetUp]
    public void SetUp()
    {
        gameObject = new GameObject();
        cardsGUI = gameObject.AddComponent<CardsGUI>();
        
        // Mock UI elements
        card1 = new GameObject().AddComponent<Image>();
        card2 = new GameObject().AddComponent<Image>();
        cardsHolder = new GameObject();
        noCardsInHandText = new GameObject();
        bestHandText = new GameObject().AddComponent<TextMeshProUGUI>();

        // Assign serialized fields using reflection
        typeof(CardsGUI).GetField("card1", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardsGUI, card1);
        typeof(CardsGUI).GetField("card2", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardsGUI, card2);
        typeof(CardsGUI).GetField("cardsHolder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardsGUI, cardsHolder);
        typeof(CardsGUI).GetField("noCardsInHandText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardsGUI, noCardsInHandText);
        typeof(CardsGUI).GetField("bestHandText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cardsGUI, bestHandText);
    }

    [Test]
    public void NewRoundStarted_HidesCardsAndBestHandText()
    {
        cardsGUI.NewRoundStarted();

        Assert.IsFalse(cardsHolder.activeSelf);
        Assert.IsTrue(noCardsInHandText.activeSelf);
        Assert.IsFalse(bestHandText.gameObject.activeSelf);
    }

    [Test]
    public void SetCards_ShowsCards()
    {
        Card[] cards = new Card[]
        {
            new Card(14, "Hearts"),
            new Card(13, "Spades")
        };
        
        cardsGUI.SetCards(cards);

        Assert.IsFalse(noCardsInHandText.activeSelf);
        Assert.IsTrue(cardsHolder.activeSelf);
        Assert.IsNotNull(card1.sprite);
        Assert.IsNotNull(card2.sprite);
    }

    [Test]
    public void SetBestHandText_UpdatesBestHandText()
    {
        CommunityCardsChangedMessage message = new CommunityCardsChangedMessage
        {
            CommunityCards = new Card[]
            {
                new Card(14, "Hearts"),
                new Card(13, "Spades"),
                new Card(12, "Diamonds"),
                new Card(11, "Clubs"),
                new Card(10, "Hearts")
            }
        };
        
        GameManager.instance = gameObject.AddComponent<GameManager>();
        GameManager.instance.cards = new Card[] 
        {
            new Card(14, "Hearts"),
            new Card(13, "Spades")
        };

        cardsGUI.SetBestHandText(message);
        
        Assert.IsTrue(bestHandText.gameObject.activeSelf);
        Assert.IsFalse(string.IsNullOrEmpty(bestHandText.text));
    }
}
