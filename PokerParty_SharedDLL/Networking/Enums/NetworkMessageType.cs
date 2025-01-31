namespace PokerParty_SharedDLL
{
    public enum NetworkMessageType
    {
        ChatMessage,
        ConnectionMessage,
        ReadyMessage,
        TurnDoneMessage,
        PlayerNameAlreadyInUseMessage,
        LoadedToGameMessage,
        RefreshedMoneyMessage,
        GameStartedMessage,
        YourTurnMessage,
        NotYourTurnMessage,
        NewTurnStartedMessage,
        DealCardsMessage,
        CommunityCardsChangedMessage,
        GameOverMessage,
        GameInfoMessage
    }
}
