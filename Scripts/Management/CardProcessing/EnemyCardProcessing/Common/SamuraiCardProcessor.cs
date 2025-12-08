using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SamuraiCardProcessor : EnemyCardProcessor
{
    public SamuraiCardProcessor(ParentSceneController parentSceneController) : base(parentSceneController)
    {
        specialCards = new Dictionary<string, SpecialEnemyCardLogicInterface>
        {
            {"Tainted Blade", new TainedBladeLogic()},
            {"Bankai", new BankaiLogic()},
            {"Bloody Strike", new BloodyStrikeLogic()},
        };
    }
}

public class TainedBladeLogic: SpecialEnemyCardLogicInterface
{
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = card.getEffects();

        DeckModelSO playerDeck = HandManager.instance.getPlayerDeck();
        
        // Find Gash Cards
        List<CardModelSO> gashCards = playerDeck.cards.FindAll((card) => card.title == "Gash");

        if (gashCards != null)
        {
            cardEffects[0].value *= gashCards.Count;
        }

        int healAmount = cardEffects[0].value / 2;

        enemy.healCharacter(healAmount);

        return cardEffects;
    }
}

public class BankaiLogic: SpecialEnemyCardLogicInterface
{
    private CardModelSO gashCard = Resources.Load<CardModelSO>("ScriptableObjects/Cards/Enemies/RegularEnemies/Samurai/Gash");
    
    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = card.getEffects();

        DeckModelSO playerDeck = HandManager.instance.getPlayerDeck();
        
        // Find Gash Cards
        List<CardModelSO> gashCards = playerDeck.cards.FindAll((card) => card.title == "Gash");

        if (gashCards != null)
        {
            cardEffects[0].value *= gashCards.Count;
        }

        // Consume all Gash Cards
        HandManager.instance.removeAllOfSpecificCardFromPlayerDeck(gashCard);

        return cardEffects;
    }
}


public class BloodyStrikeLogic: SpecialEnemyCardLogicInterface
{
    private CardModelSO gashCard = Resources.Load<CardModelSO>("ScriptableObjects/Cards/Enemies/RegularEnemies/Samurai/Gash");

    public List<CardEffect> process(CardModelSO card, Dictionary<EffectType, int> attributes, ParentSceneController parentSceneController, Enemy enemy)
    {
        List<CardEffect> cardEffects = card.getEffects();
    
        // Add Gash card to player deck
        CardModelSO gash = gashCard.clone();

        HandManager.instance.addCardToPlayerDeck(gash);

        return cardEffects;
    }
}