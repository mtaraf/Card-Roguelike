using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Character : MonoBehaviour
{
    private Deck deck;
    [SerializeField] private DeckModelSO startingDeck;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (deck == null)
        {
            // Add starting deck if the player deck is empty
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Deck getDeck()
    {
        return deck;
    }

    public void addCardToDeck(CardModelSO card)
    {
        // add card to deck
    }

    private void generateStartingDeck()
    {
        deck.setDeck(startingDeck);
    }
}
