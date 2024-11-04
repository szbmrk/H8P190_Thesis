using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using PokerParty_SharedDLL;

public class TableCard : MonoBehaviour
{
    public Card card;

    public void Flip()
    {
        GetComponent<SpriteRenderer>().sprite = GetSpriteByFileName(card.GetFileNameForSprite());
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