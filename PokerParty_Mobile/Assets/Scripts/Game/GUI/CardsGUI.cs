using PokerParty_SharedDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CardsGUI : MonoBehaviour
{
    public static CardsGUI Instance;

    [SerializeField] private Image card1;
    [SerializeField] private Image card2;

    [SerializeField] private GameObject switchButton;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void SetCards(Card[] cards)
    {
        switchButton.SetActive(true);
        card1.sprite = GetSpriteByFileName(cards[0].GetFileNameForSprite());
        card2.sprite = GetSpriteByFileName(cards[1].GetFileNameForSprite());
    }

    public Sprite GetSpriteByFileName(string fileName)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Cards");
        foreach (Sprite sprite in sprites)
        {
            if (sprite.name == fileName)
                return sprite;
        }

        return null;
    }
}
