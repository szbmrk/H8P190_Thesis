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
    public TextMeshProUGUI buttonText;
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
        ActionManager.Instance.DisableActions();
    }

    private void UpdateMoney()
    {
        GameManager.instance.UpdateMoney(GameManager.instance.money - amount);
    }

    private TurnDoneMessage MessageToSend()
    {
        TurnDoneMessage turnDoneMessage = new TurnDoneMessage()
        {
            actionAmount = amount,
            action = action,
            newMoney = GameManager.instance.money,
        };

        return turnDoneMessage;
    }

    private void SetAmount()
    {
        switch (action)
        {
            case PossibleAction.SMALL_BLIND_BET:
                amount = Settings.smallBLindAmount;
                break;
            case PossibleAction.BIG_BLIND_BET:
                amount = Settings.bigBLindAmount;
                break;
            case PossibleAction.CALL:
                amount = Settings.moneyNeededToCall;
                break;
            case PossibleAction.ALL_IN:
                amount = GameManager.instance.money;
                break;
            case PossibleAction.FOLD:
            case PossibleAction.CHECK:
            case PossibleAction.BET:
            case PossibleAction.RAISE:
            default:
                break;
        }
    }
}
