public interface IStatusEffect
{
    EffectType type { get; }
    void apply(Character target, int damageDealt = 0);
    void onEndRound(Character target);
}