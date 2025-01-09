namespace PokerParty_SharedDLL
{
    public enum PossibleAction
    {
        SmallBlindBet, //kötelező tét
        BigBlindBet, //kötelező tét
        Fold, //bedobás
        Check, //passzolás
        Bet, //nyitás
        Call, //tartás
        Raise, //emelés
        AllIn //all in
    }
}
