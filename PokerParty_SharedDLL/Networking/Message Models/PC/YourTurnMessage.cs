using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class YourTurnMessage : ANetworkMessagePC
    {
        public PossibleAction[] PossibleActions { get; set; }
        public int MoneyNeededToCall { get; set; }
        public override NetworkMessageType Type => NetworkMessageType.YourTurnMessage;
    }
}
