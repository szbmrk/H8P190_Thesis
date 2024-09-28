using System;
using System.Collections.Generic;
using System.Text;

namespace PokerParty_SharedDLL
{
    public class Deck
    {
        public List<Card> cards;

        public Deck()
        {
            cards = new List<Card>();
            for (int i = 2; i < 15; i++)
            {
                cards.Add(new Card(i, "Hearts"));
                cards.Add(new Card(i, "Diamonds"));
                cards.Add(new Card(i, "Clubs"));
                cards.Add(new Card(i, "Spades"));
            }
        }

        public void Shuffle()
        {
            Random rng = new Random();
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = cards[k];
                cards[k] = cards[n];
                cards[n] = value;
            }
        }

        public Card Draw()
        {
            Card card = cards[0];
            cards.RemoveAt(0);
            return card;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Card card in cards)
            {
                sb.Append(card.ToString()).Append("\n");
            }
            return sb.ToString();
        }
    }
}
