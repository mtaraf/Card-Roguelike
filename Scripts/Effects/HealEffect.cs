using UnityEngine;

public class HealEffect: IStatusEffect
{
    public EffectType type => EffectType.Heal;
    public int value;

    public HealEffect(int effectValue)
    {
        value = effectValue;
    }

    public void apply(Character target, int damageDealt = 0)
    {
        target.healCharacter(value);

        target.showFloatingFeedbackUI(type.ToFeedbackString(), Color.blueViolet);
        target.addAudioCue(type);
    }

    public void onEndRound(Character target)
    {
        // Do Nothing!
    }
}