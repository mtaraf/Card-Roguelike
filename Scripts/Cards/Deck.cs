using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] public int maxDeckSize = 20;
    [SerializeField] public int deckSize = 0;
    [SerializeField] public DeckModelSO deck;

    private List<CardModelSO> discardPile = new List<CardModelSO>();
    private List<CardModelSO> drawPile;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //drawPile = deck.cards;
    }

    // Update is called once per frame
    void Update()
    {

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
        Debug.Log("Deck updated: " + deck);
    }
}
