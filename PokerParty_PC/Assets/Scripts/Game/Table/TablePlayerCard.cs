using PokerParty_SharedDLL;
using TMPro;
using UnityEngine;

public class TablePlayerCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI moneyPutInText;

    [SerializeField] private GameObject dealerIcon;
    [SerializeField] private GameObject smallBlindIcon;
    [SerializeField] private GameObject bigBlindIcon;
    [SerializeField] private GameObject turnIcon;
    [SerializeField] private GameObject outOfTurn;

    [HideInInspector] public PlayerTurnInfo TurnInfo;
    [HideInInspector] public int indexInConnectionsArray;

    [HideInInspector] public bool isDealer;
    [HideInInspector] public bool isSmallBlind;
    [HideInInspector] public bool isBigBlind;

    [HideInInspector] public bool isTurn;

    public bool isStillInGame => TurnInfo.Money > 0 && !TurnInfo.Folded && !TurnInfo.WentAllIn;

    public void LoadData()
    {
        playerNameText.color = PlayerColorManager.GetColor(TurnInfo.Player.PlayerName);
        playerNameText.text = TurnInfo.Player.PlayerName;

        SetRoleIcons();
        RefreshMoney(Settings.startingMoney);
        RefreshTurnIcon();
    }

    public void Reset()
    {
        isDealer = false;
        isSmallBlind = false;
        isBigBlind = false;
        isTurn = false;
        TurnInfo.MoneyPutInPot = 0;
        TurnInfo.Cards = new Card[2];
        TurnInfo.Folded = false;
        TurnInfo.WentAllIn = false;
        moneyPutInText.text = "Put in: 0 $";
        outOfTurn.SetActive(false);

        if (TurnInfo.Money <= 0 && gameObject.activeInHierarchy)
        {
            OutOfTurn();
            GameManager.instance.SendGameOverMessageToPlayer(indexInConnectionsArray);
        }

        RefreshTurnIcon();
    }

    public void OutOfGame()
    {
        Reset();
        gameObject.SetActive(false);
    }
    
    public void Disconnected()
    {
        OutOfGame();
        TurnDoneMessage turnDoneMessage = new TurnDoneMessage();
        turnDoneMessage.action = PossibleAction.FOLD;
        TurnManager.instance.HandleTurnDone(turnDoneMessage);
    }

    public void SetRoleIcons()
    {
        dealerIcon.SetActive(isDealer);
        smallBlindIcon.SetActive(isSmallBlind);
        bigBlindIcon.SetActive(isBigBlind);
    }

    private void RefreshTurnIcon()
    {
        turnIcon.SetActive(isTurn);
        playerNameText.text = isTurn ? $"<b>{TurnInfo.Player.PlayerName}</b>" : TurnInfo.Player.PlayerName;
    }

    public void RefreshMoney(int money)
    {
        moneyText.text = $"{money} $";
    }

    public void RefreshMoneyPutIn(int money)
    {
        moneyPutInText.text = $"Put in: {money} $";
    }

    public void StartTurn()
    {
        isTurn = true;
        GameManager.instance.SetWaitingFor(TurnInfo.Player.PlayerName);
        RefreshTurnIcon();
    }

    public void TurnDone()
    {
        isTurn = false;
        RefreshTurnIcon();
    }

    public void OutOfTurn()
    {
        outOfTurn.SetActive(true);
    }

    public override bool Equals(object other)
    {
        if (other == null || !(other is TablePlayerCard))
            return false;

        TablePlayerCard otherPlayer = other as TablePlayerCard;

        return otherPlayer.TurnInfo.Player.Equals(TurnInfo.Player);
    }

    public override int GetHashCode()
    {
        return TurnInfo.Player.GetHashCode();
    }
}
