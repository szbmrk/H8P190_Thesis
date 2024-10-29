using System;

namespace PokerParty_SharedDLL
{
    public static class EvaluationHelper
    {
        public static int GetFaceValueBitField(Card[] hand)
        {
            int bitField = 0;

            foreach (Card card in hand)
            {
                int bitPosition = card.value;
                bitField |= (1 << bitPosition);
            }

            return bitField;
        }


        public static long GetFaceValueCountBitField(Card[] hand)
        {
            long bitField = 0;

            int[] counts = new int[13];

            foreach (Card card in hand)
            {
                int index = card.value - 2;
                counts[index]++;
            }

            for (int i = 0; i < counts.Length; i++)
            {
                int countBitField = 0;

                for (int j = 0; j <= counts[i]; j++)
                {
                    countBitField = (1 << j) - 1;
                }

                bitField |= (long)countBitField << (i * 4) + 8;
            }

            return bitField;
        }
    }
}
