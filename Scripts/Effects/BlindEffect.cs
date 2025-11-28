using UnityEngine;

public class BlindEffect: IStatusEffect
{
    public EffectType type => EffectType.Blind;
    public int value;

    public BlindEffect(int effectValue)
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
        // Do nothing
    }
}