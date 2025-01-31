using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    [SerializeField] private GameObject pausePanel;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    
    public void Pause()
    {
        pausePanel.SetActive(true);
    }
    
    public void Resume()
    {
        pausePanel.SetActive(false);
    }
}