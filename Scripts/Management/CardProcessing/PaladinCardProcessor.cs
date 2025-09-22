using System.Collections.Generic;
using UnityEngine;

public class PaladinCardProcessor : CardProcessor
{

    public PaladinCardProcessor(ParentSceneController parentSceneController) : base(parentSceneController)
    {
    }

    public override List<CardEffect> processCard(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies)
    {
        if (card.isSpecial())
        {
            sceneController.playAnimationsForCard(card.getCardType());
            return processSpecialCard(card, attributes, enemies);
        }

        return base.processCard(card, attributes, enemies);
    }

    protected override List<CardEffect> processSpecialCard(Card specialCard, Dictionary<EffectType, int> attributes, List<Enemy> enemies)
    {
        List<CardEffect> cardEffects = new List<CardEffect>();

        CardEffect damage = new CardEffect();
        damage.type = EffectType.Damage;

        CardEffect strength = new CardEffect();
        strength.type = EffectType.Strength;

        if ((specialCard.getCardTarget() == Target.Enemy_Multiple || specialCard.getCardTarget() == Target.Enemy_Single) && enemies == null)
        {
            Debug.LogError("No enemies selected for card processing");
            return cardEffects;
        }

        switch (specialCard.getCardTitle())
        {
            case "Blind Spot":
            case "Blind Spot+":
                if (enemies[0].getAttributes()[EffectType.Blind] > 0)
                {
                    damage.value = specialCard.getEffects()[0].value * 2;
                }
                else
                {
                    damage.value = specialCard.getEffects()[0].value;
                }
                cardEffects.Add(damage);
                break;
            case "Divine Smite":
            case "Divine Smite+":
                damage.value = specialCard.getEffects()[0].value;
                cardEffects.Add(damage);
                break;
            case "Corruptable":
                strength.value = HandManager.instance.getCorruptedCards().Count;
                cardEffects.Add(strength);
                break;
        }

        cardEffects = applyEffectsToCardDamage(cardEffects, attributes);

        return cardEffects;
    }
}
