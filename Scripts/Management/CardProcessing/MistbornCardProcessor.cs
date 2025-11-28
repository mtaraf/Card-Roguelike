using System.Collections.Generic;
using UnityEngine;


public class MistbornCardProcessor : CardProcessor
{
    private Dictionary<string, SpecialCardLogicInterface> specialCards;

    public MistbornCardProcessor(ParentSceneController parentSceneController) : base(parentSceneController)
    {
        specialCards = new Dictionary<string, SpecialCardLogicInterface>
        {
            // Corruption
            {"Cleanse", new CleanseLogic()},
            {"Cleanse+", new CleanseLogic()},
            {"Corrupt Dagger", new CorruptDaggerLogic()},
            {"Corrupt Dagger+", new CorruptDaggerLogic()},
            {"Greed", new GreedLogic()},
            {"Greed+", new GreedLogic()},
            // Bleed
            {"Taste of Blood", new TasteOfBloodLogic()},
            {"Taste of Blood+", new TasteOfBloodLogic()},
            {"Vampiric", new VampiricLogic()},
            {"Vampiric+", new VampiricLogic()},
            // Crit
            {"Luck of the Draw", new LuckOfTheDrawLogic(1)},
            {"Luck of the Draw+", new LuckOfTheDrawLogic(2)},
            {"Teamwork", new TeamworkLogic()},
            {"Teamwork+", new TeamworkLogic()},
            // Dagger
            {"Close Combat", new CloseCombatLogic()},
            {"Close Combat+", new CloseCombatLogic()},
            {"Dance of Daggers", new DanceOfDaggersLogic()},
            {"Dance of Daggers+", new DanceOfDaggersLogic()},
            {"Hidden Inventory", new HiddenInventoryLogic(2)},
            {"Hidden Inventory+", new HiddenInventoryLogic(3)},
            // General
        };
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
        SpecialCardLogicInterface specialCardLogic = specialCards[specialCard.getCardTitle()];

        if (specialCardLogic == null)
        {
            Debug.LogError($"No Special Card Logic for {specialCard.getCardTitle()}");
            return new List<CardEffect>();
        }

        List<CardEffect> cardEffects = specialCardLogic.process(specialCard,attributes,enemies,sceneController);

        return applyEffectsToCardDamage(cardEffects, attributes);

        // CardEffect damage = new CardEffect();
        // damage.type = EffectType.Damage;

        // CardEffect strength = new CardEffect();
        // strength.type = EffectType.Strength;

        // if ((specialCard.getCardTarget() == Target.Enemy_Multiple || specialCard.getCardTarget() == Target.Enemy_Single) && enemies == null)
        // {
        //     Debug.LogError("No enemies selected for card processing");
        //     return cardEffects;
        // }

        // switch (specialCard.getCardTitle())
        // {
        //     case "Cleanse":
        //     case "Cleanse+":
        //         sceneController.clearPlayerNegativeEffects();
        //         break;
        //     case "Blind Spot":
        //     case "Blind Spot+":
        //         if (enemies[0].getAttributes()[EffectType.Blind] > 0)
        //         {
        //             damage.value = specialCard.getEffects()[0].value * 2;
        //         }
        //         else
        //         {
        //             damage.value = specialCard.getEffects()[0].value;
        //         }
        //         cardEffects.Add(damage);
        //         break;
        //     case "Divine Smite":
        //     case "Divine Smite+":
        //         damage.value = specialCard.getEffects()[0].value;
        //         cardEffects.Add(damage);
        //         break;
        //     case "Corruptable":
        //         strength.value = HandManager.instance.getCorruptedCards().Count;
        //         cardEffects.Add(strength);
        //         break;
        // }

        // cardEffects = applyEffectsToCardDamage(cardEffects, attributes);

        // return cardEffects;
    }
}
