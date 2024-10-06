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

    List<GameObject> currentActions;

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
                currentActions.Add(Instantiate(smallBlindPrefab, parentForActions));
                break;
            case PossibleAction.BIG_BLIND_BET:
                currentActions.Add(Instantiate(bigBlindPrefab, parentForActions));
                break;
            case PossibleAction.CALL:
                currentActions.Add(Instantiate(callPrefab, parentForActions));
                break;
            case PossibleAction.CHECK:
                currentActions.Add(Instantiate(checkPrefab, parentForActions));
                break;
            case PossibleAction.ALL_IN:
                currentActions.Add(Instantiate(allInPrefab, parentForActions));
                break;
            case PossibleAction.FOLD:
                currentActions.Add(Instantiate(foldPrefab, parentForActions));
                break;
            case PossibleAction.BET:
                currentActions.Add(Instantiate(betPrefab, parentForActions));
                break;
            case PossibleAction.RAISE:
                currentActions.Add(Instantiate(raisePrefab, parentForActions));
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
