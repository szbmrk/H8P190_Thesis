using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using PokerParty_SharedDLL;

public class TableCard : MonoBehaviour
{
    public Card card;

    private Vector3 originalScale;
    
    private void Start()
    {
        originalScale = transform.localScale;
    }

    public IEnumerator Flip()
    {
        const float flipDuration = 0.25f;
        
        LeanTween.scaleX(gameObject, 0f, flipDuration / 2)
            .setEase(LeanTweenType.easeInOutQuad);

        yield return new WaitForSeconds(flipDuration / 2);

        GetComponent<SpriteRenderer>().sprite = GetSpriteByFileName(card.GetFileNameForSprite());;

        LeanTween.scaleX(gameObject, originalScale.x, flipDuration / 2)
            .setEase(LeanTweenType.easeInOutQuad);

        yield return new WaitForSeconds(flipDuration / 2);
    }

    private Sprite GetSpriteByFileName(string fileName)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Cards");
        return sprites.FirstOrDefault(sprite => sprite.name == fileName);
    }
}