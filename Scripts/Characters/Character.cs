using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Effects
{
    POISON,
    FREEZE,
    THORNS
}

public enum Attributes
{
    STRENGTH = 0,
    ARMOR = 1
}

public static class AttributesExtensions
{
    public static string ToDisplayString(this Attributes att)
    {
        switch (att)
        {
            case Attributes.STRENGTH: return "StrengthEffect";
            case Attributes.ARMOR: return "ArmorEffect";
            default: return "Default";
        }
    }
}


public class Character : MonoBehaviour
{
    // Attributes
    [SerializeField] protected int maxHealth;
    protected int currentHealth; // Set current health to max health only when a run starts not in start for player
    protected int weakness;
    protected Dictionary<Attributes, int> attributes = new Dictionary<Attributes, int>();
    protected List<CardEffects> multipleTurnEffects = new List<CardEffects>();

    // UI
    protected HealthAndStatus healthAndStatus;
    protected TargetableObject targetableObject;

    public virtual void Start()
    {
        // Initializers
        targetableObject = GetComponent<TargetableObject>();

        currentHealth = maxHealth;
        attributes.Add(Attributes.STRENGTH, 0);
        attributes.Add(Attributes.ARMOR, 0);

        // Set up after first frame
        StartCoroutine(findComponentsAfterFrame());
    }

    private IEnumerator findComponentsAfterFrame()
    {
        yield return null;

        // UI set up
        healthAndStatus = GetComponent<HealthAndStatus>();
        if (healthAndStatus == null)
        {
            Debug.LogError("Could not find Health and Status for this obj: " + name);
        }
        else
        {
            healthAndStatus.setHealth(currentHealth, maxHealth);
        }
    }

    public virtual void processCardEffects(CardEffects effects)
    {
        int armor = attributes[Attributes.ARMOR];

        if (effects.Turns > 0)
        {
            multipleTurnEffects.Add(effects);
        }
        else
        {
            Debug.Log("Armor: " + effects.getEffect(EffectType.Armor));
            Debug.Log("Current Armor: " + attributes[Attributes.ARMOR]);
            Debug.Log("Damage: " + effects.getEffect(EffectType.Damage));
            // Apply changes to attributes

            // Apply damage through armor
            if (effects.getEffect(EffectType.Damage) - armor > 0)
            {
                attributes[Attributes.ARMOR] = 0;
                currentHealth -= effects.getEffect(EffectType.Damage) - armor;
            }
            else
            {
                attributes[Attributes.ARMOR] = armor - effects.getEffect(EffectType.Damage);
            }


            attributes[Attributes.ARMOR] += effects.getEffect(EffectType.Armor);
            attributes[Attributes.STRENGTH] += effects.getEffect(EffectType.Strength);
        }

        healthAndStatus.updateAttributes(false, attributes);
        healthAndStatus.setHealth(currentHealth, maxHealth);
    }

    public void processStartOfTurnEffects()
    {
        List<CardEffects> turnEffectsCopy = new List<CardEffects>(multipleTurnEffects);
        foreach (CardEffects eff in turnEffectsCopy)
        {
            Debug.Log(eff.getEffect(EffectType.Damage));
            // Armor does not protect against DoT
            currentHealth -= eff.getEffect(EffectType.Damage);
            attributes[Attributes.ARMOR] += eff.getEffect(EffectType.Armor);
            attributes[Attributes.STRENGTH] += eff.getEffect(EffectType.Strength);

            if (eff.Turns == 1)
            {
                multipleTurnEffects.Remove(eff);
            }
        }

        // Decrease effects by 1 turn
        foreach (CardEffects eff in multipleTurnEffects)
        {
            eff.setTurns(eff.Turns - 1);
        }

        healthAndStatus.updateAttributes(false, attributes);
        healthAndStatus.setHealth(currentHealth, maxHealth);
    }

    public int getCurrentHealth()
    {
        return currentHealth;
    }
    
    public Dictionary<Attributes, int> getAttributes()
    {
        return attributes;
    }

    public bool checkifTargetable()
    {
        return targetableObject.isTargetable();
    }

    public void setMaxHealth(int value)
    {
        maxHealth = value;
    }
}
