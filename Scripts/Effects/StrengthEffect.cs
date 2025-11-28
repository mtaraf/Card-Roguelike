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
        target.addAttributeValue(type, value);

        //target.showFloatingFeedbackUI(type.ToFeedbackString(), Color.blueViolet);
        target.addAudioCue(type);
    }

    public void onEndRound(Character target)
    {
        // Do Nothing!
    }
}