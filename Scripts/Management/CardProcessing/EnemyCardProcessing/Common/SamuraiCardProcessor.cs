using System.Collections.Generic;
using UnityEngine;


public class SamuraiCardProcessor : EnemyCardProcessor
{
    private Dictionary<string, SpecialEnemyCardLogicInterface> specialCards;

    public SamuraiCardProcessor(ParentSceneController parentSceneController) : base(parentSceneController)
    {
        specialCards = new Dictionary<string, SpecialEnemyCardLogicInterface>
        {
            {"Tainted Blade", new TainedBladeLogic()},
            {"Bankai", new BankaiLogic()},
            {"BloodyStrike", new BloodyStrikeLogic()},
        };
    }

    public override List<CardEffect> processCard(CardModelSO card, Dictionary<EffectType, int> attributes, Enemy enemy)
    {
        if (card.special)
        {
            sceneController.playAnimationsForCard(card.type);
            processSpecialCard(card, attributes, enemy);
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

public class TainedBladeLogic: SpecialEnemyCardLogicInterface
{
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = new List<CardEffect>(card.effects);

        DeckModelSO playerDeck = enemy.getPlayerCurrentDeck();
        
        // Find Gash Cards
        List<CardModelSO> gashCards = playerDeck.cards.FindAll((card) => card.title == "Gash");

        if (gashCards != null)
        {
            cardEffects[0].value *= gashCards.Count;
        }

        int healAmount = cardEffects[0].value / 2;

        parentSceneController.processEnemyCardEffectsOnPlayer(cardEffects, enemy);

        cardEffects = new List<CardEffect>();

        CardEffect heal = new CardEffect
        {
            type = EffectType.Heal,
            value = healAmount,
            turns = 0,
            critRate = 0
        };

        cardEffects.Add(heal);

        return cardEffects;
    }
}

public class BankaiLogic: SpecialEnemyCardLogicInterface
{
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = new List<CardEffect>(card.effects);

        DeckModelSO playerDeck = enemy.getPlayerCurrentDeck();
        
        // Find Gash Cards
        List<CardModelSO> gashCards = playerDeck.cards.FindAll((card) => card.title == "Gash");

        if (gashCards != null)
        {
            cardEffects[0].value *= gashCards.Count;
        }

        parentSceneController.processEnemyCardEffectsOnPlayer(cardEffects, enemy);

        return new List<CardEffect>();
    }
}


public class BloodyStrikeLogic: SpecialEnemyCardLogicInterface
{
    private CardModelSO gashCard = Resources.Load<CardModelSO>("ScriptableObjects/Cards/Enemies/RegularEnemies/Samurai/Gash");

    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = new List<CardEffect>(card.effects);
    
        // Add Gash card to player deck
        CardModelSO gash = gashCard.clone();

        HandManager.instance.addCardToPlayerDeck(gash);

        parentSceneController.processEnemyCardEffectsOnPlayer(cardEffects, enemy);

        return new List<CardEffect>();
    }
}