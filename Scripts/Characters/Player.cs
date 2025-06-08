using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Deck deck;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deck = new Deck();
        generateStartingDeck();
        HandManager.instance.setPlayerDeck(deck.deck);
    }

    public DeckModelSO getCards()
    {
        Debug.Log(deck);
        return deck.getCurrentDeck();
    }

    public void addCardToDeck(CardModelSO card)
    {
        // add card to deck
    }

    private void generateStartingDeck()
    {
        deck.setDeck(GameManager.instance.getStarterDeck());
    }
}
