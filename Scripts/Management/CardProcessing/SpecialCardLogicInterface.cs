using System.Collections.Generic;
using UnityEngine;

public interface SpecialCardLogicInterface
{
    List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController);
}


public class CleanseLogic: SpecialCardLogicInterface
{
    public List<CardEffect> process(Card card, Dictionary<EffectType, int> attributes, List<Enemy> enemies, ParentSceneController parentSceneController)
    {
        parentSceneController.clearPlayerNegativeEffects();
        return new List<CardEffect>();
    }
}

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

        if (effects[0].critRate == 100)
        {
            HandManager.instance.drawCards(drawCards);
        }

        return effects;
    }
}

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