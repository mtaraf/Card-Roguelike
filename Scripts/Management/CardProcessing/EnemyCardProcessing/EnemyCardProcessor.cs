using System.Collections.Generic;
using UnityEngine;

public class EnemyCardProcessor
{
    protected ParentSceneController sceneController;
    protected Dictionary<string, SpecialEnemyCardLogicInterface> specialCards;

    public EnemyCardProcessor(ParentSceneController parentSceneController)
    {
        sceneController = parentSceneController;
    }

    // Returns card effects that apply to enemy, otherwise null
    public virtual List<CardEffect> processCard(CardModelSO card, Dictionary<EffectType, int> attributes, Enemy enemy)
    {
        List<CardEffect> cardEffects;

        if (card.special)
        {
            cardEffects = processSpecialCard(card, attributes, enemy);
        }
        else
        {
            cardEffects = card.getEffects();
        }

        cardEffects = applyEffectsToCardDamage(cardEffects, attributes);

        // Roll critical hits
        cardEffects = checkCritHits(cardEffects);

        enemy.playAnimation(card.type);

        Target cardTarget = card.target;
        if (cardTarget == Target.Player)
        {
            sceneController.processEnemyCardEffectsOnPlayer(cardEffects, enemy);
            return null;
        }
        else if (cardTarget == Target.Enemy_Multiple)
        {
            // TO-DO add functionality to allow enemies to buff different enemies
            return null;
        }

        return cardEffects;
    }

    // applies Strenth and Weakness attributes to the Cards effects
    protected List<CardEffect> applyEffectsToCardDamage(List<CardEffect> cardEffects, Dictionary<EffectType, int> attributes)
    {
        if (cardEffects == null)
            return new List<CardEffect>();

        // Deep copy the effects
        List<CardEffect> modifiedEffects = new List<CardEffect>();
        foreach (var effect in cardEffects)
        {
            modifiedEffects.Add(new CardEffect
            {
                type = effect.type,
                value = effect.value,
                turns = effect.turns,
                critRate = effect.critRate
            });
        }

        Debug.Log("Applying effects to damage.");

        int damage_index = modifiedEffects.FindIndex((effect) => effect.type == EffectType.Damage);
        if (damage_index != -1)
        {
            Debug.Log(attributes[EffectType.Weaken]);
            modifiedEffects[damage_index].value += attributes[EffectType.Strength] - attributes[EffectType.Weaken];
        }

        return modifiedEffects;
    }

    protected virtual List<CardEffect> processSpecialCard(CardModelSO specialCard, Dictionary<EffectType, int> attributes, Enemy enemy)
    {
        SpecialEnemyCardLogicInterface specialCardLogic = specialCards[specialCard.title];

        if (specialCardLogic == null)
        {
            Debug.LogError($"No Special Card Logic for {specialCard.title}");
            return new List<CardEffect>();
        }
        
        return specialCardLogic.process(specialCard,attributes,sceneController, enemy);
    }

    public List<CardEffect> checkCritHits(List<CardEffect> cardEffects)
    {
        foreach (CardEffect effect in cardEffects)
        {
            if (effect.type == EffectType.Damage && effect.critRate > 0)
            {
                float rand = Random.Range(0,101);
                if (rand <= effect.critRate)
                {
                    effect.critRate = 100;
                }
                else
                {
                    effect.critRate = 0;
                }
            }
        }

        return cardEffects;
    }
}