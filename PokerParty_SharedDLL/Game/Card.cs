using System.Text;

namespace PokerParty_SharedDLL
{
    public class Card
    {
        public int value;
        public string suit;

        public Card(int value, string suit)
        {
            this.value = value;
            this.suit = suit;
        }

        public string GetValueString()
        {
            if (value == 11) return "Jack";
            if (value == 12) return "Queen";
            if (value == 13) return "King";
            if (value == 14) return "A";
            return value.ToString();
        }

        public string GetFileNameForSprite()
        {
            return suit + "_" + GetValueString() + ".png";
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Value: ").Append(GetValueString()).Append("\n");
            sb.Append("Suit: ").Append(suit).Append("\n");
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Card)) return false;

            Card other = obj as Card;
            if (this.value == other.value && this.suit == other.suit) return true;

            return false;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode() + suit.GetHashCode();
        }
    }
}
