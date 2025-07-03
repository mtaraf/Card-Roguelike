using System;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Attack,
    Defense,
    Buff,
    Resource,
    Unique,
    Special
}

public enum CardRarity {
    COMMON,
    RARE,
    EPIC,
    MYTHIC
}

public enum ConditionMetric
{
    HEALTH,
    ARMOR,
    APPLIED_EFFECT,
    PLAYER_CURRENT_EFFECT,
    NO_CONDITION
}

public enum Target
{
    Enemy_Multiple,
    Enemy_Single,
    Player
}

[Serializable]
public class CardEffect
{
    public EffectType type;
    public int value;
    public int turns;
}

[Serializable]
public class ConditionTupleEquivalent
{
    public int conditionValue;
    public ConditionMetric metric;
}

[CreateAssetMenu(fileName = "Card", menuName = "New Card")]
public class CardModelSO : ScriptableObject
{
    public CardType type;
    public CardRarity rarity;
    public int energy;
    public List<CardEffect> effects = new List<CardEffect>();
    public bool special;
    public ConditionTupleEquivalent condition;
    public double multiplier;
    public string details;
    public string title;
    public Sprite image;
    public Target target;
    public int cardsDrawn;
    public bool corrupts;

    public void multiplyValues(int multiplier)
    {
        foreach (CardEffect effect in effects)
        {
            effect.value *= multiplier;
        }
    }
}