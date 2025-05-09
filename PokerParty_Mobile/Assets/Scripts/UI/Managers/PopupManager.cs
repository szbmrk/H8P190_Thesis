﻿using System;
using System.Threading.Tasks;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager instance;
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private GameObject inputPopupPrefab;
    [SerializeField] private Transform canvas;

    public Popup currentPopup;
    public InputPopup currentInputPopup;

    private void Awake()
    {
        instance = this;
    }

    public void ShowPopup(PopupType type, string text)
    {
        GameObject newPopup = Instantiate(popupPrefab, canvas);
        Popup popup = newPopup.GetComponent<Popup>();
        popup.SetData(type, text);
        popup.ShowPopup();
        currentPopup = popup;
    }

    public void ShowInputPopup(string header, string placeHolderText, string buttonText, Func<Task<bool>> methodToCall)
    {
        GameObject newPopup = Instantiate(inputPopupPrefab, canvas);
        InputPopup popup = newPopup.GetComponent<InputPopup>();
        popup.SetData(header, placeHolderText, buttonText);
        popup.SetOnSubmit(methodToCall);
        popup.ShowPopup();
        currentInputPopup = popup;
    }
}
