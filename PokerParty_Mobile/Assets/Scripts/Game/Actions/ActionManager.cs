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

    private void EnableAction(PossibleActions action)
    {
        switch (action)
        {
            case PossibleActions.SMALL_BLIND_BET:
                currentActions.Add(Instantiate(smallBlindPrefab, parentForActions));
                break;
            case PossibleActions.BIG_BLIND_BET:
                currentActions.Add(Instantiate(bigBlindPrefab, parentForActions));
                break;
            case PossibleActions.CALL:
                currentActions.Add(Instantiate(callPrefab, parentForActions));
                break;
            case PossibleActions.CHECK:
                currentActions.Add(Instantiate(checkPrefab, parentForActions));
                break;
            case PossibleActions.ALL_IN:
                currentActions.Add(Instantiate(allInPrefab, parentForActions));
                break;
            case PossibleActions.FOLD:
                currentActions.Add(Instantiate(foldPrefab, parentForActions));
                break;
            case PossibleActions.BET:
                currentActions.Add(Instantiate(betPrefab, parentForActions));
                break;
            case PossibleActions.RAISE:
                currentActions.Add(Instantiate(raisePrefab, parentForActions));
                break;
            default:
                break;
        }
    }

    public void EnableActions(PossibleActions[] possibleActions)
    {
        foreach (PossibleActions action in possibleActions)
        {
            EnableAction(action);
        }
    }

    public void DisableActions()
    {
        currentActions.ForEach(x => Destroy(x));
    }
}
