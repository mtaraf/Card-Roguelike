using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DeathWitchCardProcessor : EnemyCardProcessor
{
    private Dictionary<string, SpecialEnemyCardLogicInterface> specialCards;

    public DeathWitchCardProcessor(ParentSceneController parentSceneController) : base(parentSceneController)
    {
        specialCards = new Dictionary<string, SpecialEnemyCardLogicInterface>
        {
            {"Spirit Path", new SpiritPathLogic()},
            {"Spiritual Luck", new SpiritualLuckLogic()},
            {"Tengu Strike", new TenguStrikeLogic()},
            {"Vanquish", new VanquishLogic()},
            {"Vanquisher's Charm", new VanquishersCharmLogic()},
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

public class SpiritPathLogic: SpecialEnemyCardLogicInterface
{
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = card.getEffects();



        return cardEffects;
    }
}

public class SpiritualLuckLogic: SpecialEnemyCardLogicInterface
{
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = card.getEffects();



        return cardEffects;
    }
}

public class TenguStrikeLogic: SpecialEnemyCardLogicInterface
{
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = card.getEffects();



        return cardEffects;
    }
}

public class VanquishLogic: SpecialEnemyCardLogicInterface
{
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = card.getEffects();



        return cardEffects;
    }
}

public class VanquishersCharmLogic: SpecialEnemyCardLogicInterface
{
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = card.getEffects();



        return cardEffects;
    }
}