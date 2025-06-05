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

public enum Special
{
    TakeDamage,
    AttackEnemy
}

[CreateAssetMenu(fileName = "Card", menuName = "New Card")]
public class CardModelSO : ScriptableObject
{
    public CardType type;
    public int energy;
    public int damage;
    public int armor;
    public int ward;
    public int turns;
    public bool special;
    public Special [] condition;
    public double multiplier;
    public string details;
    public string title;
    public Sprite image;
}