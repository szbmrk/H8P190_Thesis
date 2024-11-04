using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParty_SharedDLL
{
    public class CommunityCardsChanged : ANetworkMessagePC
    {
        public Card[] communityCards;
        public override NetworkMessageType Type => NetworkMessageType.CommunityCardsChangedMessage;
    }
}
