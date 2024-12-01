using System;
using System.Collections.Generic;

namespace PokerParty_SharedDLL.Tests
{
    public static class HandCollection
    {
        // High Card: No combination
        public static readonly Card[] HighCard = new Card[]
        {
            new Card(14, "Hearts"),
            new Card(12, "Spades"),
            new Card(9, "Diamonds"),
            new Card(6, "Clubs"),
            new Card(4, "Hearts")
        };

        // One Pair: A pair of cards with the same value
        public static readonly Card[] OnePair = new Card[]
        {
            new Card(6, "Hearts"),
            new Card(6, "Spades"),
            new Card(3, "Diamonds"),
            new Card(9, "Clubs"),
            new Card(12, "Hearts")
        };

        // Two Pair: Two pairs of cards with matching values
        public static readonly Card[] TwoPair = new Card[]
        {
            new Card(8, "Hearts"),
            new Card(8, "Spades"),
            new Card(4, "Diamonds"),
            new Card(4, "Clubs"),
            new Card(13, "Hearts")
        };

        // Three of a Kind: Three cards with the same value
        public static readonly Card[] ThreeOfAKind = new Card[]
        {
            new Card(11, "Hearts"),
            new Card(11, "Spades"),
            new Card(11, "Diamonds"),
            new Card(2, "Clubs"),
            new Card(7, "Spades")
        };

        // Straight: Five sequential cards of any suit
        public static readonly Card[] Straight = new Card[]
        {
            new Card(9, "Hearts"),
            new Card(10, "Spades"),
            new Card(11, "Hearts"),
            new Card(12, "Clubs"),
            new Card(13, "Spades")
        };

        // Full House: Three of a kind and a pair
        public static readonly Card[] FullHouse = new Card[]
        {
            new Card(9, "Hearts"),
            new Card(9, "Spades"),
            new Card(9, "Diamonds"),
            new Card(12, "Clubs"),
            new Card(12, "Spades")
        };

        // Four of a Kind: Four cards with the same value
        public static readonly Card[] FourOfAKind = new Card[]
        {
            new Card(10, "Hearts"),
            new Card(10, "Spades"),
            new Card(10, "Diamonds"),
            new Card(10, "Clubs"),
            new Card(2, "Hearts")
        };

        // Flush: Five cards of the same suit, not in sequence
        public static readonly Card[] Flush = new Card[]
        {
            new Card(2, "Hearts"),
            new Card(8, "Hearts"),
            new Card(10, "Hearts"),
            new Card(11, "Hearts"),
            new Card(13, "Hearts")
        };

        // Straight Flush: Five sequential cards of the same suit
        public static readonly Card[] StraightFlush = new Card[]
        {
            new Card(9, "Spades"),
            new Card(10, "Spades"),
            new Card(11, "Spades"),
            new Card(12, "Spades"),
            new Card(13, "Spades")
        };

        // Royal Flush: Ace, King, Queen, Jack, and 10 of the same suit
        public static readonly Card[] RoyalFlush = new Card[]
        {
            new Card(10, "Hearts"),
            new Card(11, "Hearts"),
            new Card(12, "Hearts"),
            new Card(13, "Hearts"),
            new Card(14, "Hearts")
        };
    }
}
