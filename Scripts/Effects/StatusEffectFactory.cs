public static class StatusEffectFactory
{
    public static IStatusEffect create(CardEffect effect)
    {
        switch (effect.type)
        {
            case EffectType.Agility: return new AgilityEffect(effect.value);
            case EffectType.Armor: return new ArmorEffect(effect.value);
            case EffectType.Bleed: return new BleedEffect(effect.value);
            case EffectType.Blind: return new BlindEffect(effect.value);
            case EffectType.Corruption: return new CorruptionEffect(effect.value);
            case EffectType.Divinity: return new DivinityEffect(effect.value);
            case EffectType.Frostbite: return null;
            case EffectType.Heal: return new HealEffect(effect.value);
            case EffectType.HealDamageDone: return new HealDamageDoneEffect(effect.value);
            case EffectType.HealOverTime: return null;
            case EffectType.Poison: return null;
            case EffectType.Strength: return new StrengthEffect(effect.value);
            case EffectType.Stun: return null;
            case EffectType.Weaken: return new WeakenEffect(effect.value);
            default: return null;
        }
    }

    public static IStatusEffect create(EffectType type)
    {
        switch (type)
        {
            case EffectType.Agility: return new AgilityEffect(0);
            case EffectType.Armor: return new ArmorEffect(0);
            case EffectType.Bleed: return new BleedEffect(0);
            case EffectType.Blind: return new BlindEffect(0);
            case EffectType.Corruption: return new CorruptionEffect(0);
            case EffectType.Divinity: return new DivinityEffect(0);
            case EffectType.Frostbite: return null;
            case EffectType.Heal: return new HealEffect(0);
            case EffectType.HealDamageDone: return new HealDamageDoneEffect(0);
            case EffectType.HealOverTime: return null;
            case EffectType.Poison: return null;
            case EffectType.Strength: return new StrengthEffect(0);
            case EffectType.Stun: return null;
            case EffectType.Weaken: return new WeakenEffect(0);
            default: return null;
        }
    }
}
