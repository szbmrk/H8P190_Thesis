using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class GameInfoMessage : ANetworkMessagePc
    {
        public bool IsDealer;
        public bool IsSmallBlind;
        public bool IsBigBlind;

        public int SmallBlindAmount;
        public int BigBlindAmount;
        public int StartingMoney;

        public override NetworkMessageType Type => NetworkMessageType.GameInfoMessage;
    }
}
