using UnityEngine;

public class HealDamageDoneEffect: IStatusEffect
{
    public EffectType type => EffectType.HealDamageDone;
    public int value;

    public HealDamageDoneEffect(int effectValue)
    {
        value = effectValue;
    }

    public void apply(Character target, int damageDealt = 0)
    {
        target.healCharacter(damageDealt);

        target.showFloatingFeedbackUI(DamageType.Heal, damageDealt.ToString());
        target.addAudioCue(type);
    }

    public void onEndRound(Character target)
    {
        // Do nothing!
    }
}