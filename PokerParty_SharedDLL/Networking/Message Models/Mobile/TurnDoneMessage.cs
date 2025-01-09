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
        public int ActionAmount;
        public PossibleAction Action;
        public int NewMoney;

        public override NetworkMessageType Type => NetworkMessageType.TurnDoneMessage;
    }
}
