using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsGUI : MonoBehaviour
{
    [SerializeField] private Button openSettingsBtn;
    [SerializeField] private Button closeSettingsBtn;
    [SerializeField] private Button applySettingsBtn;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject mainMenuButtons;

    private void Start()
    {
        openSettingsBtn.onClick.AddListener(OpenSettings);
        closeSettingsBtn.onClick.AddListener(CloseSettings);
        applySettingsBtn.onClick.AddListener(ApplySettings);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && settingsPanel.activeSelf)
        {
            CloseSettings();
        }
    }

    private void OpenSettings()
    {
        mainMenuButtons.SetActive(false);
        settingsPanel.SetActive(true);
    }
    
    private void CloseSettings()
    {
        mainMenuButtons.SetActive(true);
        settingsPanel.SetActive(false);
    }
    
    private void ApplySettings()
    {
        SettingsManager.instance.ApplySettings();
    }
}
