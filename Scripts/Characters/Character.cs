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
    private Dictionary<EffectType, System.Action> endTurnBehaviors;
    protected List<CardEffect> multipleTurnEffects = new List<CardEffect>();
    [SerializeField] protected int id;
    protected bool dead = false;

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

        initializeEndTurnBehaviors();

        // Set up after first frame
        StartCoroutine(findComponentsAfterFrame());
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

    public virtual void processCardEffects(List<CardEffect> effects, Enemy enemy = null)
    {
        int damageDealt = 0;

        if (enemy != null)
        {
            Debug.Log("Process Card Enemy: " + enemy.currentHealth);
        }

        foreach (CardEffect effect in effects)
        {
            addAudioCue(effect);

            // Calculate this with on-hit effects 
            if (effect.type == EffectType.HealDamageDone)
            {
                continue;
            }

            if (effect.type == EffectType.Damage)
            {
                // Damage
                if (effect.value - attributes[EffectType.Armor] > 0)
                {
                    currentHealth -= effect.value - attributes[EffectType.Armor];
                    damageDealt = effect.value - attributes[EffectType.Armor];
                    showFloatingFeedbackUI(damageDealt.ToString(), Color.crimson);
                    attributes[EffectType.Armor] = 0;
                    AudioManager.instance.playDamage();
                }
                else
                {
                    attributes[EffectType.Armor] -= effect.value;
                    showFloatingFeedbackUI(effect.value.ToString(), Color.darkSlateGray);
                    AudioManager.instance.playBlock();
                }
            }
            else if (effect.type == EffectType.Heal)
            {
                healCharacter(effect.value);
            }
            else
            {
                showFloatingFeedbackUI(effect.type.ToFeedbackString(), Color.blueViolet);
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

        processOnHitEffects(effects, damageDealt, attributes, enemy);

        foreach (KeyValuePair<EffectType, int> attribute in attributes)
        {
            uIUpdater.updateEffect(attribute.Key, attribute.Value);
        }

        if (currentHealth == 0 || currentHealth < 0)
        {
            currentHealth = 0;
            dead = true;
            HandManager.instance.checkOnDeathEffect();
        }

        uIUpdater.setHealth(currentHealth, maxHealth);
    }

    void addAudioCue(CardEffect effect)
    {
        switch (effect.type)
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
                // Add general debuff
                AudioManager.instance.playDebuff();
                break;
            
        }
    }

    void processOnHitEffects(List<CardEffect> effects, int damageDealt, Dictionary<EffectType, int> attributes, Enemy enemy = null)
    {
        CardEffect healDamage = effects.Find((effect) => effect.type == EffectType.HealDamageDone);
        CardEffect divintiyCard = effects.Find((effect) => effect.type == EffectType.Divinity);

        if (healDamage != null)
        {
            if (enemy == null)
            {
                sceneController.healPlayer(damageDealt);
            }
            else
            {
                enemy.healCharacter(damageDealt);
            }
        }

        // Can not heal from divinity effect if the card applies divinity
        if (attributes[EffectType.Divinity] > 0 && damageDealt > 0 && divintiyCard == null)
        {
            if (enemy == null)
            {
                sceneController.healPlayer(5);
            }
            else
            {
                enemy.healCharacter(5);
            }
        }

        // Check Frostbite threshold
        checkFrostbiteThreshold(damageDealt);
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

        foreach (KeyValuePair<EffectType, int> attribute in attributes)
        {
            uIUpdater.updateEffect(attribute.Key, attribute.Value);
        }
        uIUpdater.setHealth(currentHealth, maxHealth);
    }

    public void processEndOfTurnEffects()
    {
        foreach (var effect in endTurnBehaviors)
        {
            if (attributes.ContainsKey(effect.Key) && attributes[effect.Key] > 0)
            {
                effect.Value.Invoke();
            }
        }

        // update UI
        foreach (KeyValuePair<EffectType, int> attribute in attributes)
        {
            uIUpdater.updateEffect(attribute.Key, attribute.Value);
        }
        uIUpdater.setHealth(currentHealth, maxHealth);
    }

    private void initializeEndTurnBehaviors()
    {
        endTurnBehaviors = new Dictionary<EffectType, System.Action>
        {
            { EffectType.Weaken, () => decrementEffect(EffectType.Weaken) },
            { EffectType.Divinity, () => decrementEffect(EffectType.Divinity) },
            { EffectType.Frostbite, () => clearEffect(EffectType.Frostbite) },
            // Add more here
        };
    }

    private void decrementEffect(EffectType type)
    {
        attributes[type] = Mathf.Max(0, attributes[type] - 1);
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
