using UnityEngine;

public class Card: MonoBehaviour
{
    public CardType type;
    public int energy;
    public int damage;
    public int armor;
    public int ward;
    public int turns;
    public bool special;
    public Special[] condition;
    public double multiplier;

    public Card(CardModelSO model)
    {
        type = model.type;
        energy = model.energy;
        damage = model.damage;
        armor = model.armor;
        ward = model.ward;
        turns = model.turns;
        special = model.special;
        condition = model.condition;
        multiplier = model.multiplier;
    }
}
