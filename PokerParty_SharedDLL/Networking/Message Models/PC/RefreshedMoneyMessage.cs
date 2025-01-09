using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class RefreshedMoneyMessage : ANetworkMessagePC
    {
        public int newMoney;
        public override NetworkMessageType Type => NetworkMessageType.RefreshedMoneyMessage;
    }
}