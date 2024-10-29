using PokerParty_SharedDLL;

namespace PokerParty_EvalTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Card[] hand =
            {
                new Card(9, "Hearts"),
                new Card(9, "Diamonds"),
                new Card(9, "Spades"),
                new Card(9, "Clubs"),
                new Card(13, "Spades")
            };

            EvaluationDisplayHelper.DisplayBitFieldFaceValues(EvaluationHelper.GetFaceValueBitField(hand));
            Console.WriteLine();
            EvaluationDisplayHelper.DisplayBitFieldCounts(EvaluationHelper.GetFaceValueCountBitField(hand));

            Console.ReadKey();
        }
    }
}
