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
    WARD = 1,
    ARMOR = 2
}

public static class AttributesExtensions
{
    public static string ToDisplayString(this Attributes att)
    {
        switch (att)
        {
            case Attributes.STRENGTH: return "StrengthEffect";
            case Attributes.ARMOR: return "ArmorEffect";
            case Attributes.WARD: return "WardEffect";
            default: return "Default";
        }
    }
}


public class Character : MonoBehaviour
{
    // Attributes
    [SerializeField] protected int maxHealth;
    protected int currentHealth; // Set current health to max health only when a run starts not in start for player
    protected int strength;
    protected int weakness;
    protected int armor;
    protected int ward;
    protected Dictionary<Attributes, int> attributes = new Dictionary<Attributes, int>();

    // UI
    protected HealthAndStatus healthAndStatus;
    protected TargetableObject targetableObject;

    public virtual void Start()
    {
        // Initializers
        targetableObject = GetComponent<TargetableObject>();

        currentHealth = maxHealth;
        attributes.Add(Attributes.STRENGTH, 1);
        attributes.Add(Attributes.ARMOR, 0);
        attributes.Add(Attributes.WARD, 0);

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
        // Apply changes to attributes
        currentHealth -= effects.getTotalDamage();
        attributes[Attributes.ARMOR] += effects.getTotalArmor();
        attributes[Attributes.WARD] += effects.getTotalWard();

        healthAndStatus.updateAttributes(false, attributes);
        healthAndStatus.setHealth(currentHealth, maxHealth);
    }

    public int getCurrentHealth()
    {
        return currentHealth;
    }

    // public int getArmor()
    // {
    //     return armor;
    // }

    // public int getWard()
    // {
    //     return ward;
    // }

    // public int getStrength()
    // {
    //     return strength;
    // }
    public Dictionary<Attributes, int> getAttributes()
    {
        return attributes;
    }

    public bool checkifTargetable()
    {
        return targetableObject.isTargetable();
    }
}
