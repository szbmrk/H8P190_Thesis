using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParty_SharedDLL
{
    public class GameInfoMessage : ANetworkMessagePC
    {
        public bool IsDealer { get; set; }
        public bool IsSmallBlind { get; set; }
        public bool IsBigBlind { get; set; }

        public int SmallBlindAmount { get; set; }
        public int BigBlindAmount { get; set; }
        public int StartingMoney { get; set; }

        public override NetworkMessageType Type => NetworkMessageType.GameInfoMessage;
    }
}
