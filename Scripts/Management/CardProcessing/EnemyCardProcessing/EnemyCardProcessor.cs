using System.Collections.Generic;
using UnityEngine;

public class EnemyCardProcessor
{
    protected ParentSceneController sceneController;

    public EnemyCardProcessor(ParentSceneController parentSceneController)
    {
        sceneController = parentSceneController;
    }

    // Returns card effects that apply to enemy, otherwise null
    public virtual List<CardEffect> processCard(CardModelSO card, Dictionary<EffectType, int> attributes, Enemy enemy)
    {
        List<CardEffect> cardEffects = applyEffectsToCardDamage(card.effects, attributes);

        // Roll critical hits
        cardEffects = checkCritHits(cardEffects);

        sceneController.playAnimationsForCard(card.type);

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

    public void processSpecialCardEffectsOnPlayer(List<CardEffect> effects, Target target, Enemy enemy)
    {
        sceneController.processEnemyCardEffectsOnPlayer(effects, enemy);
    }

    // applies Strenth and Weakness attributes to the Cards effects
    protected List<CardEffect> applyEffectsToCardDamage(List<CardEffect> cardEffects, Dictionary<EffectType, int> attributes)
    {
        // Deep copy the effects
        List<CardEffect> modifiedEffects = new List<CardEffect>();
        foreach (var effect in cardEffects)
        {
            modifiedEffects.Add(new CardEffect
            {
                type = effect.type,
                value = effect.value,
                turns = effect.turns
            });
        }

        int damage_index = modifiedEffects.FindIndex((effect) => effect.type == EffectType.Damage);
        if (damage_index != -1)
        {
            modifiedEffects[damage_index].value += attributes[EffectType.Strength];
            modifiedEffects[damage_index].value -= Mathf.FloorToInt(modifiedEffects[damage_index].value * 0.2f * attributes[EffectType.Weaken]);
        }

        return modifiedEffects;
    }

    protected virtual List<CardEffect> processSpecialCard(CardModelSO specialCard, Dictionary<EffectType, int> attributes, Enemy enemy)
    {
        return new List<CardEffect>();
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