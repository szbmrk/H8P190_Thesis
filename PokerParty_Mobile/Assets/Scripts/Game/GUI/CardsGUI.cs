using PokerParty_SharedDLL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardsGUI : MonoBehaviour
{
    public static CardsGUI instance;

    [SerializeField] private Image card1;
    [SerializeField] private Image card2;
    
    
    [SerializeField] private GameObject cardsHolder;
    [SerializeField] private GameObject noCardsInHandText;
    
    [SerializeField] private TextMeshProUGUI bestHandText;
    

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void NewRoundStarted()
    {
        cardsHolder.SetActive(false);
        noCardsInHandText.SetActive(true);
        bestHandText.gameObject.SetActive(false);
    }
    
    public void SetCards(Card[] cards)
    {
        noCardsInHandText.SetActive(false);
        card1.sprite = GetSpriteByFileName(cards[0].GetFileNameForSprite());
        card2.sprite = GetSpriteByFileName(cards[1].GetFileNameForSprite());
        cardsHolder.SetActive(true);
    }

    public void SetBestHandText(CommunityCardsChangedMessage communityCardsChangedMessage)
    {
        if (!bestHandText.gameObject.activeInHierarchy)
            bestHandText.gameObject.SetActive(true);

        Card[] cards = new Card[2 + communityCardsChangedMessage.CommunityCards.Length];
        cards[0] = GameManager.instance.cards[0];
        cards[1] = GameManager.instance.cards[1];
        for (int i = 0; i < communityCardsChangedMessage.CommunityCards.Length; i++)
        {
            cards[i + 2] = communityCardsChangedMessage.CommunityCards[i];
        }

        Card[][] allPossibleHands = TexasHoldEm.GetAllPossibleHands(cards);
        Card[] bestHand = TexasHoldEm.GetBestHandOfPlayer(allPossibleHands);
        HandType bestHandType = TexasHoldEm.EvaluateHand(bestHand);

        bestHandText.text = bestHandType.ToString();
    }

    private Sprite GetSpriteByFileName(string fileName)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Cards");
        foreach (Sprite sprite in sprites)
        {
            if (sprite.name == fileName)
                return sprite;
        }

        return null;
    }
}
