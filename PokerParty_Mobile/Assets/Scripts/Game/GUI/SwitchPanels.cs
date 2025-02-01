using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwitchPanels : MonoBehaviour
{
    [SerializeField] private GameObject actionsPanel;
    [SerializeField] private GameObject cardsPanel;

    [SerializeField] private Button switchButton;
    [SerializeField] private TextMeshProUGUI switchText;

    private void Start()
    {
        switchButton.onClick.AddListener(Switch);
    }

    private void Switch()
    {
        if (actionsPanel.activeInHierarchy)
            SwitchToCards();
        else
            SwitchToActions();
    }

    private void SwitchToCards()
    {
        switchText.text = "Show actions";
        actionsPanel.SetActive(false);
        cardsPanel.SetActive(true);
    }

    private void SwitchToActions()
    {
        switchText.text = "Show cards";
        actionsPanel.SetActive(true);
        cardsPanel.SetActive(false);
    }

}
