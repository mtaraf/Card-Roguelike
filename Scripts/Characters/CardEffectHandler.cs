using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardEffectHandler
{
    private Character character;

    public CardEffectHandler(Character myCharacter)
    {
        character = myCharacter;    
    }

    public virtual IEnumerator processCardEffects(List<CardEffect> effects, Enemy enemy = null)
    {
        if (enemy != null)
        {
            Debug.Log("Process Card Enemy: " + enemy.name);
        }

        // Damage processing
        int damageDealt = 0;
        List<CardEffect> damage = effects.FindAll((effect) => effect.type == EffectType.Damage);
        if (damage != null && damage.Count > 0)
        {
            foreach (CardEffect effect in damage)
            {
                damageDealt = processDamage(effect.value, effect.critRate, DamageType.General);
                effects.Remove(effect);
                yield return new WaitForSeconds(0.3f);
            }
        }

        foreach (CardEffect effect in effects)
        {
            IStatusEffect statusEffect = StatusEffectFactory.create(effect);
            statusEffect?.apply(character, damageDealt);
        }

        // Check if character is dead
        int currentHealth = character.getCurrentHealth();
        if (currentHealth == 0 || currentHealth < 0)
        {
            character.setCharacterDead();
            HandManager.instance.checkOnDeathEffect();
        }
    }

    public int processDamage(int damage, int critRate, DamageType type)
    {
        int currentHealth = character.getCurrentHealth();
        Dictionary<EffectType, int> attributes = character.getAttributes();
        int damageDealt = 0;

        if (critRate == 100)
        {
            if (GameManager.instance.getPlayerMythicCard() != null && GameManager.instance.getPlayerMythicCard().getMythicName() == "Critical Fate")
            {
                float randomRoll = Random.Range(1.5f, 3.51f);
                damage = (int)(damage * randomRoll);
                Debug.Log($"Dealing {randomRoll} crit damage.");
            }
            else
                damage = (int)(damage * 2.5);
            
            type = DamageType.Critical;
        }

        // Apply agility to damage
        damage = (int)(damage * (1 - attributes[EffectType.Agility] * 0.05));

        // Damage
        if (damage - attributes[EffectType.Armor] > 0)
        {
            currentHealth -= damage - attributes[EffectType.Armor];
            damageDealt = damage - attributes[EffectType.Armor];

            character.showFloatingFeedbackUI(type, damageDealt.ToString());
            character.updateAttribute(EffectType.Armor, 0);
            character.playHitAnimation();
        }
        else
        {
            character.updateAttribute(EffectType.Armor, attributes[EffectType.Armor] - damage);
            character.showFloatingFeedbackUI(type, damage.ToString());
            AudioManager.instance.playBlock();
        }

        // Update Health
        character.updateCurrentHealth(currentHealth);

        return damageDealt;
    }


    // void checkFrostbiteThreshold(int damageDealt)
    // {
    //     // Check Frostbite threshold
    //     if (attributes[EffectType.Frostbite] > 0 && damageDealt > 0)
    //     {
    //         attributes[EffectType.Frostbite] -= damageDealt;

    //         // Check if threshold is met
    //         if (attributes[EffectType.Frostbite] <= 0)
    //         {
    //             // Do frostbite damage and frostbite animation
    //             attributes[EffectType.Frostbite] = 0;
    //             int frostbiteDamage = (int)(maxHealth * 0.1f);
    //             currentHealth -= frostbiteDamage;
    //             //showFloatingFeedbackUI(frostbiteDamage.ToString(), Color.powderBlue);
    //             uIUpdater.setHealth(currentHealth, maxHealth);
    //         }

    //         // Updates the frostbite effect to either remove or lessen threshold
    //         uIUpdater.updateEffect(EffectType.Frostbite, attributes[EffectType.Frostbite]);
    //     }
    // }
}
