using UnityEngine;

public class DivinityEffect: IStatusEffect
{
    public EffectType type => EffectType.Divinity;
    public int value;

    public DivinityEffect(int effectValue)
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