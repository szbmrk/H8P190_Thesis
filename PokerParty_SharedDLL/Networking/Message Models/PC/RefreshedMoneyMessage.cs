using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class RefreshedMoneyMessage : ANetworkMessagePc
    {
        public int NewMoney;
        public override NetworkMessageType Type => NetworkMessageType.RefreshedMoneyMessage;
    }
}