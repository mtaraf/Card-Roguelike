using System;
using UnityEngine;

public class StrengthEffect: IStatusEffect
{
    public EffectType type => EffectType.Strength;
    public int value;

    public StrengthEffect(int effectValue)
    {
        value = effectValue;
    }

    public void apply(Character target, int damageDealt = 0)
    {
        if (target.getAttributeValue(EffectType.Weaken) > 0)
        {
            int targetWeakness = target.getAttributeValue(EffectType.Weaken) - value;
            if (targetWeakness < 0)
            {
                target.addAttributeValue(type, Math.Abs(targetWeakness));
                target.updateAttribute(EffectType.Weaken, 0);
            }
            else
            {
                target.updateAttribute(EffectType.Weaken, targetWeakness);
            }
        }
        else
        {
            target.addAttributeValue(type, value);
        }

        target.addAudioCue(type);
    }

    public void onEndRound(Character target)
    {
        // Do Nothing!
    }
}