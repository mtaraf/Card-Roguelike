using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDeck", menuName = "Cards/Deck")]
public class DeckModelSO : ScriptableObject
{
    public List<CardModelSO> cards = new List<CardModelSO>();

    public DeckModelSO clone()
    {
        DeckModelSO copy = CreateInstance<DeckModelSO>();

        foreach (CardModelSO cardModel in cards)
        {
            copy.cards.Add(cardModel.clone());
        }

        return copy;
    }

    public void scaleCards(double scaling)
    {
        foreach (CardModelSO cardModel in cards)
        {
            cardModel.scaleValues(scaling);
        }
    }
}