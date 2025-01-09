using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class YourTurnMessage : ANetworkMessagePc
    {
        public PossibleAction[] PossibleActions;
        public int MoneyNeededToCall;
        public override NetworkMessageType Type => NetworkMessageType.YourTurnMessage;
    }
}
