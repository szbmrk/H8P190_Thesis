namespace PokerParty_SharedDLL.Tests
{
    public class TexasHoldEmTests
    {
        [Theory]
        [InlineData(14, "Hearts", 12, "Spades", 9, "Diamonds", 6, "Clubs", 4, "Hearts", HandType.HighCard)]
        [InlineData(6, "Hearts", 6, "Spades", 3, "Diamonds", 9, "Clubs", 12, "Hearts", HandType.OnePair)]
        [InlineData(8, "Hearts", 8, "Spades", 4, "Diamonds", 4, "Clubs", 13, "Hearts", HandType.TwoPair)]
        [InlineData(11, "Hearts", 11, "Spades", 11, "Diamonds", 2, "Clubs", 7, "Spades", HandType.ThreeOfAKind)]
        [InlineData(9, "Hearts", 9, "Spades", 9, "Diamonds", 12, "Clubs", 12, "Spades", HandType.FullHouse)]
        [InlineData(10, "Hearts", 10, "Spades", 10, "Diamonds", 10, "Clubs", 2, "Hearts", HandType.FourOfAKind)]
        [InlineData(2, "Hearts", 5, "Hearts", 4, "Hearts", 7, "Hearts", 6, "Hearts", HandType.Flush)]
        [InlineData(2, "Hearts", 3, "Hearts", 4, "Hearts", 5, "Hearts", 6, "Spades", HandType.Straight)]
        [InlineData(2, "Hearts", 3, "Hearts", 4, "Hearts", 5, "Hearts", 6, "Hearts", HandType.StraightFlush)]
        [InlineData(10, "Hearts", 11, "Hearts", 12, "Hearts", 13, "Hearts", 14, "Hearts", HandType.RoyalFlush)]
        public void EvaluateHand_WhenHands_ShouldReturnCorrectHandType(
            int card1Value, string card1Suit,
            int card2Value, string card2Suit,
            int card3Value, string card3Suit,
            int card4Value, string card4Suit,
            int card5Value, string card5Suit,
            HandType expectedHandType)
        {
            Card[] hand =
            [
                new Card(card1Value, card1Suit),
                new Card(card2Value, card2Suit),
                new Card(card3Value, card3Suit),
                new Card(card4Value, card4Suit),
                new Card(card5Value, card5Suit)
            ];

            Assert.Equal(expectedHandType, TexasHoldEm.EvaluateHand(hand));
        }

         public static IEnumerable<object[]> GetBestHandTestData()
        {
            yield return
            [
                new Card[][]
                {
                    [
                        new Card(9, "Hearts"),
                        new Card(9, "Clubs"),
                        new Card(12, "Diamonds"),
                        new Card(13, "Hearts"),
                        new Card(14, "Spades")
                    ],
                    [
                        new Card(4, "Spades"),
                        new Card(4, "Hearts"),
                        new Card(6, "Clubs"),
                        new Card(8, "Diamonds"),
                        new Card(10, "Spades")
                    ],
                    [
                        new Card(3, "Diamonds"),
                        new Card(3, "Clubs"),
                        new Card(7, "Hearts"),
                        new Card(9, "Diamonds"),
                        new Card(12, "Clubs")
                    ],
                    [
                        new Card(5, "Hearts"),
                        new Card(5, "Diamonds"),
                        new Card(2, "Spades"),
                        new Card(6, "Clubs"),
                        new Card(11, "Hearts")
                    ],
                    [
                        new Card(9, "Hearts"),
                        new Card(9, "Clubs"),
                        new Card(4, "Spades"),
                        new Card(4, "Diamonds"),
                        new Card(13, "Hearts")
                    ]
                },
                new Card[]
                {
                    new Card(9, "Hearts"),
                    new Card(9, "Clubs"),
                    new Card(4, "Spades"),
                    new Card(4, "Diamonds"),
                    new Card(13, "Hearts")
                }
            ];
        }

        [Theory]
        [MemberData(nameof(GetBestHandTestData))]
        public void GetBestHand_When_ShouldReturnCorrectBestHand(Card[][] hands, Card[] expectedBestHand)
        {
            Assert.Equal(expectedBestHand, TexasHoldEm.GetBestHandOfPlayer(hands));
        }

        [Fact]
        public void GetAllPossibleHands_When_ShouldReturnCorrectPossibleHands()
        {
            Card[] cards =
            [
                new Card(2, "Hearts"),
                new Card(3, "Clubs"),
                new Card(4, "Hearts"),
                new Card(5, "Hearts"),
                new Card(6, "Spades"),
                new Card(7, "Diamonds"),
            ];

            Card[] hand1 =
            [
                new Card(2, "Hearts"),
                new Card(3, "Clubs"),
                new Card(4, "Hearts"),
                new Card(5, "Hearts"),
                new Card(6, "Spades")
            ];

            Card[] hand2 =
            [
                new Card(2, "Hearts"),
                new Card(3, "Clubs"),
                new Card(4, "Hearts"),
                new Card(5, "Hearts"),
                new Card(7, "Diamonds")
            ];

            Card[] hand3 =
            [
                new Card(2, "Hearts"),
                new Card(3, "Clubs"),
                new Card(4, "Hearts"),
                new Card(6, "Spades"),
                new Card(7, "Diamonds")
            ];

            Card[] hand4 =
            [
                new Card(2, "Hearts"),
                new Card(3, "Clubs"),
                new Card(5, "Hearts"),
                new Card(6, "Spades"),
                new Card(7, "Diamonds")
            ];

            Card[] hand5 =
            [
                new Card(2, "Hearts"),
                new Card(4, "Hearts"),
                new Card(5, "Hearts"),
                new Card(6, "Spades"),
                new Card(7, "Diamonds")
            ];

            Card[] hand6 =
            [
                new Card(3, "Clubs"),
                new Card(4, "Hearts"),
                new Card(5, "Hearts"),
                new Card(6, "Spades"),
                new Card(7, "Diamonds")
            ];

            Card[][] manualPossibleHands = new Card[][]
            {
                hand1,
                hand2,
                hand3,
                hand4,
                hand5,
                hand6
            };

            Card[][] possibleHands = TexasHoldEm.GetAllPossibleHands(cards);

            Assert.Equal(manualPossibleHands, possibleHands);
        }
        
        [Fact]
        public void GetAllPossibleHands_When_ShouldOnlyReturn5LengthHands()
        {
            Card[] cards =
            [
                new Card(2, "Hearts"),
                new Card(3, "Clubs"),
                new Card(4, "Hearts"),
                new Card(5, "Hearts"),
                new Card(6, "Spades"),
                new Card(7, "Diamonds"),
            ];

            Card[][] possibleHands = TexasHoldEm.GetAllPossibleHands(cards);

            foreach (Card[] hand in possibleHands)
            {
                Assert.Equal(5, hand.Length);
            }
        }

        [Fact]
        public void GetBestHandFromGetAllPossibleHands_When_ShouldReturnCorrectBestHand()
        {
            Card[] cards =
            [
                new Card(2, "Hearts"),
                new Card(2, "Hearts"),
                new Card(3, "Clubs"),
                new Card(4, "Hearts"),
                new Card(5, "Hearts"),
                new Card(6, "Spades"),
                new Card(7, "Diamonds"),
            ];

            Card[][] possibleHand = TexasHoldEm.GetAllPossibleHands(cards);

            Card[] bestHand =
            [
                new Card(3, "Clubs"),
                new Card(4, "Hearts"),
                new Card(5, "Hearts"),
                new Card(6, "Spades"),
                new Card(7, "Diamonds")
            ];
            
            Assert.Equal(bestHand, TexasHoldEm.GetBestHandOfPlayer(possibleHand));
        }
    
    }
}