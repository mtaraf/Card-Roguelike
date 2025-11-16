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
    public string saveName = "Save Name";
    public List<SerializableCardModel> playerCards; // Unique card IDs or titles
    public GameObject playerPrefab;
    public GameObject playerDisplay;
    public List<Tuple<EncounterType, EncounterReward>> pathOptions;
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
    public ConditionTupleEquivalent condition;
    public double multiplier;
    public Target target;
    public int cardsDrawn;
    public bool special;
    public bool corrupts;

    // Optional: Add any fields you care abou
}