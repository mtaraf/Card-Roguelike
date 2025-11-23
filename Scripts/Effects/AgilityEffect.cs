using System.Collections.Generic;
using UnityEngine;

public class AgilityEffect: IStatusEffect
{
    public EffectType type => EffectType.Agility;
    public int value;

    public AgilityEffect(int effectValue)
    {
        value = effectValue;
    }

    public void apply(Character target, int damageDealt = 0)
    {
        target.updateAttribute(type, value);

        //target.showFloatingFeedbackUI(type.ToFeedbackString(), Color.blueViolet);
        target.addAudioCue(type);
    }

    public void onEndRound(Character target)
    {
        // Lose 25% agility stacks
        Dictionary<EffectType, int> attributes = target.getAttributes();
        target.updateAttribute(type, -(int)(attributes[type] * .25));
    }
}