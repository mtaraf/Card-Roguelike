using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] public int maxDeckSize = 20;
    [SerializeField] public int deckSize = 0;
    [SerializeField] public List<CardModelSO> deck;
    [SerializeField] private GameObject deckSizeText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void AddCardToDeck(CardModelSO card)
    {
        if (deckSize != maxDeckSize)
        {
            deck.Add(card);
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
}
