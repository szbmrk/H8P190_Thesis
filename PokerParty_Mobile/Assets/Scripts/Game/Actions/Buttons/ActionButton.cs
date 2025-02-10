using PokerParty_SharedDLL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    [HideInInspector]
    public int amount;
    public PossibleAction action;

    private Button button;
    [HideInInspector] public TextMeshProUGUI buttonText;
    
    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (button != null)
            button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        SetAmount();
        UpdateMoney();
        MessageSender.SendMessageToHost(MessageToSend());
        ActionManager.instance.DisableActions();
    }

    private void UpdateMoney()
    {
        GameManager.instance.UpdateMoney(GameManager.instance.money - amount);
    }

    private TurnDoneMessage MessageToSend()
    {
        TurnDoneMessage turnDoneMessage = new TurnDoneMessage()
        {
            ActionAmount = amount,
            Action = action,
            NewMoney = GameManager.instance.money,
        };

        return turnDoneMessage;
    }

    private void SetAmount()
    {
        switch (action)
        {
            case PossibleAction.SmallBlindBet:
                amount = Settings.smallBLindAmount;
                break;
            case PossibleAction.BigBlindBet:
                amount = Settings.bigBLindAmount;
                break;
            case PossibleAction.Call:
                amount = Settings.moneyNeededToCall;
                break;
            case PossibleAction.AllIn:
                amount = GameManager.instance.money;
                break;
            case PossibleAction.Fold:
            case PossibleAction.Check:
            case PossibleAction.Bet:
            case PossibleAction.Raise:
            default:
                break;
        }
    }
}
