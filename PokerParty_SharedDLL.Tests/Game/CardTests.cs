namespace PokerParty_SharedDLL.Tests
{
    public class CardTests
    {
        [Fact]
        public void GetValueString_WhenFaceCard_ShouldReturnCorrectString()
        {
            Assert.Equal("Jack", new Card(11, "Hearts").GetValueString());
            Assert.Equal("Queen", new Card(12, "Diamonds").GetValueString());
            Assert.Equal("King", new Card(13, "Clubs").GetValueString());
            Assert.Equal("A", new Card(14, "Spades").GetValueString());
        }

        [Theory]
        [InlineData(2)]
        [InlineData(10)]
        public void GetValueString_WhenNumberCard_ShouldReturnCorrectString(int value)
        {
            Card card = new Card(value, "Spades");
            Assert.Equal(value.ToString(), card.GetValueString());
        }

        [Fact]
        public void GetFileNameForSprite_WhenCalled_ShouldReturnCorrectFileName()
        {
            Card card = new Card(11, "Hearts");
            Assert.Equal("Hearts_Jack", card.GetFileNameForSprite());
        }

        [Fact]
        public void Equals_WhenSameCard_ShouldReturnTrue()
        {
            Card card1 = new Card(10, "Hearts");
            Card card2 = new Card(10, "Hearts");
            Assert.True(card1.Equals(card2));
        }

        [Fact]
        public void Equals_WhenDifferentCard_ShouldReturnFalse()
        {
            Card card1 = new Card(10, "Hearts");
            Card card2 = new Card(11, "Hearts");
            Assert.False(card1.Equals(card2));
        }

        [Fact]
        public void GetHashCode_WhenSameCard_ShouldReturnSameHashCode()
        {
            Card card1 = new Card(9, "Spades");
            Card card2 = new Card(9, "Spades");
            Assert.Equal(card1.GetHashCode(), card2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_WhenDifferentCards_ShouldReturnDifferentHashCodes()
        {
            Card card1 = new Card(7, "Clubs");
            Card card2 = new Card(8, "Clubs");
            Assert.NotEqual(card1.GetHashCode(), card2.GetHashCode());
        }
    }
}
