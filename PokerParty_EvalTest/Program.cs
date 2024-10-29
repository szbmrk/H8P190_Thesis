using PokerParty_SharedDLL;

namespace PokerParty_EvalTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Card[] river =
            {
                new Card(10, "Hearts"),
                new Card(11, "Diamonds"),
                new Card(12, "Spades"),
                new Card(13, "Clubs"),
                new Card(14, "Spades")
            };


            EvaluationDisplayHelper.DisplayBitFieldFaceValues(river);
            Console.WriteLine();
            EvaluationDisplayHelper.DisplayBitFieldCounts(river);

            Console.ReadKey();
        }
    }
}
