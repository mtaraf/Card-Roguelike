using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;


public class Player : Character
{
    private Deck deck = null;

    // Attributes

    public override void Start()
    {
        base.Start();
    }

    public bool hasDeck()
    {
        return deck == null;
    }

    public DeckModelSO getCards()
    {
        return deck.getCurrentDeck();
    }

    public void addCardToDeck(CardModelSO card)
    {
        // add card to deck
        deck.addCardToDeck(card);


        // update hand manager
        HandManager.instance.setPlayerDeck(deck.deck);
    }

    private void generateStartingDeck()
    {
        deck.setDeck(GameManager.instance.getStarterDeck());
    }

    public override void processCardEffects(List<CardEffect> effects)
    {
        base.processCardEffects(effects);
    }

    public void setDeck(DeckModelSO deckModel)
    {
        deck = new Deck(deckModel);
        HandManager.instance.setPlayerDeck(deck.deck);
    }
}
