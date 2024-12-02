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

        [Theory]
        // High Card: No combination
        [InlineData(14, "Hearts", 12, "Spades", 9, "Diamonds", 6, "Clubs", 4, "Hearts", HandType.HighCard)]
        // One Pair: A pair of cards with the same value
        [InlineData(6, "Hearts", 6, "Spades", 3, "Diamonds", 9, "Clubs", 12, "Hearts", HandType.OnePair)]
        // Two Pair: Two pairs of cards with matching values
        [InlineData(8, "Hearts", 8, "Spades", 4, "Diamonds", 4, "Clubs", 13, "Hearts", HandType.TwoPair)]
        // Three of a Kind: Three cards with the same value
        [InlineData(11, "Hearts", 11, "Spades", 11, "Diamonds", 2, "Clubs", 7, "Spades", HandType.ThreeOfAKind)]
        // Full House: Three of a kind and a pair
        [InlineData(9, "Hearts", 9, "Spades", 9, "Diamonds", 12, "Clubs", 12, "Spades", HandType.FullHouse)]
        // Four of a Kind: Four cards with the same value
        [InlineData(10, "Hearts", 10, "Spades", 10, "Diamonds", 10, "Clubs", 2, "Hearts", HandType.FourOfAKind)]
        public void GetHandTypeByCountOfFaceValues_WhenHands_ShouldReturnCorrectHandType(
            int card1Value, string card1Suit,
            int card2Value, string card2Suit,
            int card3Value, string card3Suit,
            int card4Value, string card4Suit,
            int card5Value, string card5Suit,
            HandType expectedHandType)
        {
            Card[] hand = new Card[]
            {
                new Card(card1Value, card1Suit),
                new Card(card2Value, card2Suit),
                new Card(card3Value, card3Suit),
                new Card(card4Value, card4Suit),
                new Card(card5Value, card5Suit)
            };

            long mod15Res = EvaluationHelper.GetFaceValueCountBitField(hand) % 15;
            HandType handType = EvaluationHelper.GetHandTypeByCountOfFaceValues(mod15Res);
            Assert.Equal(expectedHandType, handType);
        }

        [Theory]
        // Straight: Five cards in a sequence
        [InlineData(9, "Hearts", 10, "Spades", 11, "Hearts", 12, "Clubs", 13, "Spades", HandType.Straight)]
        // Ace Low Straight: Five cards in a sequence with Ace as the lowest card
        [InlineData(2, "Hearts", 3, "Spades", 4, "Hearts", 5, "Clubs", 14, "Spades", HandType.Straight)]
        // Not Straight: No sequence
        [InlineData(2, "Hearts", 3, "Spades", 4, "Hearts", 5, "Clubs", 7, "Spades", HandType.None)]
        public void CheckForStraight_WhenHands_ShouldReturnCorrectHandType(
            int card1Value, string card1Suit,
            int card2Value, string card2Suit,
            int card3Value, string card3Suit,
            int card4Value, string card4Suit,
            int card5Value, string card5Suit,
            HandType expectedHandType)
        {
            Card[] hand = new Card[]
            {
                new Card(card1Value, card1Suit),
                new Card(card2Value, card2Suit),
                new Card(card3Value, card3Suit),
                new Card(card4Value, card4Suit),
                new Card(card5Value, card5Suit)
            };

            int faceValueBitField = EvaluationHelper.GetFaceValueBitField(hand);
            Assert.Equal(EvaluationHelper.CheckForStraight(faceValueBitField), expectedHandType);
        }

        [Theory]
        // Flush: Five cards of the same suit
        [InlineData(2, "Hearts", 3, "Hearts", 4, "Hearts", 5, "Hearts", 7, "Hearts", HandType.Flush)]
        // Straight Flush: Five cards in a sequence of the same suit
        [InlineData(9, "Hearts", 10, "Hearts", 11, "Hearts", 12, "Hearts", 13, "Hearts", HandType.StraightFlush)]
        // Royal Flush: A straight flush with Ace as the highest card
        [InlineData(10, "Hearts", 11, "Hearts", 12, "Hearts", 13, "Hearts", 14, "Hearts", HandType.RoyalFlush)]
        // Not Flush: No cards of the same suit
        [InlineData(2, "Hearts", 3, "Spades", 4, "Hearts", 5, "Clubs", 7, "Spades", HandType.None)]
        public void CheckForFlushes_WhenHands_ShouldReturnCorrectHandType(
            int card1Value, string card1Suit,
            int card2Value, string card2Suit,
            int card3Value, string card3Suit,
            int card4Value, string card4Suit,
            int card5Value, string card5Suit,
            HandType expectedHandType)
        {
            Card[] hand = new Card[]
            {
                new Card(card1Value, card1Suit),
                new Card(card2Value, card2Suit),
                new Card(card3Value, card3Suit),
                new Card(card4Value, card4Suit),
                new Card(card5Value, card5Suit)
            };

            Assert.Equal(EvaluationHelper.CheckForFlushes(hand), expectedHandType);
        }
    }
}
