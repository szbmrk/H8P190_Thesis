using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class GameOverMessage : ANetworkMessagePc
    {
        public int Place;
        public override NetworkMessageType Type => NetworkMessageType.GameOverMessage;
    }
}
