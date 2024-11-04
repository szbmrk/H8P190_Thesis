using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParty_SharedDLL
{
    public class DealCardsMessage : ANetworkMessagePC
    {
        public Card[] cards;
        public override NetworkMessageType Type => NetworkMessageType.DealCardsMessage;
    }
}
