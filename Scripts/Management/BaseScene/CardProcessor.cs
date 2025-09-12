using System;
using System.Collections.Generic;
using UnityEngine;

public class CardProcessor
{
    private readonly ParentSceneController sceneController;

    public CardProcessor(ParentSceneController controller)
    {
        sceneController = controller;
    }

    public List<CardEffect> processCard(Card card, Dictionary<EffectType, int> attributes)
    {
        List<CardEffect> cardEffects = applyEffectsToCardDamage(card.getEffects(), attributes);

        if (card.isSpecial())
        {
            sceneController.playAnimationsForCard(card.getCardType());
            return processSpecialCard(card, attributes);
        }

        int draw = card.getCardsToDraw();

        if (draw > 0)
        {
            HandManager.instance.drawCards(draw);
        }

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
    List<CardEffect> applyEffectsToCardDamage(List<CardEffect> cardEffects, Dictionary<EffectType, int> attributes)
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

    private List<CardEffect> processSpecialCard(Card specialCard, Dictionary<EffectType, int> attributes)
    {
        List<CardEffect> cardEffects = new List<CardEffect>();

        CardEffect damage = new CardEffect();
        damage.type = EffectType.Damage;

        CardEffect strength = new CardEffect();
        strength.type = EffectType.Strength;

        switch (specialCard.getCardTitle())
        {
            case "Stacked Hand":
                damage.value = (HandManager.instance.getNumCardsInHand() * 2) - 1;
                damage.turns = 0;
                cardEffects.Add(damage);
                break;
            case "Cleanse":
                sceneController.clearPlayerNegativeEffects();
                break;
            case "Corruptable":
                strength.value = HandManager.instance.getCorruptedCards().Count;
                cardEffects.Add(strength);
                break;
        }

        cardEffects = applyEffectsToCardDamage(cardEffects, attributes);

        return cardEffects;
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
        }
    }
}