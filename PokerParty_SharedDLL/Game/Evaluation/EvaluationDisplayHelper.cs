using System;

namespace PokerParty_SharedDLL
{
    public static class EvaluationDisplayHelper
    {
        public static void DisplayBitFieldFaceValues(Card[] hand)
        {
            int bitField = EvaluationHelper.GetFaceValueBitField(hand);
            string bitString = Convert.ToString(bitField, 2).PadLeft(15, '0');

            foreach (char c in bitString)
            {
                Console.Write(c + " ");
            }

            Console.WriteLine("\nA K Q J T 9 8 7 6 5 4 3 2");
        }

        public static void DisplayBitFieldCounts(Card[] hand)
        {
            long bitField = EvaluationHelper.GetFaceValueCountBitField(hand);
            string bitString = Convert.ToString(bitField, 2).PadLeft(60, '0');

            for (int i = 0; i < bitString.Length; i += 4)
            {
                Console.Write(bitString.Substring(i, 4) + " ");
            }

            Console.WriteLine("\n   A    K    Q    J    T    9    8    7    6    5    4    3    2");
        }
    }
}
