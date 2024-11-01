using PokerParty_SharedDLL;

namespace PokerParty_EvalTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Card[] hand =
            {
                new Card(8, "Diamonds"),
                new Card(6, "Diamonds"),
                new Card(8, "Diamonds"),
                new Card(8, "Diamonds"),
                new Card(6, "Diamonds")
            };

            Card[] hand1 =
            {
                new Card(8, "Hearts"),
                new Card(6, "Hearts"),
                new Card(8, "Hearts"),
                new Card(8, "Hearts"),
                new Card(6, "Hearts")
            };



            EvaluationDisplayHelper.DisplayBitFieldFaceValues(hand);
            Console.WriteLine();
            
            EvaluationDisplayHelper.DisplayBitFieldCounts(hand);
            Console.WriteLine();

            foreach (var item in EvaluationHelper.BreakTies(hand, hand1))
            {
                Console.WriteLine(item);
            }   

            Console.WriteLine();

            Console.ReadKey();
        }
    }
}
