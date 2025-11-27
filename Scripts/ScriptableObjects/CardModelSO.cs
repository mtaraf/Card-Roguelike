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
    Starter,
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
    public int critRate;
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
    public int cardsDiscarded;
    public bool corrupts;
    public CardModelSO upgradedCard;
    public bool oneUse;

    public void multiplyValues(int multiplier)
    {
        foreach (CardEffect effect in effects)
        {
            effect.value *= multiplier;
        }
    }

    public CardModelSO clone()
    {
        CardModelSO copy = CreateInstance<CardModelSO>();

        copy.type = type;
        copy.rarity = rarity;
        copy.energy = energy;
        copy.special = special;
        copy.condition = new ConditionTupleEquivalent
        {
            conditionValue = condition?.conditionValue ?? 0,
            metric = condition?.metric ?? ConditionMetric.NO_CONDITION
        };
        copy.multiplier = multiplier;
        copy.details = details;
        copy.title = title;
        copy.image = image;
        copy.target = target;
        copy.cardsDrawn = cardsDrawn;
        copy.corrupts = corrupts;
        copy.upgradedCard = upgradedCard;
        copy.cardsDiscarded = cardsDiscarded;
        copy.oneUse = oneUse;

        // Clone effects list
        copy.effects = new List<CardEffect>();
        foreach (var effect in effects)
        {
            copy.effects.Add(new CardEffect
            {
                type = effect.type,
                value = effect.value,
                turns = effect.turns
            });
        }

        return copy;
    }
}