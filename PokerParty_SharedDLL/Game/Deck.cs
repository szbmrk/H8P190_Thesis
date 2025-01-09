using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerParty_SharedDLL
{
    public class Deck
    {
        public Stack<Card> Cards;

        public Deck()
        {
            Cards = new Stack<Card>();
            for (int i = 2; i < 15; i++)
            {
                Cards.Push(new Card(i, "Hearts"));
                Cards.Push(new Card(i, "Diamonds"));
                Cards.Push(new Card(i, "Clubs"));
                Cards.Push(new Card(i, "Spades"));
            }
        }

        public void Shuffle()
        {
            Random rng = new Random();
            Card[] cardArray = Cards.ToArray();
            Cards.Clear();
            foreach (Card card in cardArray.OrderBy(a => rng.Next()))
            {
                Cards.Push(card);
            }
        }

        public Card Draw()
        {
            return Cards.Pop();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Card card in Cards)
            {
                sb.Append(card.ToString()).Append("\n");
            }
            return sb.ToString();
        }
    }
}
