using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerParty_SharedDLL
{
    public class Deck
    {
        public Stack<Card> cards;

        public Deck()
        {
            cards = new Stack<Card>();
            for (int i = 2; i < 15; i++)
            {
                cards.Push(new Card(i, "Hearts"));
                cards.Push(new Card(i, "Diamonds"));
                cards.Push(new Card(i, "Clubs"));
                cards.Push(new Card(i, "Spades"));
            }
        }

        public void Shuffle()
        {
            Random rng = new Random();
            Card[] cardArray = cards.ToArray();
            cards.Clear();
            foreach (Card card in cardArray.OrderBy(a => rng.Next()))
            {
                cards.Push(card);
            }
        }

        public Card Draw()
        {
            return cards.Pop();
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
