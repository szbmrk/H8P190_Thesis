namespace PokerParty_SharedDLL
{
    public enum NetworkMessageType
    {
        ChatMessage,
        ConnectionMessage,
        GamePausedMessage,
        GameUnpausedMessage,
        ReadyMessage,
        TurnDoneMessage,
        LobbyIsFullMessage,
        PlayerNameAlreadyInUseMessage,
        GameAlreadyStartedMessage,
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
