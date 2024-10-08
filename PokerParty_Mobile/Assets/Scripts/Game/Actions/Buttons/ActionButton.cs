using PokerParty_SharedDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    [HideInInspector]
    public int amount = 0;
    public PossibleAction action;

    Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }
    private void OnClick()
    {
        SetAmount();
        ActionManager.Instance.DisableActions();
        MessageSender.SendMessageToHost(MessageToSend());
    }

    public TurnDoneMessage MessageToSend()
    {
        TurnDoneMessage turnDoneMessage = new TurnDoneMessage()
        {
            Amount = amount,
            Action = action
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
                amount = Settings.PreviousBet;
                break;
            case PossibleAction.ALL_IN:
                amount = GameManager.Instance.money;
                break;
            default:
                break;
        }
    }
}
