namespace PokerParty_SharedDLL.Tests
{
    public class DeckTests
    {
        [Fact]
        public void Constructor_WhenCalled_ShouldContain52Cards()
        {
            Deck deck = new Deck();
            int cardCount = deck.Cards.Count;
            Assert.Equal(52, cardCount);
        }

        [Fact]
        public void Shuffle_WhenCalled_ShouldReorderCardsRandomly()
        {
            Deck deck1 = new Deck();
            Deck deck2 = new Deck();
            deck1.Shuffle();
            deck2.Shuffle();
            Card[] deck1Array = deck1.Cards.ToArray();
            Card[] deck2Array = deck2.Cards.ToArray();
            Assert.NotEqual(deck1Array, deck2Array);
        }

        [Fact]
        public void Draw_WhenCalled_ShouldReturnCardAndDecreaseDeckSize()
        {
            Deck deck = new Deck();
            int initialCount = deck.Cards.Count;
            Card drawnCard = deck.Draw();
            int finalCount = deck.Cards.Count;
            Assert.NotNull(drawnCard);
            Assert.Equal(initialCount - 1, finalCount);
        }

        [Fact]
        public void Draw_WhenDeckEmpty_ShouldThrowInvalidOperationException()
        {
            Deck deck = new Deck();
            while (deck.Cards.Count > 0)
            {
                deck.Draw();
            }
            Assert.Throws<InvalidOperationException>(() => deck.Draw());
        }
    }
}
