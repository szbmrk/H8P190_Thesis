namespace PokerParty_SharedDLL
{
    public enum NetworkMessageType
    {
        ChatMessage,
        DisconnectMessage,
        ConnectionMessage,
        ReadyMessage,
        TurnDoneMessage,
        LoadedToGameMessage,
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
