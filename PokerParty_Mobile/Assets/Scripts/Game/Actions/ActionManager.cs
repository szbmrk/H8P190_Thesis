using PokerParty_SharedDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static ActionManager instance;

    public Transform parentForActions;

    public GameObject allInPrefab;
    public GameObject callPrefab;
    public GameObject checkPrefab;
    public GameObject foldPrefab;
    public GameObject raisePrefab;
    public GameObject betPrefab;
    public GameObject smallBlindPrefab;
    public GameObject bigBlindPrefab;

    public TextMeshProUGUI notYourTurnText;

    List<GameObject> currentActions = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void EnableAction(PossibleAction action)
    {
        notYourTurnText.gameObject.SetActive(false);
        switch (action)
        {
            case PossibleAction.SmallBlindBet:
                GameObject smallBlind = Instantiate(smallBlindPrefab, parentForActions);
                smallBlind.GetComponent<ActionButton>().amount = Settings.smallBLindAmount;
                smallBlind.GetComponent<ActionButton>().buttonText.text = $"Small Blind Bet {Settings.smallBLindAmount}$";
                currentActions.Add(smallBlind);
                break;
            case PossibleAction.BigBlindBet:
                GameObject bigBlind = Instantiate(bigBlindPrefab, parentForActions);
                bigBlind.GetComponent<ActionButton>().amount = Settings.bigBLindAmount;
                bigBlind.GetComponent<ActionButton>().buttonText.text = $"Big Blind Bet {Settings.bigBLindAmount}$";
                currentActions.Add(bigBlind);
                break;
            case PossibleAction.Call:
                GameObject call = Instantiate(callPrefab, parentForActions);
                call.GetComponent<ActionButton>().amount = Settings.moneyNeededToCall;
                call.GetComponent<ActionButton>().buttonText.text = $"Call {Settings.moneyNeededToCall}$";
                currentActions.Add(call);
                break;
            case PossibleAction.Check:
                GameObject check = Instantiate(checkPrefab, parentForActions);
                currentActions.Add(check);
                break;
            case PossibleAction.AllIn:
                GameObject allIn = Instantiate(allInPrefab, parentForActions);
                allIn.GetComponent<ActionButton>().amount = GameManager.instance.money;
                currentActions.Add(allIn);
                break;
            case PossibleAction.Fold:
                GameObject fold = Instantiate(foldPrefab, parentForActions);
                currentActions.Add(fold);
                break;
            case PossibleAction.Bet:
                GameObject bet = Instantiate(betPrefab, parentForActions);
                bet.GetComponent<BetAmountGUI>().Initialize(1, GameManager.instance.money);
                currentActions.Add(bet);
                break;
            case PossibleAction.Raise:
                GameObject raise = Instantiate(raisePrefab, parentForActions);
                raise.GetComponent<BetAmountGUI>().Initialize(Settings.moneyNeededToCall + 1, GameManager.instance.money);
                currentActions.Add(raise);
                break;
            default:
                break;
        }
    }

    public void EnableActions(PossibleAction[] possibleActions)
    {
        foreach (PossibleAction action in possibleActions)
        {
            EnableAction(action);
        }
    }

    public void DisableActions()
    {
        currentActions.ForEach(x => Destroy(x));
        notYourTurnText.gameObject.SetActive(true);
    }
}
