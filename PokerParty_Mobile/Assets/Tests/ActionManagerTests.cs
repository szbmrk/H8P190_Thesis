using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Linq;
using PokerParty_SharedDLL;

public class ActionManagerTests
{
    private GameObject actionManagerObject;
    private ActionManager actionManager;
    private Transform parentForActions;

    [SetUp]
    public void Setup()
    {
        actionManagerObject = new GameObject();
        actionManager = actionManagerObject.AddComponent<ActionManager>();
        actionManager.parentForActions = new GameObject().transform;
        actionManager.notYourTurnText = new GameObject().AddComponent<TMPro.TextMeshProUGUI>();
        
        actionManager.allInPrefab = new GameObject();
        actionManager.callPrefab = new GameObject();
        actionManager.checkPrefab = new GameObject();
        actionManager.foldPrefab = new GameObject();
        actionManager.raisePrefab = new GameObject();
        actionManager.betPrefab = new GameObject();
        actionManager.smallBlindPrefab = new GameObject();
        actionManager.bigBlindPrefab = new GameObject();
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(actionManagerObject);
    }

    [Test]
    public void EnableActions_ShouldInstantiateCorrectPrefabs()
    {
        PossibleAction[] actions = { PossibleAction.Check, PossibleAction.Fold };
        actionManager.EnableActions(actions);

        Assert.AreEqual(actions.Length, actionManager.parentForActions.childCount);
    }
    
    [Test]
    public void DisableActions_ShouldDestroyAllActions()
    {
        actionManager.DisableActions();
        
        Assert.AreEqual(0, actionManager.parentForActions.childCount);
        Assert.IsTrue(actionManager.notYourTurnText.gameObject.activeSelf);
    }
}