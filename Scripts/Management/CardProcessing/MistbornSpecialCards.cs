using System;
using System.Collections.Generic;
using UnityEngine;

// Bleed
public class TasteOfBloodLogic : MistbornSpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor)
    {
        List<CardEffect> effects = card.getEffects();

        if (enemies.Count < 1)
        {
            Debug.LogError("No enemies found for Taste of Blood");
            return effects;
        }

        if (enemies[0].hasAttribute(EffectType.Bleed))
            effects[0].critRate = 100;
        else
            effects[0].critRate = 0;

        return effects;
    }
}

public class BloodlustLogic : MistbornSpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor)
    {
        List<CardEffect> effects = card.getEffects();

        mistbornCardProcessor.setBloodlust(true, effects[0].value);

        return new List<CardEffect>();
    }
}

public class TransfusionLogic : MistbornSpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor)
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

public class RavageLogic : MistbornSpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor)
    {
        List<CardEffect> effects = card.getEffects();

        CardModelSO bloodyDagger = (CardModelSO)Resources.Load("ScriptableObjects/Cards/Mistborn/Bleed/LitheBloodyDagger");


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

public class BleedItOutLogic : MistbornSpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor)
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

public class VampiricLogic : MistbornSpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor)
    {
        List<CardEffect> effects = card.getEffects();

        if (enemies.Count < 1)
        {
            Debug.LogError("No enemies found for Taste of Blood");
            return effects;
        }

        // Heal player
        GameManager.instance.getCurrentSceneController().healPlayer(enemies[0].getAttributes()[EffectType.Bleed]);

        // Remove bleed from target
        enemies[0].updateAttribute(EffectType.Bleed, 0);

        return effects;
    }
}

public class BloodBurstLogic : MistbornSpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor)
    {
        List<CardEffect> effects = card.getEffects();

        effects[0].value = enemies[0].getAttributeValue(EffectType.Bleed);
        enemies[0].updateAttribute(EffectType.Bleed, 0);

        return effects;
    }
}

// Crit
public class LuckOfTheDrawLogic : MistbornSpecialCardLogicInterface
{
    int drawCards = 0;
    public LuckOfTheDrawLogic(int cards)
    {
        drawCards = cards;
    }
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor)
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

public class TeamworkLogic : MistbornSpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor)
    {
        List<CardEffect> effects = card.getEffects();

        effects[0].critRate = HandManager.instance.getNumCardsInHand() * effects[0].critRate;

        return effects;
    }
}

public class AssassinsMarkLogic : MistbornSpecialCardLogicInterface
{
    int str = 0;
    public AssassinsMarkLogic(int strength)
    {
        str = strength;
    }
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor)
    {
        List<CardEffect> cardEffects = card.getEffects();

        cardEffects = mistbornCardProcessor.checkCritHits(cardEffects);
        Player player = parentSceneController.getPlayer();

        if (cardEffects.Find((effect) => effect.type == EffectType.Damage).critRate == 100)
            player.addAttributeValue(EffectType.Strength, str);

        return cardEffects;
    }
}

public class AgileFocusLogic : MistbornSpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor)
    {
        List<CardEffect> cardEffects = card.getEffects();

        Player player = parentSceneController.getPlayer();
        if (player.getAttributeValue(EffectType.Agility) > 10)
            cardEffects[0].critRate = 100;

        return cardEffects;
    }
}

public class CriticalSuccessLogic : MistbornSpecialCardLogicInterface
{
    CardModelSO litheLuckyDagger = Resources.Load<CardModelSO>("ScriptableObjects/Cards/Mistborn/Crit/LitheLuckyDagger");

    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor)
    {
        List<CardEffect> cardEffects = card.getEffects();

        cardEffects = mistbornCardProcessor.checkCritHits(cardEffects);

        if (cardEffects[0].critRate == 100)
            HandManager.instance.addCardToCurrentHand(litheLuckyDagger.clone());

        return cardEffects;
    }
}

// Corruption
public class CorruptDaggerLogic : MistbornSpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor)
    {
        List<CardEffect> effects = card.getEffects();

        int corruptCards = HandManager.instance.getCorruptedCards().Count;

        if (corruptCards == 0)
            corruptCards = 1;

        effects[0].value *= corruptCards;

        return effects;
    }
}

public class CleanseLogic : MistbornSpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor)
    {
        parentSceneController.clearPlayerNegativeEffects();
        return new List<CardEffect>();
    }
}

public class GreedLogic : MistbornSpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor)
    {
        List<CardEffect> effects = card.getEffects();

        if (enemies[0].hasAttribute(EffectType.Corruption))
        {
            // Heal player
            GameManager.instance.getCurrentSceneController().healPlayer(enemies[0].getAttributeValue(EffectType.Corruption));
        }

        return effects;
    }
}

// Dagger
public class CloseCombatLogic : MistbornSpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor)
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

public class DanceOfDaggersLogic : MistbornSpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor)
    {
        List<CardEffect> effects = card.getEffects();

        effects[0].value *= parentSceneController.getPlayerAttributes()[EffectType.Agility];

        return effects;
    }
}

public class HiddenInventoryLogic : MistbornSpecialCardLogicInterface
{
    int num = 0;
    public HiddenInventoryLogic(int numCards)
    {
        num = numCards;
    }
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController, MistbornCardProcessor mistbornCardProcessor)
    {
        CardModelSO glassDaggerCard = (CardModelSO)Resources.Load("ScriptableObjects/Cards/Mistborn/Dagger/LitheGlassDagger");

        bool success;

        for (int i = 0; i < num; i++)
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