using System.Collections.Generic;
using UnityEngine;

public class MythicCard
{
    private CardModelSO card;

    public MythicCard(CardModelSO mythic)
    {
        card = mythic.clone();
    }

    public string getMythicName()
    {
        return card.title;
    }

    public void setCard(CardModelSO model)
    {
        card = model.clone();
    }

    public CardModelSO getCard()
    {
        return card;
    }
}

public class Player : Character
{
    [SerializeField] protected Deck deck = null;
    [SerializeField] private PlayerClass playerClass;
    protected MythicCard mythic;
    private int roundEnergy;

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

    public void setMythic(CardModelSO card)
    {
        mythic = new MythicCard(card);
    }

    public MythicCard getMythicCard()
    {
        return mythic;
    }

    public void setEnergy(int energy)
    {
        roundEnergy = energy;
    }

    public int getEnergy()
    {
        return roundEnergy;
    }
}
