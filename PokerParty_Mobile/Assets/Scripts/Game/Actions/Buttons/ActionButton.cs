using PokerParty_SharedDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    [HideInInspector]
    public int amount = 0;
    public PossibleAction action;

    Button button;
    public TextMeshProUGUI buttonText;
    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
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
        GameManager.Instance.UpdateMoney(GameManager.Instance.money - amount);
    }

    public TurnDoneMessage MessageToSend()
    {
        TurnDoneMessage turnDoneMessage = new TurnDoneMessage()
        {
            actionAmount = amount,
            action = action,
            newMoney = GameManager.Instance.money,
        };

        return turnDoneMessage;
    }

    private void SetAmount()
    {
        switch (action)
        {
            case PossibleAction.SMALL_BLIND_BET:
                amount = Settings.SmallBLindAmount;
                break;
            case PossibleAction.BIG_BLIND_BET:
                amount = Settings.BigBLindAmount;
                break;
            case PossibleAction.CALL:
                amount = Settings.MoneyNeededToCall;
                break;
            case PossibleAction.ALL_IN:
                amount = GameManager.Instance.money;
                break;
            default:
                break;
        }
    }
}
