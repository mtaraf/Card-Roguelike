using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CardEffect
{
    public EffectType type;
    public int value;
    public int turns;
    public int critRate;

    public CardEffect clone()
    {
        return new CardEffect
        {
            type = this.type,
            value = this.value,
            turns = this.turns,
            critRate = this.critRate,
        };
    }
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
    public string details;
    public string baseDetails;
    public string title;
    public Sprite image;
    public Target target;
    public int cardsDrawn;
    public int cardsDiscarded;
    public bool corrupts;
    public CardModelSO upgradedCard;
    public bool oneUse;
    public bool lithe;

    public void scaleValues(double scaling)
    {
        foreach (CardEffect effect in effects)
        {
            effect.value = (int) (effect.value * scaling);
        }

        setDescription();
    }

    public void setDescription()
    {
        if (baseDetails != null && effects != null && effects.Count > 0)
            details = DescriptionBuilder.buildDescription(baseDetails, effects);
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
        copy.details = details;
        copy.baseDetails = baseDetails;
        copy.title = title;
        copy.image = image;
        copy.target = target;
        copy.cardsDrawn = cardsDrawn;
        copy.corrupts = corrupts;
        copy.upgradedCard = upgradedCard;
        copy.cardsDiscarded = cardsDiscarded;
        copy.oneUse = oneUse;
        copy.lithe = lithe;

        // Clone effects list
        copy.effects = new List<CardEffect>();
        foreach (var effect in effects)
        {
            copy.effects.Add(new CardEffect
            {
                type = effect.type,
                value = effect.value,
                turns = effect.turns,
                critRate = effect.critRate
            });
        }

        copy.setDescription();

        return copy;
    }

    public bool containsEffect(EffectType type)
    {
        foreach (CardEffect effect in effects)
        {
            if (effect.type == type)
                return true;
        }

        return false;
    }

    public List<CardEffect> getEffects()
    {
        return effects.Select(c => c.clone()).ToList();
    }
}