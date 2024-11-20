using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDeck", menuName = "Cards/Deck")]
public class Deck : ScriptableObject
{
    public List<Card> cards = new List<Card>();

    public Card DrawCard()
    {
        if (cards.Count == 0)
        {
            Debug.LogWarning("Deck is empty!");
            return null;
        }

        int index = Random.Range(0, cards.Count);
        Card drawnCard = cards[index];
        cards.RemoveAt(index);
        return drawnCard;
    }
}
