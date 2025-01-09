using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputPopup : Popup
{
    [SerializeField] private Button closeBtn;
    public TMP_InputField inputField;

    private Func<Task<bool>> methodToCall;

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
        if (methodToCall == null) return;
        
        if (await methodToCall.Invoke())
        {
            ClosePopup();
        }
    }

    public void SetOnSubmit(Func<Task<bool>> methodToCall)
    {
        this.methodToCall = methodToCall;
    }
}
