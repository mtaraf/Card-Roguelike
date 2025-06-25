using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Deck
{
    public int maxDeckSize = 20;
    public int deckSize = 0;
    public DeckModelSO deck;

    public Deck(DeckModelSO model)
    {
        deck = model;
    }
    void Start()
    {
        // Initialize empty deck
        deck = ScriptableObject.CreateInstance<DeckModelSO>();
        deck.cards = new List<CardModelSO>();
    }

    void AddCardToDeck(CardModelSO card)
    {
        if (deckSize != maxDeckSize)
        {
            deck.cards.Add(card);
        }
    }

    void drawCard(CardModelSO card)
    {
        // foreach (GameObject slot in cardSlots)
        // {
        //     if (slot.transform.childCount == 0)
        //     {
        //         // Add Card to this slot

        //         break;
        //     }
        // }
    }

    public void setDeck(DeckModelSO cardList)
    {
        deck = cardList;
    }

    public DeckModelSO getCurrentDeck()
    {
        return deck;
    }
}
