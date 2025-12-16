using System.Collections.Generic;
using UnityEngine;

public static class DescriptionBuilder
{
    public static string buildDescription(string template, List<CardEffect> effects)
    {
        string result = template;

        foreach (CardEffect effect in effects)
        {
            result = result.Replace("{" + effect.type + "}", effect.value.ToString());
        }

        return result;
    }
}