using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class KnightCardProccesor : EnemyCardProcessor
{
    private Dictionary<string, SpecialEnemyCardLogicInterface> specialCards;

    public KnightCardProccesor(ParentSceneController parentSceneController) : base(parentSceneController)
    {
        specialCards = new Dictionary<string, SpecialEnemyCardLogicInterface>
        {
            {"Worldreaver", new WorldreaverLogic()},
            {"Rending Edge", new RendingEdgeLogic()},
            {"Raging Blow", new RagingBlowLogic()},
            {"Power Stance", new PowerStanceLogic()},
            {"Cry Valhalla", new CryValhallaLogic()},
            {"Combo Attack", new ComboAttackLogic()}
        };
    }

    public override List<CardEffect> processCard(CardModelSO card, Dictionary<EffectType, int> attributes, Enemy enemy)
    {
        if (card.special)
        {
            enemy.playAnimation(card.type);
            return processSpecialCard(card, attributes, enemy);
        }

        return base.processCard(card, attributes, enemy);
    }

    protected override List<CardEffect> processSpecialCard(CardModelSO specialCard, Dictionary<EffectType, int> attributes, Enemy enemy)
    {
        SpecialEnemyCardLogicInterface specialCardLogic = specialCards[specialCard.title];

        if (specialCardLogic == null)
        {
            Debug.LogError($"No Special Card Logic for {specialCard.title}");
            return new List<CardEffect>();
        }
        
        return specialCardLogic.process(specialCard,attributes,sceneController, enemy);
    }
}

public class ComboAttackLogic: SpecialEnemyCardLogicInterface
{
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = card.getEffects();

        cardEffects = parentSceneController.checkCritHits(cardEffects);

        // Add armor if critical hit
        if (cardEffects[0].critRate == 100)
        {
            enemy.addAttributeValue(EffectType.Armor, 5);
        }

        parentSceneController.processEnemyCardEffectsOnPlayer(cardEffects, enemy);

        return null;
    }
}

public class CryValhallaLogic: SpecialEnemyCardLogicInterface
{
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        int armor = enemy.getAttributeValue(EffectType.Armor);

        // Remove armor, add half the armor amount to strength
        enemy.updateAttribute(EffectType.Armor, 0);
        enemy.addAttributeValue(EffectType.Strength, armor / 2);

        return new List<CardEffect>();
    }
}

public class PowerStanceLogic: SpecialEnemyCardLogicInterface
{
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        enemy.updateAttribute(EffectType.Armor, enemy.getAttributeValue(EffectType.Armor) * 2);
        enemy.updateAttribute(EffectType.Strength, enemy.getAttributeValue(EffectType.Strength) * 2);


        return new List<CardEffect>();
    }
}

public class RagingBlowLogic: SpecialEnemyCardLogicInterface
{
    CardModelSO punctureCard = Resources.Load<CardModelSO>("ScriptableObjects/Cards/Enemies/RegularEnemies/Knight/Puncture");
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = card.getEffects();

        HandManager.instance.addCardToPlayerDeck(punctureCard);

        parentSceneController.processEnemyCardEffectsOnPlayer(cardEffects, enemy);

        return null;
    }
}

public class RendingEdgeLogic: SpecialEnemyCardLogicInterface
{
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = card.getEffects();

        // Find Puncture Cards
        DeckModelSO playerDeck = HandManager.instance.getPlayerDeck();
        List<CardModelSO> punctureCards = playerDeck.cards.FindAll((card) => card.title == "Puncture");

        if (punctureCards.Count > 0)
        {
            CardEffect weaken = new CardEffect
            {
                type = EffectType.Weaken,
                value = 2,
                turns = 0,
                critRate = 0
            };
            cardEffects.Add(weaken);
        }

        parentSceneController.processEnemyCardEffectsOnPlayer(cardEffects, enemy);


        return new List<CardEffect>();
    }
}

public class WorldreaverLogic: SpecialEnemyCardLogicInterface
{
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = card.getEffects();

        // Find Puncture Cards
        DeckModelSO playerDeck = HandManager.instance.getPlayerDeck();
        List<CardModelSO> punctureCards = playerDeck.cards.FindAll((card) => card.title == "Puncture");

        if (punctureCards.Count > 0)
        {
            cardEffects[0].value *= punctureCards.Count;
            parentSceneController.processEnemyCardEffectsOnPlayer(cardEffects, enemy);
        }

        return null;
    }
}