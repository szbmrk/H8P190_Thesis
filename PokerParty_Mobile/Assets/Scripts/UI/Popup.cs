using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI popupText;
    [SerializeField] protected Button okButton;

    public float animationDuration = 0.5f;
    private Vector3 originalScale;

    protected virtual void Awake()
    {
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    public void SetData (PopupType type, string text)
    {
        switch (type)
        {
            case PopupType.ErrorPopup:
                popupText.color = new Color(1f, 0.32f, 0.3f, 1f);
                break;
            case PopupType.SuccessPopup:
                popupText.color = new Color(0.3f, 1f, 0.32f);
                break;
            case PopupType.BasicPopup:
                popupText.color = Color.white;
                break;
            default:
                break;
        }

        popupText.text = text;
        okButton.onClick.AddListener(ClosePopup);
    }

    public void ShowPopup()
    {
        LeanTween.scale(gameObject, originalScale, animationDuration).setEase(LeanTweenType.easeOutBack);
    }

    protected void ClosePopup()
    {
        LeanTween.scale(gameObject, Vector3.zero, animationDuration).setEase(LeanTweenType.easeInBack).setOnComplete(OnPopupClosed);
    }

    private void OnPopupClosed()
    {
        Destroy(gameObject);
    }
}
