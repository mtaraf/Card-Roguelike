using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DeathWitchCardProcessor : EnemyCardProcessor
{
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
}

public class SpiritPathLogic: SpecialEnemyCardLogicInterface
{
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = card.getEffects();

        int strength = enemy.getAttributeValue(EffectType.Strength);
        enemy.updateAttribute(EffectType.Strength, 0);

        cardEffects[0].value = strength * 5;


        return cardEffects;
    }
}

public class SpiritualLuckLogic: SpecialEnemyCardLogicInterface
{
    CardModelSO hakuCurseCard = Resources.Load<CardModelSO>("ScriptableObjects/Cards/Enemies/MiniBosses/DeathWitch/Haku'sCurse");

    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = card.getEffects();

        cardEffects = parentSceneController.checkCritHits(cardEffects);

        if (cardEffects[0].critRate == 100)
        {
            HandManager.instance.addCardToPlayerDeck(hakuCurseCard);
        }

        return cardEffects;
    }
}

public class TenguStrikeLogic: SpecialEnemyCardLogicInterface
{
    CardModelSO hakuCurseCard = Resources.Load<CardModelSO>("ScriptableObjects/Cards/Enemies/MiniBosses/DeathWitch/Haku'sCurse");

    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = card.getEffects();

        HandManager.instance.addCardToPlayerDeck(hakuCurseCard);

        return cardEffects;
    }
}

public class VanquishLogic: SpecialEnemyCardLogicInterface
{
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = card.getEffects();

        int playerStrength = parentSceneController.getPlayerAttributes()[EffectType.Strength];
        Debug.Log($"Player strength: {playerStrength}");

        if (playerStrength == 0)
        {
            cardEffects[0].critRate = 100;
        }

        return cardEffects;
    }
}

public class VanquishersCharmLogic: SpecialEnemyCardLogicInterface
{
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        Player player = parentSceneController.getPlayer();
        int currentPlayerStrength = player.getAttributeValue(EffectType.Strength);

        player.updateAttribute(EffectType.Strength, currentPlayerStrength / 2);
        enemy.addAttributeValue(EffectType.Strength, currentPlayerStrength / 2);

        return null;
    }
}