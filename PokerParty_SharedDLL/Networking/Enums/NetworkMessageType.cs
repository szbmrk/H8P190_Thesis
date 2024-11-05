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
        EveryoneLoadedMessage,
        YourTurnMessage,
        NotYourTurnMessage,
        DealCardsMessage,
        CommunityCardsChangedMessage,
        GameOverMessage,
        GameInfoMessage
    }
}
