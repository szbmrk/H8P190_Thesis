using System.Linq;
using UnityEngine;
using PokerParty_SharedDLL;

public class TableCard : MonoBehaviour
{
    public Card card;

    public void Flip()
    {
        GetComponent<SpriteRenderer>().sprite = GetSpriteByFileName(card.GetFileNameForSprite());
    }

    private Sprite GetSpriteByFileName(string fileName)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Cards");
        return sprites.FirstOrDefault(sprite => sprite.name == fileName);
    }
}