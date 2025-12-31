using System.Collections.Generic;
using UnityEngine;

public class EffectInformation : MonoBehaviour
{
    [SerializeField] public EffectExecutionType effectExecutionType;
    [SerializeField] public EffectType effectType;
}

public static class EffectTooltipFactory
{
    public static string createCardTooltipDescription(List<CardEffect> effects)
    {
        string description = "";
        foreach (CardEffect effect in effects)
        {
            switch (effect.type)
            {
                case EffectType.Agility:
                    description += $"For each stack of agility, damage is reduced by 5%\n";
                    break;
                case EffectType.Blind:
                    description += $"For each stack of blind, the target has a 10% to miss each attack\n";
                    break;
                case EffectType.Corruption:
                    description += $"Adds half the damage as corruption, the user takes the same amount in damage.\n";
                    break;
                case EffectType.Weaken:
                    description += $"Target will either lose strength stacks or deals 1 less damage per weaken stack.\n";
                    break;
                case EffectType.Strength:
                    description += $"Target will either lose weaken stacks or deals 1 more damage per strength stack.\n";
                    break;
                default:
                    break;
            }
        }
        return description;
    }
}