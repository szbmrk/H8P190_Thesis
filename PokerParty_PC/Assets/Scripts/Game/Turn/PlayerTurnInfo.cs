using PokerParty_SharedDLL;

public class PlayerTurnInfo
{
    public readonly Player Player;

    public int Money = Settings.startingMoney;
    public int MoneyPutInPot = 0;
    public Card[] Cards = new Card[2];

    public bool WentAllIn = false;
    public bool Folded = false;

    public PlayerTurnInfo(Player player)
    {
        this.Player = player;
    }
}