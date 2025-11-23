using UnityEngine;

public class CorruptionEffect: IStatusEffect
{
    public EffectType type => EffectType.Corruption;
    public int value;

    public CorruptionEffect(int effectValue)
    {
        value = effectValue;
    }

    public void apply(Character target, int damageDealt = 0)
    {
        target.updateAttribute(type, damageDealt);

        // Player takes half the damage dealt, can mitigate this damage.
        ParentSceneController parentSceneController = GameManager.instance.getCurrentSceneController();
        Player player = parentSceneController.getPlayer();
        player.processDamage(damageDealt / 2, -1, DamageType.Corruption);

        //target.showFloatingFeedbackUI(type.ToFeedbackString(), Color.blueViolet);
        target.addAudioCue(type);
    }

    public void onEndRound(Character target)
    {
        // Do Nothing!
    }
}