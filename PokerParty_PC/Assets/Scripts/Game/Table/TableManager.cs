using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PokerParty_SharedDLL;

public class TableManager : MonoBehaviour
{
    public static TableManager Instance;
    public Deck deck;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        CreateDeck();
        AssignSeatsToPlayers();
    }

    private void CreateDeck()
    {
        deck = new Deck();
        deck.Shuffle();
        Debug.Log("Deck created and shuffled");
        Debug.Log("Deck count: " + deck.cards.Count);
    }

    private void AssignSeatsToPlayers()
    {

    }
}
