using PokerParty_SharedDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerTurnInfo
{
    public Player player;

    public int money = Settings.StartingMoney;
    public int moneyPutInPot = 0;
    public Card[] cards = new Card[2];

    public bool wentAllIn = false;
    public bool folded = false;
    public bool isOutOfGame = false;

    public PlayerTurnInfo(Player player)
    {
        this.player = player;
    }
}