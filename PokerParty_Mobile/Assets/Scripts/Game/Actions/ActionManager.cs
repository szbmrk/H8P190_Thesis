using PokerParty_SharedDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static ActionManager Instance;

    public Transform parentForActions;

    public GameObject allInPrefab;
    public GameObject callPrefab;
    public GameObject checkPrefab;
    public GameObject foldPrefab;
    public GameObject raisePrefab;
    public GameObject betPrefab;
    public GameObject smallBlindPrefab;
    public GameObject bigBlindPrefab;

    List<GameObject> currentActions = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void EnableAction(PossibleAction action)
    {
        switch (action)
        {
            case PossibleAction.SMALL_BLIND_BET:
                GameObject smallBlind = Instantiate(smallBlindPrefab, parentForActions);
                smallBlind.GetComponent<ActionButton>().amount = Settings.smallBLindAmount;
                smallBlind.GetComponent<ActionButton>().buttonText.text = $"Place Small Blind Bet ({Settings.smallBLindAmount} $)";
                currentActions.Add(smallBlind);
                break;
            case PossibleAction.BIG_BLIND_BET:
                GameObject bigBlind = Instantiate(bigBlindPrefab, parentForActions);
                bigBlind.GetComponent<ActionButton>().amount = Settings.bigBLindAmount;
                bigBlind.GetComponent<ActionButton>().buttonText.text = $"Place Big Blind Bet ({Settings.bigBLindAmount} $)";
                currentActions.Add(bigBlind);
                break;
            case PossibleAction.CALL:
                GameObject call = Instantiate(callPrefab, parentForActions);
                call.GetComponent<ActionButton>().amount = Settings.moneyNeededToCall;
                call.GetComponent<ActionButton>().buttonText.text = $"Call ({Settings.moneyNeededToCall} $)";
                currentActions.Add(call);
                break;
            case PossibleAction.CHECK:
                GameObject check = Instantiate(checkPrefab, parentForActions);
                currentActions.Add(check);
                break;
            case PossibleAction.ALL_IN:
                GameObject allIn = Instantiate(allInPrefab, parentForActions);
                allIn.GetComponent<ActionButton>().amount = GameManager.instance.money;
                currentActions.Add(allIn);
                break;
            case PossibleAction.FOLD:
                GameObject fold = Instantiate(foldPrefab, parentForActions);
                currentActions.Add(fold);
                break;
            case PossibleAction.BET:
                GameObject bet = Instantiate(betPrefab, parentForActions);
                bet.GetComponent<BetAmountGUI>().Initialize(1, GameManager.instance.money);
                currentActions.Add(bet);
                break;
            case PossibleAction.RAISE:
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
    }
}
