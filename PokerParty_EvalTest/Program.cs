using PokerParty_SharedDLL;

namespace PokerParty_EvalTest
{

    public static class PokerHands
    {
        public static Card[] RoyalFlush = {
            new Card(10, "Hearts"),
            new Card(11, "Hearts"),
            new Card(12, "Hearts"),
            new Card(13, "Hearts"),
            new Card(14, "Hearts")
        };

        public static Card[] StraightFlush = {
            new Card(9, "Spades"),
            new Card(8, "Spades"),
            new Card(7, "Spades"),
            new Card(6, "Spades"),
            new Card(5, "Spades")
        };

        public static Card[] FourOfAKind = {
            new Card(12, "Clubs"),
            new Card(12, "Diamonds"),
            new Card(12, "Hearts"),
            new Card(12, "Spades"),
            new Card(5, "Hearts")
        };

        public static Card[] FullHouse = {
            new Card(13, "Hearts"),
            new Card(13, "Spades"),
            new Card(13, "Diamonds"),
            new Card(14, "Clubs"),
            new Card(14, "Diamonds")
        };

        public static Card[] Flush = {
            new Card(2, "Hearts"),
            new Card(6, "Hearts"),
            new Card(9, "Hearts"),
            new Card(11, "Hearts"),
            new Card(14, "Hearts")
        };

        public static Card[] Straight = {
            new Card(6, "Clubs"),
            new Card(7, "Diamonds"),
            new Card(8, "Spades"),
            new Card(9, "Hearts"),
            new Card(10, "Clubs")
        };

        public static Card[] ThreeOfAKind = {
            new Card(11, "Diamonds"),
            new Card(11, "Hearts"),
            new Card(11, "Clubs"),
            new Card(3, "Spades"),
            new Card(7, "Diamonds")
        };

        public static Card[] TwoPair = {
            new Card(13, "Hearts"),
            new Card(13, "Clubs"),
            new Card(3, "Diamonds"),
            new Card(3, "Hearts"),
            new Card(10, "Spades")
        };

        public static Card[] OnePair = {
            new Card(10, "Diamonds"),
            new Card(10, "Hearts"),
            new Card(5, "Clubs"),
            new Card(7, "Spades"),
            new Card(2, "Hearts")
        };

        public static Card[] HighCard = {
            new Card(14, "Diamonds"),
            new Card(9, "Clubs"),
            new Card(6, "Hearts"),
            new Card(3, "Spades"),
            new Card(2, "Diamonds")
        };

        public static Card[] StraightDraw = {
            new Card(2, "Diamonds"),
            new Card(3, "Hearts"),
            new Card(5, "Diamonds"),
            new Card(4, "Spades"),
            new Card(6, "Diamonds")
        };

        public static Card[] InsideStraightDraw = {
            new Card(13, "Clubs"),
            new Card(12, "Diamonds"),
            new Card(11, "Hearts"),
            new Card(10, "Spades"),
            new Card(9, "Diamonds")
        };

        public static Card[][] Hands = {
            RoyalFlush,
            StraightFlush,
            FourOfAKind,
            FullHouse,
            Flush,
            Straight,
            ThreeOfAKind,
            TwoPair,
            OnePair,
            HighCard,
            StraightDraw,
            InsideStraightDraw
        };
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Card[] cards = new[]
            {
                new Card(9, "Hearts"),
                new Card(10, "Hearts"),
                new Card(11, "Hearts"),
                new Card(12, "Hearts"),
                new Card(13, "Hearts")
            };

            Console.WriteLine();
            EvaluationDisplayHelper.DisplayBitFieldFaceValues(cards);
            Console.WriteLine();
            EvaluationDisplayHelper.DisplayBitFieldCounts(cards);
        }
    }
}
