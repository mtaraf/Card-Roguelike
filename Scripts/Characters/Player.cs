using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public enum PlayerClass
{
    Paladin,
    Mistborn
}

public class Player : Character
{
    [SerializeField] private Deck deck = null;
    [SerializeField] private PlayerClass playerClass;

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

    public override void processCardEffects(List<CardEffect> effects, Enemy enemy = null)
    {
        base.processCardEffects(effects, enemy);
    }

    public void setDeck(DeckModelSO deckModel)
    {
        deck = new Deck(deckModel);
        HandManager.instance.setPlayerDeck(deck.deck);
    }

    public PlayerClass getPlayerClass()
    {
        return playerClass;
    }
}
