using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParty_SharedDLL.Tests
{
    public class DeckTests
    {
        [Fact]
        public void Constructor_WhenCalled_ShouldContain52Cards()
        {
            Deck deck = new Deck();
            int cardCount = deck.cards.Count;
            Assert.Equal(52, cardCount);
        }

        [Fact]
        public void Shuffle_WhenCalled_ShouldReorderCardsRandomly()
        {
            Deck deck1 = new Deck();
            Deck deck2 = new Deck();
            deck1.Shuffle();
            deck2.Shuffle();
            Card[] deck1Array = deck1.cards.ToArray();
            Card[] deck2Array = deck2.cards.ToArray();
            Assert.NotEqual(deck1Array, deck2Array);
        }

        [Fact]
        public void Draw_WhenCalled_ShouldReturnCardAndDecreaseDeckSize()
        {
            Deck deck = new Deck();
            int initialCount = deck.cards.Count;
            Card drawnCard = deck.Draw();
            int finalCount = deck.cards.Count;
            Assert.NotNull(drawnCard);
            Assert.Equal(initialCount - 1, finalCount);
        }

        [Fact]
        public void Draw_WhenDeckEmpty_ShouldThrowInvalidOperationException()
        {
            Deck deck = new Deck();
            while (deck.cards.Count > 0)
            {
                deck.Draw();
            }
            Assert.Throws<InvalidOperationException>(() => deck.Draw());
        }
    }
}
