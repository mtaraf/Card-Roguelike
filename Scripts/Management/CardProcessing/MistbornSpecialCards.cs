using System;
using System.Collections.Generic;
using UnityEngine;

// Bleed
public class TasteOfBloodLogic: SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController)
    {
        List<CardEffect> effects = card.getEffects();
        

        if (enemies.Count < 1)
        {
            Debug.LogError("No enemies found for Taste of Blood");
            return effects;
        }

        if (enemies[0].hasAttribute(EffectType.Bleed))
            effects[0].value *= (int)2.5;

        return effects;
    }
}

public class TransfusionLogic: SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController)
    {
        List<CardEffect> effects = card.getEffects();
        

        if (enemies.Count < 1)
        {
            Debug.LogError("No enemies found for Taste of Blood");
            return effects;
        }

        if (enemies[0].hasAttribute(EffectType.Bleed))
        {
            CardModelSO transfusionCard = HandManager.instance.getCardInDeckDuringEncounter("Transfusion");
            transfusionCard.energy = Math.Max(0, transfusionCard.energy - 1);
            
        }

        return effects;
    }
}

public class RavageLogic: SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController)
    {
        List<CardEffect> effects = card.getEffects();

        CardModelSO bloodyDagger = (CardModelSO) Resources.Load("ScriptableObjects/Cards/Mistborn/Bleed/LitheBloodyDagger");
        

        if (enemies.Count < 1)
        {
            Debug.LogError("No enemies found for Taste of Blood");
            return effects;
        }

        if (enemies[0].hasAttribute(EffectType.Bleed))
            HandManager.instance.addCardToCurrentHand(bloodyDagger);

        return effects;
    }
}

public class BleedItOutLogic: SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController)
    {
        if (enemies.Count < 1)
        {
            Debug.LogError("No enemies found for Taste of Blood");
        }

        if (enemies[0].hasAttribute(EffectType.Bleed))
            enemies[0].updateAttribute(EffectType.Bleed, enemies[0].getAttributes()[EffectType.Bleed] * 2);

        return new List<CardEffect>();
    }
}

public class VampiricLogic: SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController)
    {
        List<CardEffect> effects = card.getEffects();

        if (enemies.Count < 1)
        {
            Debug.LogError("No enemies found for Taste of Blood");
            return effects;
        }

        // Get bleed value
        effects[0].value = enemies[0].getAttributes()[EffectType.Bleed];

        // Remove bleed from target
        enemies[0].updateAttribute(EffectType.Bleed, 0);

        // Heal for bleed value
        return effects;
    }  
}

// Crit
public class LuckOfTheDrawLogic: SpecialCardLogicInterface
{
    int drawCards = 0;
    public LuckOfTheDrawLogic(int cards)
    {
        drawCards = cards;
    }
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController)
    {
        CardProcessor cardProcessor = new CardProcessor(parentSceneController);
        List<CardEffect> effects = cardProcessor.processCard(card, attributes, enemies);

        Debug.Log("Crit hit: " + effects[0].critRate);

        if (effects[0].critRate == 100)
        {
            Debug.Log("Draw Card");
            HandManager.instance.drawCards(drawCards);
        }

        return effects;
    }
}

public class TeamworkLogic: SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController)
    {
        List<CardEffect> effects = card.getEffects();

        effects[0].critRate = HandManager.instance.getNumCardsInHand() * effects[0].critRate;

        return effects;
    }
}

// Corruption
public class CorruptDaggerLogic: SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController)
    {
        List<CardEffect> effects = card.getEffects();
        
        int corruptCards = HandManager.instance.getCorruptedCards().Count;

        if (corruptCards == 0)
            corruptCards = 1;

        effects[0].value *= corruptCards;

        return effects;
    }
}

public class CleanseLogic: SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController)
    {
        parentSceneController.clearPlayerNegativeEffects();
        return new List<CardEffect>();
    }
}

public class GreedLogic: SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController)
    {
        List<CardEffect> effects = card.getEffects();

        if (enemies[0].hasAttribute(EffectType.Corruption))
        {
            CardEffect heal = new CardEffect
            {
                type = EffectType.Heal,
                value = enemies[0].getAttributes()[EffectType.Corruption],
                turns = 0,
                critRate = 0
            };
            effects.Add(heal);

            enemies[0].updateAttribute(EffectType.Corruption, 0);
        }

        return effects;
    }
}

// Dagger
public class CloseCombatLogic: SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController)
    {
        List<CardEffect> effects = card.getEffects();

        int daggerCards = 0;
        List<CardModelSO> cards = HandManager.instance.getPlayerDeck().cards;

        foreach (CardModelSO item in cards)
        {
            if (item.title.ToLower().Contains("dagger"))
                daggerCards++;
        }

        effects[0].value *= daggerCards;

        return effects;
    }
}

public class DanceOfDaggersLogic: SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController)
    {
        List<CardEffect> effects = card.getEffects();

        effects[0].value *= parentSceneController.getPlayerAttributes()[EffectType.Agility];

        return effects;
    }
}

public class HiddenInventoryLogic:  SpecialCardLogicInterface
{
    int num = 0;
    public HiddenInventoryLogic(int numCards)
    {
        num = numCards;
    }
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController)
    {
        CardModelSO glassDaggerCard = (CardModelSO) Resources.Load("ScriptableObjects/Cards/Mistborn/StarterCards/GlassDagger");

        bool success = true;

        for (int i=0; i<num; i++)
        {
            success = HandManager.instance.addCardToCurrentHand(glassDaggerCard.clone());

            if (!success)
            {
                Debug.Log("Hand is full");
                // TO-DO: add hand is full message to player
                break;
            }
        }

        return new List<CardEffect>();
    }
}

// General