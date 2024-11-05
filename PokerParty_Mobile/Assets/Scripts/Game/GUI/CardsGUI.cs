using PokerParty_SharedDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardsGUI : MonoBehaviour
{
    public static CardsGUI Instance;

    [SerializeField] private Image card1;
    [SerializeField] private Image card2;

    [SerializeField] private TextMeshProUGUI bestHandText;

    [SerializeField] private GameObject switchButton;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void SetCards(Card[] cards)
    {
        switchButton.SetActive(true);
        card1.sprite = GetSpriteByFileName(cards[0].GetFileNameForSprite());
        card2.sprite = GetSpriteByFileName(cards[1].GetFileNameForSprite());
    }

    public void SetBestHandText(CommunityCardsChanged communityCardsChanged)
    {
        if (!bestHandText.gameObject.activeInHierarchy)
            bestHandText.gameObject.SetActive(true);

        Card[] cards = new Card[2 + communityCardsChanged.communityCards.Length];
        cards[0] = GameManager.Instance.cards[0];
        cards[1] = GameManager.Instance.cards[1];
        for (int i = 0; i < communityCardsChanged.communityCards.Length; i++)
        {
            cards[i + 2] = communityCardsChanged.communityCards[i];
        }

        Card[][] allPossibleHands = TexasHoldEm.GetAllPossibleHands(cards);
        Card[] bestHand = TexasHoldEm.GetBestHandOfPlayer(allPossibleHands);
        HandType bestHandType = TexasHoldEm.EvaluateHand(bestHand);

        bestHandText.text = bestHandType.ToString();
    }

    public Sprite GetSpriteByFileName(string fileName)
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
