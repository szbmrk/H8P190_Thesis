using PokerParty_SharedDLL;

public static class Settings
{
    public static bool isDealer;
    public static bool isSmallBlind;
    public static bool isBigBlind;
    
    public static int smallBLindAmount;
    public static int bigBLindAmount;

    public static int moneyNeededToCall;

    public static void SetSettings(GameInfoMessage gameInfo)
    {
        isDealer = gameInfo.IsDealer;
        isSmallBlind = gameInfo.IsSmallBlind;
        isBigBlind = gameInfo.IsBigBlind;
        smallBLindAmount = gameInfo.SmallBlindAmount;
        bigBLindAmount = gameInfo.BigBlindAmount;
    }
}