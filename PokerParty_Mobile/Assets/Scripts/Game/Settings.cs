using PokerParty_SharedDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Settings
{
    public static bool IsDealer;
    public static bool IsSmallBlind;
    public static bool IsBigBlind;
    
    public static int SmallBLindAmount;
    public static int BigBLindAmount;

    public static int MoneyNeededToCall;

    public static void SetSettings(GameInfoMessage gameInfo)
    {
        IsDealer = gameInfo.IsDealer;
        IsSmallBlind = gameInfo.IsSmallBlind;
        IsBigBlind = gameInfo.IsBigBlind;
        SmallBLindAmount = gameInfo.SmallBlindAmount;
        BigBLindAmount = gameInfo.BigBlindAmount;
    }
}