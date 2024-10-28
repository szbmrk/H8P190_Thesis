using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TableGUI : MonoBehaviour
{
    public static TableGUI Instance;
    public List<Transform> seatPositions = new List<Transform>();
    public List<Transform> cardPositions = new List<Transform>();

    private void Awake()
    {
        Instance = this;
    }

    public void DisplayPlayer(TablePlayerCard card, int i)
    {
        card.transform.position = seatPositions[i].position;
        card.gameObject.SetActive(true);
        card.LoadData();
    }

    public void DisplayCard(TableCard card, int i)
    {
        card.transform.position = cardPositions[i].position;
        card.gameObject.SetActive(true);
    }
}
