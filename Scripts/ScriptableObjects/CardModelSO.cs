using System;
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
    public int damage;
    public int armor;
    public int turns;
    public bool special;
    public ConditionTupleEquivalent condition;
    public double multiplier;
    public string details;
    public string title;
    public Sprite image;
    public Target target;
    public int cardsDrawn;
    public int strength;

    public void multiplyValues(int multiplier)
    {
        damage *= multiplier;
        armor *= multiplier;
        strength *= multiplier;
    }
}