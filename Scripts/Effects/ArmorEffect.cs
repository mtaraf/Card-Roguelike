using UnityEngine;

public class ArmorEffect: IStatusEffect
{
    public EffectType type => EffectType.Armor;
    public int value;

    public ArmorEffect(int effectValue)
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
        // Do nothing
    }
}