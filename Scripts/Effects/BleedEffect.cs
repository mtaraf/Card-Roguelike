using System.Collections.Generic;
using UnityEngine;

public class BleedEffect: IStatusEffect
{
    public EffectType type => EffectType.Bleed;
    public int value;

    public BleedEffect(int effectValue)
    {
        value = effectValue;
    }

    public void apply(Character target, int damageDealt = 0)
    {
        target.addAttributeValue(type, value);

        //target.showFloatingFeedbackUI(type.ToFeedbackString(), Color.blueViolet);
        target.addAudioCue(type);
    }

    public void onEndRound(Character target)
    {
        Dictionary<EffectType, int> attributes = target.getAttributes();
        int bleedValue = attributes[type];

        if (bleedValue > 0)
        {
            target.processDamage(bleedValue, -1, DamageType.Bleed);
            target.updateAttribute(type, 0);
        }

    }
}