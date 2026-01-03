using System;
using System.Collections.Generic;
using UnityEngine;

public class CardProcessor
{
    protected ParentSceneController sceneController;

    private Tuple<bool, int> nextTurnEnergyLoss = new Tuple<bool, int>(false, 0);

    public CardProcessor(ParentSceneController parentSceneController)
    {
        sceneController = parentSceneController;
    }

    public virtual List<CardEffect> processCard(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies)
    {
        List<CardEffect> cardEffects = card.getEffects();

        // Roll critical hits
        checkCritHits(cardEffects);

        cardEffects = applyEffectsToCardDamage(cardEffects, attributes);

        int draw = card.getCardsToDraw();
        int discard = card.getCardsDiscarded();

        if (draw > 0)
        {
            HandManager.instance.drawCards(draw);
        }

        if (discard > 0)
        {
            // TO-DO: add discard functionality to parent scene controller and call here
        }

        sceneController.playAnimationsForCard(card.getCardType());

        return cardEffects;
    }

    // applies Strenth, Weakness attributes to the Cards effects
    public static List<CardEffect> applyEffectsToCardDamage(List<CardEffect> cardEffects, Dictionary<EffectType, int> attributes)
    {
        if (cardEffects == null || cardEffects.Count == 0)
            return cardEffects;

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

        foreach (CardEffect effect in modifiedEffects)
        {
            if (effect.type == EffectType.Damage)
            {
                effect.value += attributes[EffectType.Strength] - attributes[EffectType.Weaken];
            }
        }

        return modifiedEffects;
    }

    protected virtual List<CardEffect> processSpecialCard(Card specialCard, Dictionary<EffectType, int> attributes, List<Enemy> enemies)
    {
        return new List<CardEffect>();
    }

    public void checkOnDeathEffect(Card lastCardPlayed)
    {
        switch (lastCardPlayed.getCardTitle())
        {
            case "Scaling Strike":
                List<CardEffect> effects = lastCardPlayed.getCardModel().effects;
                CardEffect damage = effects.Find((effect) => effect.type == EffectType.Damage);
                damage.value += 2;
                string details = "Deals " + damage.value + " damage. Corrupts. When this card deals the killing blow, adds 2 damage.";
                lastCardPlayed.setCardDetails(details);
                break;
            default:
                break;
        }
    }

    public List<CardEffect> checkCritHits(List<CardEffect> cardEffects)
    {
        foreach (CardEffect effect in cardEffects)
        {
            if (effect.type == EffectType.Damage && effect.critRate > 0)
            {
                float rand = UnityEngine.Random.Range(0, 101);
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

    public virtual void endOfRoundEffects()
    {

    }

    public virtual void startOfRoundEffects()
    {
        if (nextTurnEnergyLoss.Item1)
        {
            sceneController.usePlayerEnergy(nextTurnEnergyLoss.Item2);
            nextTurnEnergyLoss = new Tuple<bool, int>(false, 0);
        }
    }

    public void setNextTurnEnergyLoss(int energy)
    {
        nextTurnEnergyLoss = new Tuple<bool, int>(true, nextTurnEnergyLoss.Item2 + energy);
    }
}