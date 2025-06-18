using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerEffects
{
    // Positive
    private int strength = 0;
    private int armor_at_end_of_turn = 0;

    // Negative
    private int weakened = 0;
    private int poisoned = 0;

    public void addStrength(int stacks)
    {
        strength += stacks;
    }
}

public class Player : MonoBehaviour
{
    private Deck deck;

    private PlayerEffects currentEffects = new PlayerEffects();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

    public PlayerEffects getPlayerEffects()
    {
        return currentEffects;
    }

    public void processCardEffects(CardEffects effects)
    {
        
    }
}
