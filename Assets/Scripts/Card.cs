using UnityEngine;

public abstract class Card : ScriptableObject
{
    public string cardName;
    public string description;
    public Sprite artwork;
    public CardRarity rarity;

    public abstract void PlayCard(GameManager gameManager, bool isWhitePlayer, System.Action onEffectComplete);
}

public enum CardRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}
