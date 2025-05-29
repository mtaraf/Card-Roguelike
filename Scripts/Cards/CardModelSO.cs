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

[CreateAssetMenu(fileName = "Card", menuName = "New Card")]
public class CardModelSO : ScriptableObject
{
    public CardType type;
    public int energy;
    public int damage;
    public int armor;
    public int ward;
    public double multiplier;
    public string text;
    public string title;
    public Sprite image;
}
