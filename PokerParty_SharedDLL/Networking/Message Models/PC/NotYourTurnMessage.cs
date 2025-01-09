using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class NotYourTurnMessage : ANetworkMessagePc
    {
        public string PlayerInTurn;
        public override NetworkMessageType Type => NetworkMessageType.NotYourTurnMessage;
    }
}
