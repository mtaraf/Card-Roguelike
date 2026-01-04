using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int currentLevel;
    public int playerMaxHealth;
    public int playerCurrentHealth;
    public int playerHandSize;
    public int playerGold;
    public int playerHandEnergy;
    public int playerCardChoices;
    public int playerCardRarity;
    public PlayerClass playerClass;
    public List<DeckModelSO> victoryCards;
    public string saveName = "Save Name";
    public List<SerializableCardModel> playerCards; // Unique card IDs or titles
    public MythicCard mythicCard;
    public GameObject playerPrefab;
    public GameObject playerDisplay;
    public List<PathOptionData> pathOptions;
    public float musicVolume;
    public float sfxVolume;
}

[Serializable]
public class SerializableCardModel
{
    public string title;
    public string details;
    public CardType type;
    public CardRarity rarity;
    public int energy;
    public List<CardEffect> effects;
    public double multiplier;
    public Target target;
    public int cardsDrawn;
    public bool special;
    public bool corrupts;

    // Optional: Add any fields you care abou
}

[Serializable]
public class PathOptionData
{
    public EncounterType encounterType;
    public EncounterReward encounterReward;
    public int rewardValue;

    public PathOptionData(EncounterType type, EncounterReward reward, int value)
    {
        encounterType = type;
        encounterReward = reward;
        rewardValue = value;
    }
}