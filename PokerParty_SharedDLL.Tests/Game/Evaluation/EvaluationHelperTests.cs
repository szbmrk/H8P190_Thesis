using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParty_SharedDLL.Tests
{
    public class EvaluationHelperTests
    {
        public Card[] handWithStraightFromNinetoK = new Card[]
        {
            new Card(9, "Hearts"),
            new Card(10, "Spades"),
            new Card(11, "Hearts"),
            new Card(12, "Clubs"),
            new Card(13, "Spades")
        };

        public Card[] handWithFullHouse3Nine2K = new Card[]
        {
            new Card(9, "Clubs"),
            new Card(9, "Clubs"),
            new Card(9, "Hearts"),
            new Card(13, "Clubs"),
            new Card(13, "Spades")
        };

        [Fact]
        public void GetFaceValueBitField_WhenHands_ShouldReturnCorrectBitfield()
        {
            Assert.Equal(0b011111000000000, EvaluationHelper.GetFaceValueBitField(handWithStraightFromNinetoK));
            Assert.Equal(0b010001000000000, EvaluationHelper.GetFaceValueBitField(handWithFullHouse3Nine2K));
        }

        [Fact]
        public void GetFaceValueCountBitField_WhenHands_ShouldReturnCorrectBitfield()
        {
            Assert.Equal(0b000000010001000100010001000000000000000000000000000000000000, EvaluationHelper.GetFaceValueCountBitField(handWithStraightFromNinetoK));
            Assert.Equal(0b000000110000000000000111000000000000000000000000000000000000, EvaluationHelper.GetFaceValueCountBitField(handWithFullHouse3Nine2K));
        }

        /*
        [Theory]
        [InlineData(HandCollection.HighCard, HandType.HighCard)]
        public void GetHandTypeByCountOfFaceValues_WhenHands_ShouldReturnCorrectHandType(Card[] hand, HandType expectedHandType)
        {

        }

        /*
        public static HandType GetHandTypeByCountOfFaceValues(long mod15Res)
        {
            if (mod15Res == 1) return HandType.FourOfAKind;
            if (mod15Res == 10) return HandType.FullHouse;
            if (mod15Res == 9) return HandType.ThreeOfAKind;
            if (mod15Res == 7) return HandType.TwoPair;
            if (mod15Res == 6) return HandType.OnePair;
            if (mod15Res == 5) return HandType.HighCard;

            return HandType.None;
        }*/
    }
}
