using System;
using System.Text;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class Card
    {
        public int Value;
        public string Suit;

        public Card(int value, string suit)
        {
            this.Value = value;
            this.Suit = suit;
        }

        public string GetValueString()
        {
            if (Value == 11)
                return "Jack";
            if (Value == 12)
                return "Queen";
            if (Value == 13)
                return "King";
            if (Value == 14)
                return "A";
            return Value.ToString();
        }

        public string GetFileNameForSprite()
        {
            return Suit + "_" + GetValueString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Value: ").Append(GetValueString()).Append("\n");
            sb.Append("Suit: ").Append(Suit).Append("\n");
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Card)) return false;

            Card other = obj as Card;
            
            return this.Value == other.Value && this.Suit == other.Suit;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode() + Suit.GetHashCode();
        }
    }
}
