using System;
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
    [SerializeField] protected int id;
    protected bool dead = false;
    [SerializeField] private int xSpawnOffset;
    [SerializeField] private int ySpawnOffset;

    // UI
    protected UIUpdater uIUpdater;
    protected TargetableObject targetableObject;
    protected GameObject floatingFeedbackUI;
    protected Color floatingFeedbackColor = Color.crimson;
    protected RectTransform characterRect;

    // Animations
    [SerializeField] protected GameObject spriteObject;
    protected Animator animator;

    protected ParentSceneController sceneController;

    public virtual void Start()
    {
        // Initializers
        targetableObject = GetComponent<TargetableObject>();
        floatingFeedbackUI = Resources.Load<GameObject>("UI/General/FloatingFeedbackUIPrefab");
        characterRect = GetComponent<RectTransform>();

        foreach (EffectType effectType in Enum.GetValues(typeof(EffectType)))
        {
            attributes.Add(effectType, 0);
        }

        // Set up after first frame
        StartCoroutine(findComponentsAfterFrame());

        Vector2 position = transform.localPosition;
        transform.localPosition = new Vector2(position.x + xSpawnOffset, position.y + ySpawnOffset);
    }

    private IEnumerator findComponentsAfterFrame()
    {
        yield return null;

        sceneController = GameManager.instance.getCurrentSceneController();
        if (sceneController == null)
        {
            Debug.LogError("Could not find scene controller for character!");
        }

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


    // Card Processing
    public virtual void processCardEffects(List<CardEffect> effects, Enemy enemy = null)
    {
        if (enemy != null)
        {
            Debug.Log("Process Card Enemy: " + enemy.currentHealth);
        }

        // Damage processing
        int damageDealt = 0;
        CardEffect damage = effects.Find((effect) => effect.type == EffectType.Damage);
        CardEffect crit = effects.Find((effect) => effect.type == EffectType.Critical);
        if (damage != null)
        {
            int critRate = -1;
            if (crit != null)
            {
                critRate = crit.value;
                effects.Remove(crit);
            }
            damageDealt = processDamage(damage.value, critRate);
            effects.Remove(damage);
        }

        foreach (CardEffect effect in effects)
        {
            IStatusEffect statusEffect = StatusEffectFactory.create(effect);
            statusEffect?.apply(this, damageDealt);
        }

        // Check if character is dead
        if (currentHealth == 0 || currentHealth < 0)
        {
            currentHealth = 0;
            dead = true;
            HandManager.instance.checkOnDeathEffect();
        }
    }

    public int processDamage(int damage, int critRate)
    {
        int damageDealt = 0;

        // Check if Crit lands
        float rand = UnityEngine.Random.Range(0,101);
        if (rand <= critRate)
        {
            damage = (int)(damage * 2.5);
        }

        // Damage
        if (damage - attributes[EffectType.Armor] > 0)
        {
            currentHealth -= damage - attributes[EffectType.Armor];
            damageDealt = damage - attributes[EffectType.Armor];
            showFloatingFeedbackUI(damageDealt.ToString(), Color.crimson);
            attributes[EffectType.Armor] = 0;
            AudioManager.instance.playDamage();
        }
        else
        {
            attributes[EffectType.Armor] -= damage;
            showFloatingFeedbackUI(damage.ToString(), Color.darkSlateGray);
            AudioManager.instance.playBlock();
        }

        // Update Health
        uIUpdater.setHealth(currentHealth, maxHealth);

        return damageDealt;
    }

    void checkFrostbiteThreshold(int damageDealt)
    {
        // Check Frostbite threshold
        if (attributes[EffectType.Frostbite] > 0 && damageDealt > 0)
        {
            attributes[EffectType.Frostbite] -= damageDealt;

            // Check if threshold is met
            if (attributes[EffectType.Frostbite] <= 0)
            {
                // Do frostbite damage and frostbite animation
                attributes[EffectType.Frostbite] = 0;
                int frostbiteDamage = (int)(maxHealth * 0.1f);
                currentHealth -= frostbiteDamage;
                showFloatingFeedbackUI(frostbiteDamage.ToString(), Color.powderBlue);
                uIUpdater.setHealth(currentHealth, maxHealth);
            }

            // Updates the frostbite effect to either remove or lessen threshold
            uIUpdater.updateEffect(EffectType.Frostbite, attributes[EffectType.Frostbite]);
        }
    }

    public void updateAttribute(EffectType type, int value)
    {
        attributes[type] += value;
        uIUpdater.updateEffect(type, attributes[type]);
    }

    public void processEndOfTurnEffects()
    {
        Dictionary<EffectType, int> attributesCopy = new Dictionary<EffectType, int>(attributes);
        foreach (EffectType type in attributesCopy.Keys)
        {
            if (attributesCopy[type] > 0)
            {
                IStatusEffect effect = StatusEffectFactory.create(type);
                effect?.onEndRound(this);
            }
        }
    }

    public void addAudioCue(EffectType effect)
    {
        switch (effect)
        {
            case EffectType.Armor:
                // Add armor audio cue
                break;
            case EffectType.Strength:
                // Add armor audio cue
                AudioManager.instance.playBuff();
                break;
            case EffectType.Poison:
                // Add poison audio cue
                break;
            case EffectType.Frostbite:
                // Add armor audio cue
                break;
            case EffectType.Divinity:
            case EffectType.Weaken:
            case EffectType.Blind:
            case EffectType.Bleed:
                // Add general debuff
                AudioManager.instance.playDebuff();
                break;
            
        }
    }

    public void decrementEffect(EffectType type)
    {
        attributes[type] = Mathf.Max(0, attributes[type] - 1);
        uIUpdater.updateEffect(type, attributes[type]);
    }

    private void clearEffect(EffectType type)
    {
        attributes[type] = 0;
    }

    public int getCurrentHealth()
    {
        return currentHealth;
    }

    public int getMaxHealth()
    {
        return maxHealth;
    }

    public Dictionary<EffectType, int> getAttributes()
    {
        return attributes;
    }

    public bool hasAttribute(EffectType effectType)
    {
        Debug.Log(attributes[effectType]);
        return attributes[effectType] > 0;
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
        if (value < 0)
        {
            currentHealth = 0;
        }
        else
        {
            currentHealth = value;
        }
    }

    public void updateHealthUI()
    {
        uIUpdater.setHealth(currentHealth, maxHealth);
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

    public void healCharacter(int amount)
    {
        showFloatingFeedbackUI(amount.ToString(), Color.seaGreen);
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        uIUpdater.setHealth(currentHealth, maxHealth);
        AudioManager.instance.playHeal();
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

    public void showFloatingFeedbackUI(string message, Color color)
    {
        GameObject feedback = Instantiate(floatingFeedbackUI, transform);
        feedback.transform.localPosition = new Vector3(0, characterRect.sizeDelta.y / 2, 0);
        FloatingFeedbackUI feedbackUI = feedback.GetComponent<FloatingFeedbackUI>();
        feedbackUI.SetText(message, color);
        StartCoroutine(feedbackUI.moveAndDestroy());
    }
}
