using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager instance;
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private Transform canvas;

    private void Awake()
    {
        instance = this;
    }

    public void ShowPopup(PopupType type, string text)
    {
        GameObject newPopup = Instantiate(popupPrefab, canvas);
        Popup popup = newPopup.GetComponent<Popup>();
        popup.SetText(type, text);
        popup.ShowPopup();
    }
}
