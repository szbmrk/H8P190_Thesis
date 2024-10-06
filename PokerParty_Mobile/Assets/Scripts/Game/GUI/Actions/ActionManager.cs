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

    public void EnableActions(PossibleActions[] possibleActions)
    {

    }

    public void DisableActions()
    {
        currentActions.ForEach(x => Destroy(x));
    }
}
