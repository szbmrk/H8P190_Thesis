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
                new Card(2, "Hearts"),
                new Card(4, "Hearts"),
                new Card(13, "Hearts"),
                new Card(14, "Hearts")
            };


            EvaluationDisplayHelper.DisplayBitFieldFaceValues(river);
            Console.WriteLine();
            EvaluationDisplayHelper.DisplayBitFieldCounts(river);

            Console.WriteLine();
            Console.WriteLine(EvaluationHelper.CheckForFlush(river));

            Console.ReadKey();
        }
    }
}
