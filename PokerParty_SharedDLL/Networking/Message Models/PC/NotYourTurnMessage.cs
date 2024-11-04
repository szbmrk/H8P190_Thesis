using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class NotYourTurnMessage : ANetworkMessagePC
    {
        public string playerInTurn;
        public override NetworkMessageType Type => NetworkMessageType.NotYourTurnMessage;
    }
}
