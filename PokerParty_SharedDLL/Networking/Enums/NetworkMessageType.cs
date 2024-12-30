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
        DealCardsMessage,
        CommunityCardsChangedMessage,
        GameOverMessage,
        GameInfoMessage
    }
}
