using System;
using UnityEngine;

public class WeakenEffect: IStatusEffect
{
    public EffectType type => EffectType.Weaken;
    public int value;

    public WeakenEffect(int effectValue)
    {
        value = effectValue;
    }

    public void apply(Character target, int damageDealt = 0)
    {
        if (target.getAttributeValue(EffectType.Strength) > 0)
        {
            int targetStrength = target.getAttributeValue(EffectType.Strength) - value;
            if (targetStrength < 0)
            {
                target.addAttributeValue(type, Math.Abs(targetStrength));
                target.updateAttribute(EffectType.Strength, 0);
            }
            else
            {
                target.updateAttribute(EffectType.Strength, targetStrength);
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
        // Do nothing!
    }
}