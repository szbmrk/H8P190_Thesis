using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private Transform canvas;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowPopup(PopupType type, string text)
    {
        GameObject newPopup = Instantiate(popupPrefab, canvas);
        Popup popup = newPopup.GetComponent<Popup>();
        popup.SetText(type, text);
        popup.ShowPopup();
    }
}
