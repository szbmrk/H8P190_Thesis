using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParty_SharedDLL
{
    public class GameInfoMessage : ANetworkMessagePC
    {
        public bool isDealer;
        public bool isSmallBlind;
        public bool isBigBlind;

        public int smallBlindAmount;
        public int bigBlindAmount;
        public int startingMoney;

        public override NetworkMessageType Type => NetworkMessageType.GameInfoMessage;
    }
}
