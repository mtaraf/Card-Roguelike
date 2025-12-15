using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Character : MonoBehaviour
{
    // Attributes
    [SerializeField] protected int maxHealth;
    protected int currentHealth;
    protected int weakness;
    protected Dictionary<EffectType, int> attributes = new Dictionary<EffectType, int>();
    [SerializeField] protected int id;
    protected bool dead = false;
    [SerializeField] private int xSpawnOffset;
    [SerializeField] private int ySpawnOffset;
    protected CardEffectHandler cardEffectHandler;
    private int corruptionLimit = 25;

    // UI
    protected UIUpdater uIUpdater;
    protected TargetableObject targetableObject;
    protected GameObject floatingFeedbackUI;
    protected Color floatingFeedbackColor = Color.crimson;
    protected RectTransform characterRect;
    protected FeedbackUI feedbackUI;

    // Animations
    [SerializeField] protected GameObject spriteObject;
    protected Animator animator;

    protected ParentSceneController sceneController;

    public virtual void Start()
    {
        // Initializers
        targetableObject = GetComponent<TargetableObject>();
        floatingFeedbackUI = Resources.Load<GameObject>("UI/General/Feedback/FloatingFeedbackUIPrefab");
        characterRect = GetComponent<RectTransform>();

        feedbackUI = transform.AddComponent<FeedbackUI>();

        foreach (EffectType effectType in Enum.GetValues(typeof(EffectType)))
        {
            attributes.Add(effectType, 0);
        }

        cardEffectHandler = new CardEffectHandler(this);

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
        StartCoroutine(cardEffectHandler.processCardEffects(effects, enemy));
    }

    public virtual int processDamage(int damage, int critRate, DamageType type)
    {
        return cardEffectHandler.processDamage(damage, critRate, type);
    }

    public virtual void updateAttribute(EffectType type, int value)
    {
        attributes[type] = value;
        uIUpdater.updateEffect(type, attributes[type]);
    }

    public virtual void addAttributeValue(EffectType type, int value)
    {
        attributes[type] += value;
        uIUpdater.updateEffect(type, attributes[type]);
    }

    public void addAgility(int value)
    {
        attributes[EffectType.Agility] = Math.Min(attributes[EffectType.Agility] + value, 20);
        uIUpdater.updateEffect(EffectType.Agility, attributes[EffectType.Agility]);
    }

    public void addCorruption(int amount)
    {
        Debug.Log($"Amount: {amount} limit: {corruptionLimit}");
        int currentCorruption = attributes[EffectType.Corruption];
        currentCorruption += amount;

        if (currentCorruption >= corruptionLimit)
        {
            consumeCorruption(corruptionLimit);
            return;
        }

        updateAttribute(EffectType.Corruption, currentCorruption);
    }

    public void consumeCorruption(int amount)
    {
        // TO-DO: Add corruption animation!
        sceneController.healPlayer(amount);
        attributes[EffectType.Corruption] = 0;
        processDamage(amount, 0, DamageType.General);
    }

    public void updateCurrentHealth(int health)
    {
        currentHealth = health;
        uIUpdater.setHealth(currentHealth, maxHealth);
    }

    public void processEndOfTurnEffects()
    {
        // Dictionary<EffectType, int> attributesCopy = new Dictionary<EffectType, int>(attributes);
        // foreach (EffectType type in attributesCopy.Keys)
        // {
        //     if (attributesCopy[type] > 0)
        //     {
        //         IStatusEffect effect = StatusEffectFactory.create(type);
        //         effect?.onEndRound(this);
        //     }
        // }
    }

    public virtual void processEndOfRoundEffects()
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
        return attributes[effectType] > 0;
    }

    public int getAttributeValue(EffectType effectType)
    {
        return attributes[effectType];
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
        showFloatingFeedbackUI(DamageType.Heal, amount.ToString());
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

    public void showFloatingFeedbackUI(DamageType type, string message)
    {
        feedbackUI.showFloatingFeedback(type, message);
    }

    public void setCharacterDead()
    {
        currentHealth = 0;
        dead = true;
    }
}
