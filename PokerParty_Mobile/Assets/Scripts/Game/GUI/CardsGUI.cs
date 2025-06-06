﻿using System.Collections.Generic;
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
    public GameObject yourTurnText;
    
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
        
        List<Card> allCards = new List<Card>();
        allCards.Add(GameManager.instance.cards[0]);
        allCards.Add(GameManager.instance.cards[1]);
        foreach (Card card in communityCardsChangedMessage.CommunityCards)
        {
            allCards.Add(card);
        }
        
        Card[][] allPossibleHands = TexasHoldEm.GetAllPossibleHands(allCards.ToArray());
        
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
