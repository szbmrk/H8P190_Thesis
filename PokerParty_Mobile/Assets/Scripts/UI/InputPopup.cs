using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputPopup : Popup
{
    [SerializeField] private Button closeBtn;
    public TMP_InputField inputField;

    Func<Task<bool>> methodToCall;

    protected override void Awake()
    {
        base.Awake();
        base.okButton.onClick.AddListener(OnClickSubmit);
        closeBtn.onClick.AddListener(ClosePopup);
    }

    public void SetData(string header, string inputPlaceHolderText, string buttonText)
    {
        base.popupText.text = header;
        inputField.placeholder.GetComponent<TextMeshProUGUI>().text = inputPlaceHolderText;
        base.okButton.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
    }

    private async void OnClickSubmit()
    {
        if (methodToCall != null) 
        {
            if (await methodToCall.Invoke())
            {
                ClosePopup();
            }
        }
    }

    public void SetOnSubmit(Func<Task<bool>> methodToCall)
    {
        this.methodToCall = methodToCall;
    }
}
