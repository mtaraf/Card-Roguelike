using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;


public class Character : MonoBehaviour
{
    // Attributes
    [SerializeField] protected int maxHealth;
    protected int currentHealth; // Set current health to max health only when a run starts not in start for player
    protected int weakness;
    protected Dictionary<EffectType, int> attributes = new Dictionary<EffectType, int>();
    protected List<CardEffect> multipleTurnEffects = new List<CardEffect>();
    [SerializeField] protected int id;

    // UI
    protected UIUpdater uIUpdater;
    protected TargetableObject targetableObject;

    // Animations
    [SerializeField] protected GameObject spriteObject;
    protected Animator animator;

    public virtual void Start()
    {
        // Initializers
        targetableObject = GetComponent<TargetableObject>();

        currentHealth = maxHealth;
        attributes.Add(EffectType.Strength, 0);
        attributes.Add(EffectType.Armor, 0);
        attributes.Add(EffectType.Weaken, 0);
        attributes.Add(EffectType.Divinity, 0);
        attributes.Add(EffectType.Poison, 0);

        // Set up after first frame
        StartCoroutine(findComponentsAfterFrame());
    }

    private IEnumerator findComponentsAfterFrame()
    {
        yield return null;

        // UI set up
        uIUpdater = GetComponent<UIUpdater>();
        uIUpdater.setHealth(currentHealth, maxHealth);

        if (uIUpdater == null)
        {
            Debug.LogError($"UIUpdater not found on {gameObject.name}");
        }


        // Find Animator
        if (spriteObject == null)
        {
            Debug.Log("Sprite Object for " + gameObject.name + " not set in inspector");
        }
        animator = spriteObject.GetComponent<Animator>();
    }

    public virtual void processCardEffects(List<CardEffect> effects)
    {

        foreach (CardEffect effect in effects)
        {
            if (effect.type == EffectType.Damage)
            {
                // Damage
                if (effect.value - attributes[EffectType.Armor] > 0)
                {
                    currentHealth -= effect.value - attributes[EffectType.Armor];
                    attributes[EffectType.Armor] = 0;
                }
                else
                {
                    attributes[EffectType.Armor] -= effect.value;
                }
            }
            else
            {
                if (effect.turns > 0)
                {
                    // multiple turn effects
                    attributes[effect.type] += effect.value;
                    effect.turns--;
                    multipleTurnEffects.Add(effect);
                }
                else
                {
                    attributes[effect.type] += effect.value;
                }
            }
        }
        
        foreach (KeyValuePair<EffectType, int> attribute in attributes)
        {
            uIUpdater.updateEffect(attribute.Key, attribute.Value);
        }
        uIUpdater.setHealth(currentHealth, maxHealth);
    }

    public void processStartOfTurnEffects()
    {
        // Poison Damage
        currentHealth -= attributes[EffectType.Poison];

        // Decrease stacks
        attributes[EffectType.Poison] = 0;

        List<CardEffect> copy = new List<CardEffect>(multipleTurnEffects);

        // apply effects
        foreach (CardEffect effect in copy)
        {
            if (effect.turns == 0)
            {
                multipleTurnEffects.Remove(effect);
            }
            else
            {
                effect.turns--;

                attributes[effect.type] += effect.value;
            }
        }

        foreach (KeyValuePair<EffectType, int> attribute in attributes) {
            uIUpdater.updateEffect(attribute.Key, attribute.Value);
        }
        uIUpdater.setHealth(currentHealth, maxHealth);
    }

    public void processEndOfTurnEffects()
    {
        if (attributes[EffectType.Weaken] > 0)
        {
            attributes[EffectType.Weaken] -= 1;
        }
        if (attributes[EffectType.Divinity] > 0)
        {
            attributes[EffectType.Divinity] -= 1;
        }


        foreach (KeyValuePair<EffectType, int> attribute in attributes) {
            uIUpdater.updateEffect(attribute.Key, attribute.Value);
        }
        uIUpdater.setHealth(currentHealth, maxHealth);
    }


    public int getCurrentHealth()
    {
        return currentHealth;
    }

    public Dictionary<EffectType, int> getAttributes()
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

    public void setCurrentHealth(int value)
    {
        currentHealth = value;
    }

    public int getId()
    {
        return id;
    }

    public UIUpdater getUiUpdater()
    {
        return uIUpdater;
    }

    public void clearAllNegativeEffects()
    {
        Dictionary<EffectType, int> effectsCopy = new Dictionary<EffectType, int>(attributes);
        foreach (KeyValuePair<EffectType, int> pair in effectsCopy)
        {
            if (pair.Key != EffectType.Damage && pair.Key != EffectType.Armor && pair.Key != EffectType.Strength)
            {
                Debug.Log("Cleansed: " + pair.Key);
                attributes[pair.Key] = 0;
                uIUpdater.updateEffect(pair.Key, 0);
            }
        }
    }

    public void playAnimation(CardType type)
    {
        if (type == CardType.Attack)
        {
            animator.SetTrigger("attack");
        }
        else if (type == CardType.Defense)
        {
            animator.SetTrigger("defense");
        }
        else if (type == CardType.Buff)
        {
            animator.SetTrigger("buff");
        }
    }
}
