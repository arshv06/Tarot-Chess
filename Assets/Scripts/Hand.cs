using System;
using System.Collections.Generic;

[Serializable]
public class Hand
{
    public List<Card> cardsInHand = new List<Card>();

    // Event to notify when the hand changes
    public event Action OnHandChanged;

    public void AddCard(Card card)
    {
        cardsInHand.Add(card);
        OnHandChanged?.Invoke(); // Invoke the event
    }

    public void RemoveCard(Card card)
    {
        cardsInHand.Remove(card);
        OnHandChanged?.Invoke(); // Invoke the event
    }
}
