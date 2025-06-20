using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;


public class Player : Character
{
    private Deck deck;

    // Attributes

    public override void Start()
    {
        base.Start();

        // Player Deck set up
        deck = new Deck();
        generateStartingDeck();
        HandManager.instance.setPlayerDeck(deck.deck);
    }

    public DeckModelSO getCards()
    {
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

    public void processCardEffects(CardEffects effects)
    {
        
    }
}
