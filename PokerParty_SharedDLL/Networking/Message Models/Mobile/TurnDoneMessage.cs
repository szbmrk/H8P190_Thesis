using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class TurnDoneMessage : ANetworkMessageMobile
    {
        public int Amount { get; set; }
        public PossibleAction Action { get; set; }

        public override NetworkMessageType Type => NetworkMessageType.TurnDoneMessage;
    }
}
