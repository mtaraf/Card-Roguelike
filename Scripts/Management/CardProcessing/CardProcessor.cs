using System.Collections.Generic;
using UnityEngine;

public class CardProcessor
{
    protected ParentSceneController sceneController;

    public CardProcessor(ParentSceneController parentSceneController)
    {
        sceneController = parentSceneController;
    }

    public virtual List<CardEffect> processCard(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies)
    {
        List<CardEffect> cardEffects = applyEffectsToCardDamage(card.getEffects(), attributes);

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

        // Roll critical hits
        cardEffects = checkCritHits(cardEffects);

        sceneController.playAnimationsForCard(card.getCardType());

        return cardEffects;
    }

    public List<CardEffect> processEnemyCard(CardModelSO model, Dictionary<EffectType, int> attributes, Enemy enemy)
    {
        List<CardEffect> cardEffects = applyEffectsToCardDamage(model.effects, attributes);

        if (model.target == Target.Player)
        {
            sceneController.processEnemyCardEffectsOnPlayer(cardEffects, enemy);
            return null;
        }

        return cardEffects;
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
            Debug.Log("Damage after strength and weakness: " + modifiedEffects[damage_index].value);
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