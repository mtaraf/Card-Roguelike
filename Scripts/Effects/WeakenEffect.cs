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
        target.updateAttribute(type, value);

        target.showFloatingFeedbackUI(type.ToFeedbackString(), Color.blueViolet);
        target.addAudioCue(type);
    }

    public void onEndRound(Character target)
    {
        target.decrementEffect(type);
    }
}