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
        isDealer = gameInfo.isDealer;
        isSmallBlind = gameInfo.isSmallBlind;
        isBigBlind = gameInfo.isBigBlind;
        smallBLindAmount = gameInfo.smallBlindAmount;
        bigBLindAmount = gameInfo.bigBlindAmount;
    }
}