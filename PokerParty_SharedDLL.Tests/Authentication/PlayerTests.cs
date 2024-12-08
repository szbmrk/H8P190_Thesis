using System;
using System.Text;

namespace PokerParty_SharedDLL.Tests
{
    public class PlayerTests
    {
        [Fact]
        public void ToString_WhenCalled_ShouldReturnFormattedString()
        {
            Player player = new Player
            {
                playerName = "John Doe",
                ELO = 1500,
                gamesPlayed = 100,
                gamesWon = 50,
                XP = 2000,
                level = 10
            };

            string expected = "PlayerName: John Doe\n" +
                              "ELO: 1500\n" +
                              "Games Played: 100\n" +
                              "Games Won: 50\n" +
                              "XP: 2000\n" +
                              "Level: 10\n";

            Assert.Equal(expected, player.ToString());
        }

        [Fact]
        public void Equals_WhenSamePlayerName_ShouldReturnTrue()
        {
            Player player1 = new Player { playerName = "John Doe" };
            Player player2 = new Player { playerName = "John Doe" };

            Assert.True(player1.Equals(player2));
        }

        [Fact]
        public void Equals_WhenDifferentPlayerName_ShouldReturnFalse()
        {
            Player player1 = new Player { playerName = "John Doe" };
            Player player2 = new Player { playerName = "Jane Doe" };

            Assert.False(player1.Equals(player2));
        }

        [Fact]
        public void Equals_WhenNullObject_ShouldReturnFalse()
        {
            Player player = new Player { playerName = "John Doe" };

            Assert.False(player.Equals(null));
        }

        [Fact]
        public void GetHashCode_WhenSamePlayerName_ShouldReturnSameHashCode()
        {
            Player player1 = new Player { playerName = "John Doe" };
            Player player2 = new Player { playerName = "John Doe" };

            Assert.Equal(player1.GetHashCode(), player2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_WhenDifferentPlayerNames_ShouldReturnDifferentHashCodes()
        {
            Player player1 = new Player { playerName = "John Doe" };
            Player player2 = new Player { playerName = "Jane Doe" };

            Assert.NotEqual(player1.GetHashCode(), player2.GetHashCode());
        }
    }
}
