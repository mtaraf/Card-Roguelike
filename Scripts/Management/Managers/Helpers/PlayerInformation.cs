using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation
{
    public int playerMaxHealth;
    public int playerCurrentHealth;
    public int playerHandEnergy;
    public int playerGold;
    public int playerHandSize;
    public int playerCardRarity;
    public int playerCardChoices;
    public DeckModelSO playerDeck;
    public MythicCard playerMythic;
    public PlayerClass playerClass;
    public List<DeckModelSO> victoryCardPools = new List<DeckModelSO>();

    public PlayerInformation(int maxHealth, int currentHealth, int energy, int gold, int handSize, DeckModelSO deck, MythicCard mythic, PlayerClass playerSelectedClass, List<DeckModelSO> victoryCards)
    {
        playerCurrentHealth = currentHealth;
        playerMaxHealth = maxHealth;
        playerHandEnergy = energy;
        playerGold = gold;
        playerHandSize = handSize;
        playerMythic = mythic;
        playerClass = playerSelectedClass;

        playerCardChoices = 3;
        playerCardRarity = 0;

        if (deck == null)
            playerDeck = cloneDeck(Resources.Load<DeckModelSO>("ScriptableObjects/Cards/Mistborn/StarterCards/StarterDeck"));
        else
            playerDeck = deck;

        if (victoryCards == null)
            instantiateVictoryPoolCards(playerSelectedClass);
        else
            victoryCardPools = victoryCards;
    }

    private void instantiateVictoryPoolCards(PlayerClass playerClass)
    {
        string path;
        switch (playerClass)
        {
            case PlayerClass.Mistborn:
                path = "ScriptableObjects/Cards/Mistborn/VictoryCards/";
                break;
            default:
                path = "ScriptableObjects/Decks/";
                break;
        }

        victoryCardPools.Insert(0, cloneDeck(Resources.Load<DeckModelSO>(path + "CommonVictoryCards")));
        victoryCardPools.Insert(1, cloneDeck(Resources.Load<DeckModelSO>(path + "RareVictoryCards")));
        victoryCardPools.Insert(2, cloneDeck(Resources.Load<DeckModelSO>(path + "EpicVictoryCards")));
        victoryCardPools.Insert(3, cloneDeck(Resources.Load<DeckModelSO>(path + "MythicVictoryCards")));
    }

    private DeckModelSO cloneDeck(DeckModelSO deck)
    {
        DeckModelSO clonedDeck = ScriptableObject.CreateInstance<DeckModelSO>();
        clonedDeck.cards = new List<CardModelSO>();

        foreach (CardModelSO card in deck.cards)
        {
            clonedDeck.cards.Add(card.clone());
        }

        return clonedDeck;
    }
}